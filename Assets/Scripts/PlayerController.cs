using System;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private InputManager inputManager;
    private SpriteRenderer sprite;
    private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDeadZone;

    void Start()
    {
        inputManager = InputManager.Instance;
        moveDeadZone = inputManager.MoveDeadzone;
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        FlipSpriteOnMove();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void FlipSpriteOnMove()
    {
        if (inputManager.PlayerMoveInput.x > moveDeadZone)
        {
            sprite.flipX = true;
        }
        else if (inputManager.PlayerMoveInput.x < -moveDeadZone)
        {
            sprite.flipX = false;
        }
        else
        {
            return;
        }
    }

    private void HandleMovement()
    {
        if (Math.Abs(inputManager.PlayerMoveInput.sqrMagnitude) > moveDeadZone)
        {
            //Vector3 moveDirection = new Vector3(inputManager.PlayerMoveInput.x, 0, inputManager.PlayerMoveInput.y);
            Vector3 moveDirection = inputManager.PlayerMoveInput;
            Vector3 velocity = moveDirection.normalized * moveSpeed;

            rb.AddForce(velocity, ForceMode.Force);
        }
    }
}
