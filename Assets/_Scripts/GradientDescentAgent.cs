
// --------------------------------------------------
// Gradient descent agent that lives on the surface
// --------------------------------------------------
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class GradientDescentAgent : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private int maxTrailPoints = 500;
    [SerializeField] private MeshRenderer visual;

    [Header("Time Settings")]
    [SerializeField] private float stepInterval = 0.1f; // seconds per gradient update

    [Header("Optimization")]
    [SerializeField] private bool useMomentum = false;
    [SerializeField] private float momentum = 0.9f;

    private Vector2 velocity = Vector2.zero;

    private MeshGenerator surface;
    private float learningRate = 0.1f; // step size
    private float stepTimer = 0f;
    private LineRenderer lineRenderer;
    private List<Vector3> trail = new List<Vector3>();
    private bool isRunning = false;
    private float startX = 0f;
    private float startZ = 0f;
    private AgentInfoUI agentInfoUI;
    // internal xz state in surface-local coordinates
    private Vector2 xz;

    public void OnInstatiated(MeshGenerator surface, float startX, float startZ, float learningRate, bool useMomentum, AgentInfoUI agentInfoUI)
    {
        this.surface = surface;
        this.startX = startX;
        this.startZ = startZ;
        this.learningRate = learningRate;
        this.useMomentum = useMomentum;
        this.agentInfoUI = agentInfoUI;


        xz = new Vector2(startX, startZ);
        transform.position = ToWorldPos(xz);

        visual.material.color = Random.ColorHSV();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.material = visual.material;

        agentInfoUI.Initialize(visual.material.color, xz, surface.Gradient(xz.x, xz.y));
    }
    private void Update()
    {
        if (surface == null) return;

        if (isRunning)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                NextStep();
                stepTimer = 0f;
            }
        }

        Vector3 p = ToWorldPos(xz);
        transform.position = p;

        UpdateTrail(transform.position);
    }

    public void NextStep()
    {
        Vector2 gradient = surface.Gradient(xz.x, xz.y);

        if (useMomentum)
        {
            // Momentum update
            velocity = momentum * velocity - learningRate * gradient;
            xz += velocity;
        }
        else
        {
            // Standard GD
            xz -= learningRate * gradient;
        }

        agentInfoUI.UpdatePositionAndGradient(xz, gradient);
    }

    public void Run()
    {
        isRunning = true;
    }
    public void Stop()
    {
        isRunning = false;
    }
    public void SetLearningRate(float learningRate)
    {
        this.learningRate = learningRate;
    }
    public void SetUseMomentum(bool useMomentum)
    {
        this.useMomentum = useMomentum;
    }
    private Vector3 ToWorldPos(Vector2 xzLocal)
    {
        // The MeshGenerator uses coordinates relative to its transform's local origin
        Vector3 world = surface.transform.position + new Vector3(xzLocal.x, 0f, xzLocal.y);
        world.y = surface.GetHeight(xzLocal.x, xzLocal.y);
        return world;
    }

    private void UpdateTrail(Vector3 pos)
    {
        if (lineRenderer == null) return;
        trail.Add(pos);
        //if (trail.Count > maxTrailPoints) trail.RemoveAt(0);
        lineRenderer.positionCount = trail.Count;
        lineRenderer.SetPositions(trail.ToArray());
    }
    public void ResetToStart()
    {
        velocity = Vector2.zero;
        xz = new Vector2(startX, startZ);
        trail.Clear();
        if (lineRenderer != null) lineRenderer.positionCount = 0;
        transform.position = ToWorldPos(xz);

        agentInfoUI.UpdatePositionAndGradient(xz, surface.Gradient(xz.x, xz.y));
    }
    private void OnDestroy()
    {
        Destroy(agentInfoUI.gameObject);
    }
}
