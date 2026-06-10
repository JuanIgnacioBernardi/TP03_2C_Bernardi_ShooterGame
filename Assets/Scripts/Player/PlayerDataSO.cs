using UnityEngine;
[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;

    [Header("Movement Smoothing")]
    public float acceleration = 10f;

    [Header("Jump and Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float crouchTransitionSpeed = 8f;

    [Header("Ground Detection")]
    public float groundCheckRadius = 0.28f;
    public float groundCheckOffset = 0.05f;
}