using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerObject;
    public Rigidbody2D rb;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundLayer;
    float enabledGroundBuffer = 0f;
    float disabledGroundBuffer = 0f;

    public float gravity;
    public float maxFallSpeed;
    public float jumpPower;
    public float horizontalMaxSpeed;
    public float horizontalAcceleration;
    public float groundBufferDuration;

    bool canInterruptJump;
    bool pressedJump;
    bool holdingJump;


    PlayerMovementDirection playerMovementDirection;
    void Start()
    {
        rb.gravityScale = 0;
    }

    void Update()
    {
        CheckInputs();
    }

    void CheckInputs()
    {
        pressedJump = pressedJump || GameInputManager.GetManagerKeyDown("Jump");
        playerMovementDirection = PlayerMovementDirection.None;
        if (GameInputManager.GetManagerKey("Right"))
        {
            playerMovementDirection = PlayerMovementDirection.Right;
        }
        if (GameInputManager.GetManagerKey("Left"))
        {
            playerMovementDirection = PlayerMovementDirection.Left;
        }
        if (GameInputManager.GetManagerKey("Left") && GameInputManager.GetManagerKey("Right"))
        {
            playerMovementDirection = PlayerMovementDirection.None;
        }
        if (GameInputManager.GetManagerKey("Jump"))
        {
            holdingJump = true;
        }
        else
        {
            holdingJump = false;
        }
    }
    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        if (IsGrounded(velocity))
        {
            canInterruptJump = true;
        }
        velocity = CheckMovement(velocity);
        velocity = CheckGravity(velocity);

        rb.velocity = velocity;
        pressedJump = false;
        enabledGroundBuffer -= Time.fixedDeltaTime;
        if (enabledGroundBuffer < 0) { enabledGroundBuffer = 0; }
        disabledGroundBuffer -= Time.fixedDeltaTime;
        if (disabledGroundBuffer < 0) { disabledGroundBuffer = 0; }
    }



    Vector2 CheckMovement(Vector2 velocity)
    {
        if (pressedJump && IsGrounded(velocity))
        {
            pressedJump = false;
            enabledGroundBuffer = 0;
            disabledGroundBuffer = groundBufferDuration;
            velocity.y = jumpPower;
        }
        switch (playerMovementDirection)
        {
            case PlayerMovementDirection.Right:
                velocity.x += horizontalAcceleration * Time.fixedDeltaTime;
                if (velocity.x < 0)
                {
                    velocity.x = 0;
                }
                break;
            case PlayerMovementDirection.Left:
                if (velocity.x > 0)
                {
                    velocity.x = 0;
                }
                velocity.x -= horizontalAcceleration * Time.fixedDeltaTime;
                break;
            case PlayerMovementDirection.None:
                velocity.x = 0;
                break;
        }
        if (Mathf.Abs(velocity.x) > horizontalMaxSpeed)
        {
            velocity.x = horizontalMaxSpeed * velocity.x / Mathf.Abs(velocity.x);
        }

        if (holdingJump == false && velocity.y > 0 && canInterruptJump)
        {
            velocity.y = 0;
        }
        return velocity;
    }

    Vector2 CheckGravity(Vector2 velocity)
    {
        velocity.y += -1 * gravity * Time.fixedDeltaTime;
        if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
        }
        return velocity;
    }

    bool IsGrounded(Vector2 velocity)
    {
        bool grounded = velocity.y == 0;
        if (grounded)
        {
            enabledGroundBuffer = groundBufferDuration;
        }
        return (grounded || (enabledGroundBuffer > 0 && disabledGroundBuffer <= 0));
    }

    void ResetPlayerMovementDirection()
    {
        playerMovementDirection = PlayerMovementDirection.None;
    }
}

enum PlayerMovementDirection
{
    None,
    Right,
    Left,
}
