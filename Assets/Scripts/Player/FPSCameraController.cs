using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCameraController : MonoBehaviour
{
    [Header("Mouse sensitivity")]
    [Range(0.01f, 1f)]
    [SerializeField] private float sensitivityX = 0.15f;

    [Range(0.01f, 1f)]
    [SerializeField] private float sensitivityY = 0.15f;

    [Header("Camera smoothing")]
    [Tooltip("Interpolation of camera movement. 0 = no smoothing, higher = smoother but with lag.")]
    [Range(0f, 1f)]
    [SerializeField] private float smoothing = 0f;

    [Header("Vertical rotation limits")]
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    [Header("Player reference (horizontal rotation)")]
    [Tooltip("The Transform of the root Player. The Y rotation of the mouse is applied here.")]
    [SerializeField] private Transform playerBody;

    private float currentPitch;
    private float targetPitch;
    private float currentYaw;
    private float targetYaw;
    private bool cursorLocked = true;
    private void Start()
    {
        LockCursor(true);

        if (playerBody != null)
            currentYaw = targetYaw = playerBody.eulerAngles.y;

        currentPitch = targetPitch = transform.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;
    }
    private void Update()
    {
        HandleCursorLock();
        if (!cursorLocked) return;

        ReadMouseInput();
        ApplyRotation();
    }
    private void ReadMouseInput()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();

        targetYaw += delta.x * sensitivityX;
        targetPitch -= delta.y * sensitivityY;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
    }
    private void ApplyRotation()
    {
        if (smoothing > 0f)
        {
            float t = 1f - Mathf.Pow(smoothing, Time.deltaTime * 60f);
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, t);
            currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, t);
        }
        else
        {
            currentYaw = targetYaw;
            currentPitch = targetPitch;
        }

        if (playerBody != null)
            playerBody.rotation = Quaternion.Euler(0f, currentYaw, 0f);

        transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }
    private void HandleCursorLock()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            LockCursor(false);

        if (Mouse.current.leftButton.wasPressedThisFrame && !cursorLocked)
            LockCursor(true);
    }
    public void LockCursor(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
    public float CurrentPitch => currentPitch;
    public bool IsCursorLocked => cursorLocked;

    public void AddRecoil(float pitchDelta, float yawDelta = 0f)
    {
        targetPitch += pitchDelta;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        targetYaw += yawDelta;
    }
    public void SetSensitivity(float x, float y)
    {
        sensitivityX = x;
        sensitivityY = y;
    }
}