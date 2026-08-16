using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private readonly int isMoving = AnimatorManager.IsMovingHash;
    private readonly int isRolling = AnimatorManager.IsRollingHash;
    private readonly int isFlipped = AnimatorManager.IsFlippedHash;
    private readonly int isCaught = AnimatorManager.IsCaughtHash;
    private readonly int isFalling = AnimatorManager.IsFallingHash;

    private GameManager gameManager; // TODO
    private InputManager inputManager;
    private Animator playerAnimator;
    private SpriteRenderer sprite;
    private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Transform playerSkyPoint;
    [field: SerializeField] public float CaptureTime { get; private set; }
    private Transform seagullBeakTransform;

    [Header("Movement")]
    [SerializeField] private float idleDamping;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDamping;
    [SerializeField] private float depthMoveMultiplier;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollDamping;
    [SerializeField] private float depthRollMultiplier;
    private Vector3 moveDirection;
    private float moveDeadZone;

    [Header("Player State")]
    [field: SerializeField] public static PlayerState CurrentState { get; private set; }
    public enum PlayerState { idle, moving, rolling, caught, falling } // TODO - carrying, pricked, kelpy

    void Start()
    {
        gameManager = GameManager.Instance;
        inputManager = InputManager.Instance;
        playerAnimator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();

        moveDeadZone = inputManager.MoveDeadzone;
        CurrentState = PlayerState.idle;
    }

    void Update()
    {
        UpdatePlayerState();

        AlignSkyPointWithPlayer();
    }

    private void UpdatePlayerState()
    {
        if (playerAnimator.GetBool(isMoving))
        {
            CurrentState = PlayerState.moving;
        }
        else if (playerAnimator.GetBool(isRolling))
        {
            CurrentState = PlayerState.rolling;
        }
        else if (playerAnimator.GetBool(isCaught))
        {
            CurrentState = PlayerState.caught;

        }
        else if (playerAnimator.GetBool(isFalling))
        {
            CurrentState = PlayerState.falling;

        }// TODO other player states
        else
        {
            CurrentState = PlayerState.idle;
        }
    }

    private void AlignSkyPointWithPlayer()
    {
        playerSkyPoint.position = new Vector3(transform.position.x, playerSkyPoint.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        FlipSpriteOnMove();

        HandleCaught();
        HandleFalling();
    }

    private void FlipSpriteOnMove()
    {
        if (CurrentState != PlayerState.rolling && CurrentState != PlayerState.caught)
        {
            if (inputManager.PlayerMoveInput.x > moveDeadZone)
            {
                sprite.flipX = true;
                playerAnimator.SetBool(isFlipped, true);
            }
            else if (inputManager.PlayerMoveInput.x < -moveDeadZone)
            {
                sprite.flipX = false;
                playerAnimator.SetBool(isFlipped, false);
            }
            else
            {
                return;
            }
        }
    }

    public void TriggerCapture(Transform seagullBeak)
    {
        seagullBeakTransform = seagullBeak;
        playerAnimator.SetBool(isCaught, true);
        StartCoroutine(nameof(PlayerCaptureTimer));
    }

    private void HandleCaught()
    {
        if (seagullBeakTransform && CurrentState == PlayerState.caught)
        {
            transform.position = seagullBeakTransform.position;
            moveDirection *= 0;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void HandleFalling()
    {
        if (CurrentState == PlayerState.falling)
        {
            moveDirection *= 0;
        }
    }

    IEnumerator PlayerCaptureTimer()
    {
        yield return new WaitForSeconds(CaptureTime);
        playerAnimator.SetBool(isCaught, false);
        seagullBeakTransform = null;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRoll();
    }

    private void HandleMovement()
    {
        if (CurrentState != PlayerState.rolling && CurrentState != PlayerState.caught)
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

                playerAnimator.SetBool(isMoving, true);
                return;
            }
            rb.linearDamping = idleDamping;
            moveDirection.y = 0;
            moveDirection.z = 0;
            playerAnimator.SetBool(isMoving, false);
        }
    }

    private void HandleRoll(string rollFinished = "false")
    {
        if (CurrentState != PlayerState.rolling || CurrentState != PlayerState.caught)
        {
            if (inputManager.PlayerRollInput)
            {
                playerAnimator.SetBool(isRolling, true);
                playerAnimator.SetBool(isMoving, false);
                return;
            }
        }

        if (rollFinished == "true")
        {
            playerAnimator.SetBool(isRolling, false);
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

    private void PlaySound(string soundName)
    {
        gameManager.AudioManager.PlaySound(soundName);
    }

    private void StopSound()
    {
        gameManager.AudioManager.StopSound(AudioManager.SFXType.player);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && CurrentState == PlayerState.falling)
        {
            playerAnimator.SetBool(isFalling, false);
            return;
        }
    }
}