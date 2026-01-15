using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerObject;
    public Rigidbody2D rb;

    public float gravity;
    public float maxFallSpeed;
    public float jumpPower;
    public float horizontalSpeed;

    PlayerMovementDirection playerMovementDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb.gravityScale = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;

        velocity = CheckMovementInputs(velocity);
        velocity.y += -1 * gravity * Time.fixedDeltaTime;
        if (velocity.y > maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }

        rb.velocity = velocity;
    }

    public Vector2 CheckMovementInputs(Vector2 velocity)
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
        if (GameInputManager.GetManagerKey("Jump"))
        {
            if (IsGrounded(velocity))
            {
                velocity.y += jumpPower;
            }
        }
        else if (velocity.y > 0)
        {
            velocity.y = 0;
        }
        
        return velocity;
    }

    bool IsGrounded(Vector2 velocity)
    {
        if (velocity.y == 0)
        {
            return true;
        }
        return false;
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
