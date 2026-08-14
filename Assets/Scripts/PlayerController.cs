using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public static readonly int IsCaughtHash = Animator.StringToHash("isCaught");
    public static readonly int FlippedHash = Animator.StringToHash("flipped");
    public static readonly int RolledHash = Animator.StringToHash("rolled");
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
    [SerializeField] private float depthRollMultiplier;
    [SerializeField] private float idleDamping;
    [SerializeField] private float moveDamping;
    [SerializeField] private float rollDamping;
    private Vector3 moveDirection;
    private float moveDeadZone;

    [Header("Player State")]
    public PlayerState currentState;
    public enum PlayerState { idle, moving, rolling, caught } // TODO - carrying, pricked, kelpy

    void Start()
    {
        gameManager = GameManager.Instance;
        inputManager = InputManager.Instance;
        playerAnimator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();

        moveDeadZone = inputManager.MoveDeadzone;
        currentState = PlayerState.idle;
    }

    void Update()
    {
        UpdatePlayerState();

        AlignSkyPointWithPlayer();
    }

    private void UpdatePlayerState()
    {
        if (playerAnimator.GetBool(IsMovingHash))
        {
            currentState = PlayerState.moving;
        }
        else if (playerAnimator.GetBool(RolledHash))
        {
            currentState = PlayerState.rolling;
        }
        else if (playerAnimator.GetBool(IsCaughtHash))
        {
            currentState = PlayerState.caught;

            moveDirection *= 0;
            rb.linearVelocity = Vector3.zero;
        }// TODO other player states
        else
        {
            currentState = PlayerState.idle;
        }
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
        if (currentState != PlayerState.rolling && currentState != PlayerState.caught)
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
        if (currentState != PlayerState.rolling && currentState != PlayerState.caught)
        {
            if (Math.Abs(inputManager.PlayerMoveInput.sqrMagnitude) > moveDeadZone)
            {
                moveDirection = new Vector3(inputManager.PlayerMoveInput.x, 0, inputManager.PlayerMoveInput.y).normalized;
                Vector3 velocity = moveDirection * moveSpeed;

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
        if (currentState != PlayerState.rolling || currentState != PlayerState.caught)
        {
            if (inputManager.PlayerRollInput)
            {
                playerAnimator.SetBool(RolledHash, true);
                playerAnimator.SetBool(IsMovingHash, false);
                return;
            }
        }

        if (rollFinished == "true")
        {
            playerAnimator.SetBool(RolledHash, false);
            rb.linearDamping = rollDamping;
        }
    }

    private void InitiateRoll()
    {
        PerformRoll(moveDirection);
    }

    private void PerformRoll(Vector3 rollDirection)
    {
        Vector3 rollVelocity = rollDirection * rollSpeed;

        rollVelocity.z *= depthRollMultiplier;
        if (Math.Abs(rollVelocity.x) > 0 && Math.Abs(rollVelocity.z) > 0)
        {
            rollVelocity.x++;
        }

        rb.linearDamping = 0;
        rb.AddForce(rollVelocity, ForceMode.VelocityChange);
    }
}
