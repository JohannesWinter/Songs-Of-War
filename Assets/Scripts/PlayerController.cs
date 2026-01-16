using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    public GameObject playerObject;
    public Transform playerRoot;
    public Transform playerTop;
    public BoxCollider2D playerCollider;
    public Rigidbody2D rb;
    public Vector2 velocity;
    Vector2 positionLastFrame;

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
    public float stepHeight;
    public float wallJumpSlipTime;
    public float wallJumpBufferDuration;
    public float playerMinGripHeight;
    public bool canWallJump;

    bool canInterruptJump;
    bool canHoldOnWall;
    float canHoldOnWallTimer;
    bool pressedJump;
    bool holdingJump;
    public bool holding { get; private set; }
    public bool slipping { get; private set; }
    float holdJumpDirection;


    public PlayerMovementDirection playerMovementDirection { get; private set; }
    void Start()
    {
        rb.gravityScale = 0;
    }

    void Update()
    {
        CheckInputs();
        UpdateDebugVisuals();
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
    void UpdateDebugVisuals()
    {
        //displays various player-states for debugging
        if (holding == true)
        {
            playerObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (slipping == true)
        {
            playerObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        if (slipping == true && holding == true)
        {
            playerObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        if (slipping == false && holding == false)
        {
            playerObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    void FixedUpdate()
    {
        CheckParameterAccessibility();
        TryStepUp(velocity);
        TryHold(velocity);
        velocity = CheckMovement(velocity);
        velocity = CheckGravity(velocity);
        if (holding) velocity = Vector2.zero;
        rb.velocity = velocity;
        EndParameterAccessibilty();
    }
    void CheckParameterAccessibility()
    {
        //resets on-ground parameters
        if (IsGrounded(velocity))
        {
            canInterruptJump = true;
            canHoldOnWall = true;
            holding = false;
            slipping = false;
            velocity.y = -5;
        }
        //sets walljump/hold/slip parameters
        if (canHoldOnWall == false && holding == false && slipping == false)
        {
            canHoldOnWallTimer += Time.fixedDeltaTime;
            if (canHoldOnWallTimer > 0.5f)
            {
                canHoldOnWall = true;
            }
        }
        else
        {
            canHoldOnWallTimer = 0;
        }
        //checks head-hitters
        if (IsTopFree(playerObject.transform.position) == false)
        {
            velocity.y = 0;
        }
        //updates on-ground buffer for jumping
        enabledGroundBuffer -= Time.fixedDeltaTime;
        if (enabledGroundBuffer < 0) { enabledGroundBuffer = 0; }
        disabledGroundBuffer -= Time.fixedDeltaTime;
        if (disabledGroundBuffer < 0) { disabledGroundBuffer = 0; }
    }

    void EndParameterAccessibilty()
    {
        //updates Past-Velocity-Update parameters
        pressedJump = false;
        positionLastFrame = playerObject.transform.position;
    }

    Vector2 CheckMovement(Vector2 velocity)
    {
        //checks jump movement
        if (pressedJump && (IsGrounded(velocity) || holding || slipping))
        {
            pressedJump = false;
            enabledGroundBuffer = 0;
            disabledGroundBuffer = groundBufferDuration;
            velocity.y = jumpPower;
            canHoldOnWall = true;
            if (holding || slipping)
            {
                holding = false;
                slipping = false;
                velocity.x = horizontalMaxSpeed * holdJumpDirection;
            }
        }

        //checks Jump interrupt
        if (holdingJump == false && velocity.y > 0 && canInterruptJump)
        {
            velocity.y = 0;
        }

        //checks walk movement
        if (disabledGroundBuffer <= 0)
        {
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
            if (velocity.x > horizontalMaxSpeed) velocity.x = horizontalMaxSpeed;
            if (velocity.x < -horizontalMaxSpeed) velocity.x = -horizontalMaxSpeed;
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
    void TryStepUp(Vector2 currentVelocity)
    {
        //check stepup logic
        if (IsGrounded(currentVelocity) == false) { return; }

        Vector2 origin = (Vector2)playerRoot.transform.position + Vector2.up * 0.05f;

        Vector2 direction = Vector2.zero;
        switch (playerMovementDirection)
        {
            case PlayerMovementDirection.Right:
                direction = Vector2.right;
                break;
            case PlayerMovementDirection.Left:
                direction = Vector2.left;
                break;
            case PlayerMovementDirection.None:
                return;
        }
        //check if player can stepup
        RaycastHit2D lowerHit = Physics2D.Raycast(
            origin,
            direction,
            playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.05f,
            groundLayer
        );

        if (!lowerHit) return;

        RaycastHit2D upperHit = Physics2D.Raycast(
            origin + Vector2.up * stepHeight,
            direction,
            playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.05f,
            groundLayer
        );
        float currentStepHeight = stepHeight;
        int counter = 5;
        while (counter > 0)
        {
            RaycastHit2D newHit = Physics2D.Raycast(
                origin + Vector2.up * (currentStepHeight - stepHeight / 5),
                direction,
                playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.05f,
                groundLayer
            );
            if (!newHit)
            {
                upperHit = newHit;
                currentStepHeight -= stepHeight / 5;
            }
            else
            {
                break;
            }
            counter--;
        }
        //player steps up
        if (!upperHit)
        {
            playerObject.transform.position += Vector3.up * (currentStepHeight + stepHeight / 5) + (Vector3)direction * 0.1f;
        }
    }
    void TryHold(Vector2 currentVelocity)
    {
        //check hold logic
        if (IsGrounded(currentVelocity) == true || disabledGroundBuffer > 0 || canHoldOnWall == false || slipping == true || holding == true) return;

        Vector2 origin = (Vector2)playerRoot.transform.position + Vector2.up * 0.05f;

        Vector2 direction = Vector2.zero;
        switch (playerMovementDirection)
        {
            case PlayerMovementDirection.Right:
                direction = Vector2.right;
                break;
            case PlayerMovementDirection.Left:
                direction = Vector2.left;
                break;
            case PlayerMovementDirection.None:
                return;
        }
        if (TryHoldLedge(origin, direction)) return;
        else TryHoldWall(origin, direction);
    }

    bool TryHoldLedge(Vector2 origin, Vector2 direction)
    {
        //check if player can hold
        bool foundHit = Physics2D.Raycast(
            origin,
            direction,
            playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.05f,
            groundLayer
        );
        float currentHeight = 0;
        float maxHeight = playerObject.transform.localScale.y * 1f - 0.05f;

        while (foundHit == false && currentHeight < maxHeight)
        {
            currentHeight += playerObject.transform.localScale.y / 100;
            foundHit = Physics2D.Raycast(
                origin + Vector2.up * currentHeight,
                direction,
                playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.05f,
                groundLayer
            );
        }

        if (foundHit == false) return false;

        while (foundHit == true && currentHeight < maxHeight)
        {
            currentHeight += playerObject.transform.localScale.y / 100;
            foundHit = Physics2D.Raycast(
                origin + Vector2.up * currentHeight,
                direction,
                playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.5f,
                groundLayer
            );
        }
        float targetHeight = (origin + Vector2.up * currentHeight).y - playerObject.transform.localScale.y / 2;
        Vector2 targetPosition = new Vector2(playerObject.transform.position.x, targetHeight);

        if (IsBotFree(targetPosition) == false) return false;

        if (IsTopFree(playerObject.transform.position) == false) return false;

        //check if player holds on edge or on straight wall
        if (foundHit == false)
        {
            StartCoroutine(Hold(playerObject, targetPosition, playerMovementDirection, float.MaxValue, wallJumpBufferDuration));
            return true;
        }
        return false;
    }

    bool TryHoldWall(Vector2 origin, Vector2 direction)
    {
        if (canWallJump == false) return false;

        //check if player can hold
        bool foundHit = Physics2D.Raycast(
            origin,
            direction,
            playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.05f,
            groundLayer
        );
        float currentHeight = 0;
        float maxHeight = playerObject.transform.localScale.y * 1f - 0.05f;

        while (foundHit == false && currentHeight < maxHeight)
        {
            currentHeight += playerObject.transform.localScale.y / 100;
            foundHit = Physics2D.Raycast(
                origin + Vector2.up * currentHeight,
                direction,
                playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.05f,
                groundLayer
            );
            if (currentHeight > playerObject.transform.localScale.y * (1 - playerMinGripHeight))
            {
                return false;
            }
        }

        if (foundHit == false) return false;

        while (foundHit == true && currentHeight < maxHeight)
        {
            currentHeight += playerObject.transform.localScale.y / 100;
            foundHit = Physics2D.Raycast(
                origin + Vector2.up * currentHeight,
                direction,
                playerCollider.size.x * playerObject.transform.localScale.x / 2 + 0.5f,
                groundLayer
            );
        }
        float targetHeight = (origin + Vector2.up * currentHeight).y - playerObject.transform.localScale.y / 2;
        Vector2 targetPosition = new Vector2(playerObject.transform.position.x, targetHeight);

        if (IsBotFree(targetPosition) == false) return false;

        if (IsTopFree(playerObject.transform.position) == false) return false;

        //check if player holds on edge or on straight wall
        if (foundHit == false)
        {
            return false;
        }

        StartCoroutine(Hold(playerObject, playerObject.transform.position, playerMovementDirection, wallJumpSlipTime, wallJumpBufferDuration));
        return true;
    }

    bool IsGrounded(Vector2 velocity)
    {
        RaycastHit2D downCheck = Physics2D.Raycast(
            (Vector2)playerObject.transform.position,
            Vector2.down,
            playerObject.transform.localScale.y / 2 + 0.05f,
            groundLayer
        );
        RaycastHit2D downCheckRight = Physics2D.Raycast(
            (Vector2)playerObject.transform.position + Vector2.right * playerObject.transform.localScale.x / 2 * 0.95f,
            Vector2.down,
            playerObject.transform.localScale.y / 2 + 0.05f,
            groundLayer
        );
        RaycastHit2D downCheckLeft = Physics2D.Raycast(
            (Vector2)playerObject.transform.position + Vector2.left * playerObject.transform.localScale.x / 2 * 0.95f,
            Vector2.down,
            playerObject.transform.localScale.y / 2 + 0.05f,
            groundLayer
        );

        bool grounded = (downCheck || downCheckLeft || downCheckRight);

        //sets a decreasing ground-Buffer for jumping
        if (grounded)
        {
            enabledGroundBuffer = groundBufferDuration;
        }
        return (grounded || (enabledGroundBuffer > 0 && disabledGroundBuffer <= 0));
    }
    bool IsTopFree(Vector2 targetPosition)
    {
        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                targetPosition + Vector2.right * (-playerObject.transform.localScale.x / 2 + (playerObject.transform.localScale.x * i / 10)),
                Vector2.up,
                playerObject.transform.localScale.y / 2 + 0.1f,
                groundLayer
                );
            if (topHit == true)
            {
                return false;
            }
        }
        return true;
    }
    bool IsBotFree(Vector2 targetPosition)
    {
        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                targetPosition + Vector2.right * (-playerObject.transform.localScale.x / 2 + (playerObject.transform.localScale.x * i / 10)),
                Vector2.down,
                playerObject.transform.localScale.y / 2 + 0.05f,
                groundLayer
                );
            if (topHit == true)
            {
                return false;
            }
        }
        return true;
    }

    bool IsTouchingWall(PlayerMovementDirection pMD)
    {
        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                (Vector2)playerObject.transform.position + Vector2.up * (-playerObject.transform.localScale.y / 2 + (playerObject.transform.localScale.y * i / 10)),
                GetPlayerMovementDirectionVector(pMD),
                playerObject.transform.localScale.x / 2 + 0.1f,
                groundLayer
                );
            if (topHit == true)
            {
                return true;
            }
        }
        return false;
    }

    Vector2 GetPlayerMovementDirectionVector(PlayerMovementDirection pMD)
    {
        switch (pMD)
        {
            case PlayerMovementDirection.Right:
                return Vector2.right;
            case PlayerMovementDirection.Left:
                return Vector2.left;
            case PlayerMovementDirection.None:
                return Vector2.zero;
        }
        return Vector2.zero;
    }

    void ResetPlayerMovementDirection()
    {
        playerMovementDirection = PlayerMovementDirection.None;
    }

    IEnumerator Hold(GameObject playerObject, Vector2 targetPosition, PlayerMovementDirection pMD, float slipTime, float wallJumpBufferDuration)
    {
        holding = true;
        canInterruptJump = true;
        canHoldOnWall = false;
        playerObject.transform.position = targetPosition;
        holdJumpDirection = pMD == PlayerMovementDirection.Right ? -1 : 1; //inverted to jump away from wall

        if (pMD == PlayerMovementDirection.Right)
        {
            while ((GameInputManager.GetManagerKey("Right") || wallJumpBufferDuration > 0) && (holding || slipping))
            {
                if (GameInputManager.GetManagerKey("Right") == false)
                {
                    wallJumpBufferDuration -= Time.deltaTime;
                }
                if (holding == true)
                {
                    playerObject.transform.position = targetPosition;
                }
                slipTime -= Time.deltaTime;
                if (slipTime < 0)
                {
                    holding = false;
                    canHoldOnWall = false;
                    slipping = true;
                    slipTime = float.MaxValue;
                }
                if (IsTouchingWall(pMD) == false)
                {
                    break;
                }
                yield return null;
            }
        }
        else if (pMD == PlayerMovementDirection.Left)
        {
            while ((GameInputManager.GetManagerKey("Left") || wallJumpBufferDuration > 0) && (holding || slipping))
            {
                if (GameInputManager.GetManagerKey("Left") == false)
                {
                    wallJumpBufferDuration -= Time.deltaTime;
                }
                if (holding == true)
                {
                    playerObject.transform.position = targetPosition;
                }
                slipTime -= Time.deltaTime;
                if (slipTime < 0)
                {
                    holding = false;
                    canHoldOnWall = false;
                    slipping = true;
                    slipTime = float.MaxValue;
                }
                if (IsTouchingWall(pMD) == false)
                {
                    break;
                }
                yield return null;
            }
        }
        
        yield return null;
        holding = false;
        slipping = false;
    }
}

public enum PlayerMovementDirection
{
    None,
    Right,
    Left,
}
