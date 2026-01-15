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

    public float gravity;
    public float maxFallSpeed;
    public float jumpPower;
    public float horizontalSpeed;

    bool canInterruptJump;
    bool pressedJump;

    PlayerMovementDirection playerMovementDirection;
    void Start()
    {
        rb.gravityScale = 0;
    }

    void Update()
    {
        pressedJump = pressedJump || GameInputManager.GetManagerKeyDown("Jump");

    }
    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        if (IsGrounded(velocity))
        {
            canInterruptJump = true;
        }
        velocity = CheckMovementInputs(velocity);
        velocity = CheckGravity(velocity);

        rb.velocity = velocity;
        pressedJump = false;
    }

    Vector2 CheckMovementInputs(Vector2 velocity)
    {
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
        switch (playerMovementDirection)
        {
            case PlayerMovementDirection.Right:
                velocity.x = horizontalSpeed;
                break;
            case PlayerMovementDirection.Left:
                velocity.x = -horizontalSpeed;
                break;
            case PlayerMovementDirection.None:
                velocity.x = 0;
                break;
        }
        if (pressedJump && IsGrounded(velocity))
        {
            pressedJump = false;
            velocity.y += jumpPower;
        }
        if (GameInputManager.GetManagerKey("Jump") == false && velocity.y > 0 && canInterruptJump)
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
        return Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundDistance,
            groundLayer
        );
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
