using UnityEngine;

/// <summary>
/// Editor-like free-fly camera for play mode.
/// WASD: move, QE (or Space/Ctrl): up/down, Mouse drag/right-click: look
/// Hold Left Shift to boost speed.
/// Attach to your Camera and tweak speeds in the Inspector.
/// </summary>
[DisallowMultipleComponent]
public class FreeCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;           // base movement speed (units/sec)
    [SerializeField] private float boostMultiplier = 4f;     // multiplier when holding boost key
    [SerializeField] private KeyCode boostKey = KeyCode.LeftShift;
    [SerializeField] private bool useLocalSpace = true;      // move relative to transform rotation

    [Header("Vertical Movement")]
    [SerializeField] KeyCode upKey = KeyCode.Space;
    [SerializeField] private KeyCode downKey = KeyCode.LeftControl;

    [Header("Mouse Look")]
    [SerializeField] private bool requireRightMouse = true;  // only look while right mouse button is held
    [SerializeField] private float mouseSensitivity = 6f;    // higher -> faster rotation
    [SerializeField] private bool invertY = false;
    [SerializeField] private float maxPitch = 89f;           // clamp pitch to avoid flipping

    [Header("Smoothing")]
    [SerializeField] private bool smoothMovement = true;
    [SerializeField] private float movementSmoothTime = 0.06f;
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float rotationSmoothTime = 0.03f;

    // private state
    Vector3 currentVelocity = Vector3.zero;
    Vector3 smoothVelocity = Vector3.zero;
    float yaw = 0f;
    float pitch = 0f;

    private void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    private void Update()
    {
        ProcessLook();
        ProcessMovement();
    }

    private void ProcessLook()
    {
        if (requireRightMouse && !Input.GetMouseButton(1))
            return;

        // raw mouse delta
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        float inv = invertY ? 1f : -1f; // invert: moving mouse up should pitch down normally
        yaw += mx * mouseSensitivity;
        pitch += my * mouseSensitivity * inv;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        Quaternion targetRot = Quaternion.Euler(pitch, yaw, 0f);
        if (smoothRotation)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, clamp01(Time.deltaTime / rotationSmoothTime));
        else
            transform.rotation = targetRot;
    }

    private void ProcessMovement()
    {
        float forward = 0f;
        if (Input.GetKey(KeyCode.W)) forward += 1f;
        if (Input.GetKey(KeyCode.S)) forward -= 1f;

        float right = 0f;
        if (Input.GetKey(KeyCode.D)) right += 1f;
        if (Input.GetKey(KeyCode.A)) right -= 1f;

        float up = 0f;
        if (Input.GetKey(upKey)) up += 1f;
        if (Input.GetKey(downKey)) up -= 1f;

        Vector3 dir = new Vector3(right, up, forward);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        float speed = moveSpeed;
        if (Input.GetKey(boostKey)) speed *= boostMultiplier;

        Vector3 desiredVel = dir * speed;

        // convert to world space if moving relative to camera
        if (useLocalSpace)
            desiredVel = transform.TransformDirection(desiredVel);

        if (smoothMovement)
        {
            // SmoothDamp style smoothing
            smoothVelocity = Vector3.SmoothDamp(smoothVelocity, desiredVel, ref currentVelocity, movementSmoothTime);
            transform.position += smoothVelocity * Time.deltaTime;
        }
        else
        {
            transform.position += desiredVel * Time.deltaTime;
        }
    }

    // small helper to avoid dividing by zero when smoothing
    float clamp01(float v) => Mathf.Clamp01(v);

    // helpful: toggle cursor locking automatically while looking
    private void LateUpdate()
    {
        if (requireRightMouse)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
