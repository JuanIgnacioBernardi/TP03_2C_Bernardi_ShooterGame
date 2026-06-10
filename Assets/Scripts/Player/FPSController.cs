using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerDataSO data;

    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
    private float standingCamHeight;
    private float crouchingCamHeight;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.height = data.standingHeight;
        standingCamHeight = cameraHolder != null ? cameraHolder.localPosition.y : 0.75f;
        float heightDiff = data.standingHeight - data.crouchingHeight;
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
        Vector3 spherePos = transform.position + Vector3.up * data.groundCheckOffset;
        isGrounded = Physics.CheckSphere(spherePos, data.groundCheckRadius, groundMask);
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
    }
    private void HandleCrouch()
    {
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame && isGrounded)
        {
            if (isCrouching && !CanStandUp()) return;
            isCrouching = !isCrouching;
        }

        float targetControllerHeight = isCrouching ? data.crouchingHeight : data.standingHeight;
        float targetCamHeight = isCrouching ? crouchingCamHeight : standingCamHeight;

        controller.height = Mathf.Lerp(controller.height, targetControllerHeight,
                                        Time.deltaTime * data.crouchTransitionSpeed);
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        if (cameraHolder != null)
        {
            Vector3 camPos = cameraHolder.localPosition;
            camPos.y = Mathf.MoveTowards(camPos.y, targetCamHeight, Time.deltaTime * data.crouchTransitionSpeed);
            cameraHolder.localPosition = camPos;
        }
    }
    private bool CanStandUp()
    {
        float margin = 0.1f;
        float castDistance = data.standingHeight - data.crouchingHeight - margin;
        return !Physics.SphereCast(transform.position + Vector3.up * (data.crouchingHeight - data.groundCheckRadius), data.groundCheckRadius, Vector3.up, out _, castDistance, groundMask);
    }
    private void HandleMovement()
    {
        Keyboard kb = Keyboard.current;
        float inputX = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);
        float inputZ = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);

        Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;
        isRunning = kb.leftShiftKey.isPressed && !isCrouching && isGrounded;
        float targetSpeed = isCrouching ? data.crouchSpeed : (isRunning ? data.runSpeed : data.walkSpeed);

        Vector3 targetVelocity = transform.TransformDirection(inputDir) * targetSpeed;
        currentMoveVelocity = Vector3.Lerp(currentMoveVelocity, targetVelocity, Time.deltaTime * data.acceleration);
        controller.Move(currentMoveVelocity * Time.deltaTime);
    }
    private void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !isCrouching)
            velocity.y = Mathf.Sqrt(2f * Mathf.Abs(data.gravity) * data.jumpHeight);
    }
    private void ApplyGravity()
    {
        velocity.y += data.gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);
    }
    public bool IsGrounded => isGrounded;
    public bool IsCrouching => isCrouching;
    public bool IsRunning => isRunning;
    public bool IsMoving => currentMoveVelocity.sqrMagnitude > 0.01f;
    public float CurrentSpeed => currentMoveVelocity.magnitude;
}