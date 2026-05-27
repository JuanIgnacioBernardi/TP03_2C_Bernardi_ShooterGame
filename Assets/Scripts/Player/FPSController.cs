using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Movement Smoothing")]
    [Tooltip("How quickly the character reaches the target speed. Higher = more responsive.")]
    [SerializeField] private float acceleration = 10f;

    [Header("Jump and Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    [Header("Crouch")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;
    [Tooltip("Speed at which the CharacterController interpolates its height when crouching.")]
    [SerializeField] private float crouchTransitionSpeed = 8f;

    [Header("Camera pivot reference")]
    [Tooltip("Child GameObject at eye height. Its Y adjusts when crouching.")]
    [SerializeField] private Transform cameraHolder;

    private float standingCamHeight;
    private float crouchingCamHeight;

    [Header("Ground detection")]
    [SerializeField] private float groundCheckRadius = 0.28f;
    [SerializeField] private float groundCheckOffset = 0.05f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        standingHeight = controller.height;
        standingCamHeight = cameraHolder != null ? cameraHolder.localPosition.y : 0.75f;

        float heightDiff = standingHeight - crouchingHeight;
        crouchingCamHeight = standingCamHeight - heightDiff;
    }
    private void Update()
    {
        CheckGround();
        HandleCrouch();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }
    private void CheckGround()
    {
        Vector3 spherePos = transform.position + Vector3.up * groundCheckOffset;
        isGrounded = Physics.CheckSphere(spherePos, groundCheckRadius, groundMask);

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
    }
    private void HandleCrouch()
    {
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame && isGrounded)
        {
            if (isCrouching && !CanStandUp())
                return;

            isCrouching = !isCrouching;
        }

        float targetControllerHeight = isCrouching ? crouchingHeight : standingHeight;
        float targetCamHeight = isCrouching ? crouchingCamHeight : standingCamHeight;

        controller.height = Mathf.Lerp(controller.height, targetControllerHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        if (cameraHolder != null)
        {
            Vector3 camPos = cameraHolder.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCamHeight, Time.deltaTime * crouchTransitionSpeed);
            cameraHolder.localPosition = camPos;
        }
    }
    private bool CanStandUp()
    {
        float margin = 0.1f;
        float castDistance = standingHeight - crouchingHeight - margin;

        return !Physics.SphereCast(
            transform.position + Vector3.up * (crouchingHeight - groundCheckRadius),
            groundCheckRadius,
            Vector3.up,
            out _,
            castDistance,
            groundMask
        );
    }
    private void HandleMovement()
    {
        Keyboard kb = Keyboard.current;

        float inputX = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                     - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);
        float inputZ = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f)
                     - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);

        Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;

        isRunning = kb.leftShiftKey.isPressed && !isCrouching && isGrounded;
        float targetSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        Vector3 targetVelocity = transform.TransformDirection(inputDir) * targetSpeed;

        currentMoveVelocity = Vector3.Lerp(currentMoveVelocity, targetVelocity, Time.deltaTime * acceleration);
        controller.Move(currentMoveVelocity * Time.deltaTime);
    }
    private void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !isCrouching)
            velocity.y = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
    }
    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);
    }
    public bool IsGrounded => isGrounded;
    public bool IsCrouching => isCrouching;
    public bool IsRunning => isRunning;
    public bool IsMoving => currentMoveVelocity.sqrMagnitude > 0.01f;
    public float CurrentSpeed => currentMoveVelocity.magnitude;
}