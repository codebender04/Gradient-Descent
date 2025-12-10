using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MeshGenerator : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int resolutionX = 100; // number of quads along X
    [SerializeField] private int resolutionZ = 100; // number of quads along Z
    [SerializeField] private float sizeX = 10f;     // world size X
    [SerializeField] private float sizeZ = 10f;     // world size Z
    [SerializeField] private float heightScale = 1f;

    [Header("SinCos Parameters")]
    [SerializeField] private float freqX = 2f;
    [SerializeField] private float freqZ = 3f;
    [SerializeField] private float crossStrength = 0.5f;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    Mesh mesh;

    private void Awake()
    {
        EnsureComponents();
        Generate();
    }
    public void SetFunctionParameters(float a, float b, float c)
    {
        freqX = a;
        freqZ = b;
        crossStrength = c;

        Generate();
    }
    private void OnValidate()
    {
        resolutionX = Mathf.Max(1, resolutionX);
        resolutionZ = Mathf.Max(1, resolutionZ);
        sizeX = Mathf.Max(0.01f, sizeX);
        sizeZ = Mathf.Max(0.01f, sizeZ);
        heightScale = Mathf.Max(0f, heightScale);
        EnsureComponents();
        Generate();
    }

    private void EnsureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    // Generates a flat grid mesh whose y-values come from Evaluate(x,z)
    public void Generate()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "ProceduralSurface";
        }
        else
        {
            mesh.Clear();
        }

        int vertsX = resolutionX + 1;
        int vertsZ = resolutionZ + 1;
        Vector3[] verts = new Vector3[vertsX * vertsZ];
        Vector2[] uv = new Vector2[verts.Length];
        int[] tris = new int[resolutionX * resolutionZ * 6];

        // world-space origin is at transform.position; grid centered at that position
        float startX = -sizeX * 0.5f;
        float startZ = -sizeZ * 0.5f;
        float stepX = sizeX / resolutionX;
        float stepZ = sizeZ / resolutionZ;

        int vi = 0;
        for (int z = 0; z < vertsZ; z++)
        {
            for (int x = 0; x < vertsX; x++)
            {
                float wx = startX + x * stepX;
                float wz = startZ + z * stepZ;
                float wy = Evaluate(wx, wz) * heightScale;
                verts[vi] = new Vector3(wx, wy, wz);
                uv[vi] = new Vector2((float)x / resolutionX, (float)z / resolutionZ);
                vi++;
            }
        }

        int ti = 0;
        for (int z = 0; z < resolutionZ; z++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                int i0 = z * vertsX + x;
                int i1 = i0 + 1;
                int i2 = i0 + vertsX;
                int i3 = i2 + 1;

                // two triangles (i0, i2, i1) and (i1, i2, i3)
                tris[ti++] = i0;
                tris[ti++] = i2;
                tris[ti++] = i1;

                tris[ti++] = i1;
                tris[ti++] = i2;
                tris[ti++] = i3;
            }
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    // Evaluate the selected function at world-space (x,z) which is relative to the transform's local origin
    public float Evaluate(float x, float z)
    {
        return Mathf.Sin(freqX * x) + Mathf.Cos(freqZ * z) + crossStrength * Mathf.Sin(x + z);
    }

    // Returns the gradient vector (df/dx, df/dz) at world-space (x,z)
    public Vector2 Gradient(float x, float z)
    {
        float h = 0.01f;
        float fx = Evaluate(x, z);
        float dfdx = (Evaluate(x + h, z) - fx) / h;
        float dfdz = (Evaluate(x, z + h) - fx) / h;
        return new Vector2(dfdx, dfdz);
    }

    public float GetHeight(float x, float z)
    {
        return Evaluate(x, z) * heightScale + transform.position.y;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(sizeX, 0.01f, sizeZ);
        Gizmos.DrawWireCube(center, size);
    }
}