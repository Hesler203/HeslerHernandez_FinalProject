using System;
using Unity.Mathematics;
using UnityEditor.MPE;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int FlippedHash = Animator.StringToHash("flipped");
    private static readonly int RolledHash = Animator.StringToHash("rolled");
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private InputManager inputManager;
    private GameManager gameManager;
    private Animator playerAnimator;
    private SpriteRenderer sprite;
    private Rigidbody rb;
    private float initialDamping;
    private Vector3 moveDirection;

    [Header("Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float depthSpeedMultiplier;
    [SerializeField] private float idleDampingMultiplier;
    [SerializeField] private float moveDeadZone;

    void Start()
    {
        gameManager = GameManager.Instance;
        inputManager = InputManager.Instance;
        playerAnimator = GetComponent<Animator>();
        moveDeadZone = inputManager.MoveDeadzone;
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();

        initialDamping = rb.linearDamping;
    }

    void LateUpdate()
    {
        FlipSpriteOnMove();
        moveDirection = new Vector3(inputManager.PlayerMoveInput.x, 0, inputManager.PlayerMoveInput.y).normalized;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRoll();
    }

    private void FlipSpriteOnMove()
    {
        if (inputManager.PlayerMoveInput.x > moveDeadZone && !playerAnimator.GetBool(RolledHash))
        {
            sprite.flipX = true;
            playerAnimator.SetBool(FlippedHash, true);
        }
        else if (inputManager.PlayerMoveInput.x < -moveDeadZone && !playerAnimator.GetBool(RolledHash))
        {
            sprite.flipX = false;
            playerAnimator.SetBool(FlippedHash, false);
        }
        else
        {
            return;
        }
    }

    private void HandleMovement()
    {
        if (Math.Abs(inputManager.PlayerMoveInput.sqrMagnitude) > moveDeadZone && !playerAnimator.GetBool(RolledHash))
        {
            Vector3 velocity = moveDirection * moveSpeed;

            if (Math.Abs(velocity.x) > 0 && Math.Abs(velocity.z) > 0)
            {
                velocity.x *= depthSpeedMultiplier * 2 / 3;
            }
            velocity.z *= depthSpeedMultiplier;

            if (!playerAnimator.GetBool(RolledHash))
            {
                rb.linearDamping = initialDamping;
            }
            rb.AddForce(velocity, ForceMode.Acceleration);

            playerAnimator.SetBool(IsMovingHash, true);
            return;
        }
        rb.linearDamping = initialDamping * idleDampingMultiplier;
        playerAnimator.SetBool(IsMovingHash, false);
    }

    private void HandleRoll(string rollFinished = "false")
    {
        if (inputManager.PlayerRollInput && !playerAnimator.GetBool(RolledHash))
        {
            playerAnimator.SetBool(RolledHash, true);
            return;
        }
        if (rollFinished == "true")
        {
            playerAnimator.SetBool(RolledHash, false);
        }
    }

    private void PerformRoll(Vector3 rollDirection)
    {
        rb.linearDamping = initialDamping * idleDampingMultiplier;

        Vector3 rollVelocity = rollDirection * rollSpeed;
        if (Math.Abs(rollVelocity.x) > 0 && Math.Abs(rollVelocity.z) > 0)
        {
            rollVelocity.x *= depthSpeedMultiplier * 2 / 3;
        }
        rollVelocity.z *= depthSpeedMultiplier;

        if (sprite.flipX == true)
        {
            rb.AddForce(rollVelocity, ForceMode.VelocityChange);
            return;
        }
        rb.AddForce(rollVelocity, ForceMode.VelocityChange);
    }

    private void InitiateRoll()
    {
        PerformRoll(moveDirection);
    }
}
