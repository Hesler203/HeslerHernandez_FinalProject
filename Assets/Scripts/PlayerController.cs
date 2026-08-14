using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public static readonly int FlippedHash = Animator.StringToHash("flipped");
    public static readonly int IsMovingHash = Animator.StringToHash("isMoving");

    private GameManager gameManager; // TODO
    private InputManager inputManager;
    private Animator playerAnimator;
    private SpriteRenderer sprite;
    private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private Transform playerSkyPoint;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float depthMoveMultiplier;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float idleDamping;
    [SerializeField] private float moveDamping;
    private Vector3 moveDirection;
    private float moveDeadZone;

    void Start()
    {
        gameManager = GameManager.Instance;
        inputManager = InputManager.Instance;
        playerAnimator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();

        moveDeadZone = inputManager.MoveDeadzone;
    }

    void Update()
    {
        AlignSkyPointWithPlayer();
    }

    private void AlignSkyPointWithPlayer()
    {
        playerSkyPoint.position = new Vector3(transform.position.x, playerSkyPoint.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        FlipSpriteOnMove();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRoll();
    }

    private void FlipSpriteOnMove()
    {
        {
            if (inputManager.PlayerMoveInput.x > moveDeadZone)
            {
                sprite.flipX = true;
                playerAnimator.SetBool(FlippedHash, true);
            }
            else if (inputManager.PlayerMoveInput.x < -moveDeadZone)
            {
                sprite.flipX = false;
                playerAnimator.SetBool(FlippedHash, false);
            }
            else
            {
                return;
            }
        }
    }

    private void HandleMovement()
    {
        if (Math.Abs(inputManager.PlayerMoveInput.sqrMagnitude) > moveDeadZone && !playerAnimator.GetBool(RolledHash))
        {
            if (Math.Abs(inputManager.PlayerMoveInput.sqrMagnitude) > moveDeadZone)
            {
                velocity.x *= depthSpeedMultiplier * 2 / 3;
            }
            velocity.z *= depthSpeedMultiplier;
                moveDirection = new Vector3(inputManager.PlayerMoveInput.x, 0, inputManager.PlayerMoveInput.y).normalized;
                Vector3 velocity = moveDirection * moveSpeed;

            if (!playerAnimator.GetBool(RolledHash))
            {
                rb.linearDamping = initialDamping;
            }
                velocity.z *= depthMoveMultiplier;
                if (Math.Abs(velocity.x) > 0 && Math.Abs(velocity.z) > 0)
                {
                    velocity.x++;
                }

                rb.linearDamping = moveDamping;

                rb.AddForce(velocity, ForceMode.Acceleration);

                playerAnimator.SetBool(IsMovingHash, true);
                return;
            }
            rb.linearDamping = idleDamping;
            moveDirection.y = 0;
            moveDirection.z = 0;
            playerAnimator.SetBool(IsMovingHash, false);
        }
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
