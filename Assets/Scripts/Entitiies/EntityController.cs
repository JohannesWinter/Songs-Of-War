using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityController : MonoBehaviour
{
    public GameObject entityObject;
    public Transform entityRoot;
    public Transform entityTop;
    public BoxCollider2D entityCollider;
    public Rigidbody2D rb;
    public Vector2 velocity;
    Vector2 positionLastFrame;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundLayer;

    List<EntityRequest> requests = new List<EntityRequest>();
    bool movementLocked;
    List<PlayerRequestTimer> movementLockTimers = new List<PlayerRequestTimer>();
    bool gravityLocked;
    List<PlayerRequestTimer> gravityLockTimers = new List<PlayerRequestTimer>();
    bool velocityLocked;
    List<PlayerRequestTimer> velocityLockTimers = new List<PlayerRequestTimer>();

    public float gravity;
    public float maxFallSpeed;
    public float stepHeight;

    public EntityMovementDirection entityMovementDirection;
    EntityMovementDirection lastEntityMovementDirection;
    SimpleEntityMovementDirection lastSimpleEntityMovementDirection;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessRequests();
        CheckParameterAccessibility();
        velocity = CheckGravity(velocity);
        if (velocityLocked) velocity = Vector2.zero;
        rb.velocity = velocity;
        EndParameterAccessibilty();
    }

    void ProcessRequests()
    {
        //sort requests by priority
        requests.Sort((a, b) => a.priority.CompareTo(b.priority));

        //process outside requests in fixed update
        while (requests.Count > 0)
        {
            var r = requests[0];
            requests.RemoveAt(0);
            PlayerRequestTimer pRT = new PlayerRequestTimer();

            switch (r.type)
            {
                case EntityRequestType.AddVelocity:
                    velocity += r.vector;
                    break;

                case EntityRequestType.SetVelocity:
                    velocity = r.vector;
                    break;

                case EntityRequestType.SetPosition:
                    rb.position = r.vector;
                    break;

                case EntityRequestType.AddPosition:
                    rb.position += r.vector;
                    break;

                case EntityRequestType.LockMovement:
                    pRT.remaining = r.duration;
                    pRT.priority = r.priority;
                    movementLockTimers.Add(pRT);
                    break;

                case EntityRequestType.UnlockMovement:
                    for (int i = movementLockTimers.Count - 1; i >= 0; i--)
                    {
                        if (movementLockTimers[i].priority <= r.priority)
                        {
                            movementLockTimers.RemoveAt(i);
                        }
                    }
                    break;

                case EntityRequestType.LockGravity:
                    pRT.remaining = r.duration;
                    pRT.priority = r.priority;
                    gravityLockTimers.Add(pRT);
                    break;

                case EntityRequestType.UnlockGravity:
                    for (int i = gravityLockTimers.Count - 1; i >= 0; i--)
                    {
                        if (gravityLockTimers[i].priority <= r.priority)
                        {
                            gravityLockTimers.RemoveAt(i);
                        }
                    }
                    break;

                case EntityRequestType.LockVelocity:
                    pRT.remaining = r.duration;
                    pRT.priority = r.priority;
                    velocityLockTimers.Add(pRT);
                    break;

                case EntityRequestType.UnlockVelocity:
                    for (int i = velocityLockTimers.Count - 1; i >= 0; i--)
                    {
                        if (velocityLockTimers[i].priority <= r.priority)
                        {
                            velocityLockTimers[i].remaining = 0;
                        }
                    }
                    break;

                case EntityRequestType.OverrideGravity:
                    gravity = r.values[0];
                    break;
            }
        }
        UpdatePriorities();
    }

    void UpdatePriorities()
    {
        for (int i = movementLockTimers.Count - 1; i >= 0; i--)
        {
            movementLockTimers[i].remaining -= Time.fixedDeltaTime;
            if (movementLockTimers[i].remaining <= 0)
            {
                movementLockTimers.RemoveAt(i);
            }
        }
        if (movementLockTimers.Count <= 0) movementLocked = false;
        else movementLocked = true;

        for (int i = gravityLockTimers.Count - 1; i >= 0; i--)
        {
            gravityLockTimers[i].remaining -= Time.fixedDeltaTime;
            if (gravityLockTimers[i].remaining <= 0)
            {
                gravityLockTimers.RemoveAt(i);
            }
        }
        if (gravityLockTimers.Count <= 0) gravityLocked = false;
        else gravityLocked = true;

        for (int i = velocityLockTimers.Count - 1; i >= 0; i--)
        {
            velocityLockTimers[i].remaining -= Time.fixedDeltaTime;
            if (velocityLockTimers[i].remaining <= 0)
            {
                velocityLockTimers.RemoveAt(i);
            }
        }
        if (velocityLockTimers.Count <= 0) velocityLocked = false;
        else velocityLocked = true;
    }

    public void AddRequest(EntityRequest request)
    {
        //receive movement requests from outside
        requests.Add(request);
    }

    void CheckParameterAccessibility()
    {
        entityMovementDirection = GetDirectionFromVelocity(velocity);
    }
    public static EntityMovementDirection GetDirectionFromVelocity(Vector2 velocity, float deadZone = 0.01f)
    {
        if (velocity.magnitude < deadZone)
            return EntityMovementDirection.None;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        if (angle >= 337.5f || angle < 22.5f) return EntityMovementDirection.East;
        if (angle < 67.5f) return EntityMovementDirection.NorthEast;
        if (angle < 112.5f) return EntityMovementDirection.North;
        if (angle < 157.5f) return EntityMovementDirection.NorthWest;
        if (angle < 202.5f) return EntityMovementDirection.West;
        if (angle < 247.5f) return EntityMovementDirection.SouthWest;
        if (angle < 292.5f) return EntityMovementDirection.South;
        return EntityMovementDirection.SouthEast;
    }

    Vector2 CheckGravity(Vector2 velocity)
    {
        //check if gravity is enabled
        if (gravityLocked) return velocity;

        velocity.y += -1 * gravity * Time.fixedDeltaTime;
        if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
        }
        return velocity;
    }
    void EndParameterAccessibilty()
    {
        //updates Past-Velocity-Update parameters
        if (entityMovementDirection == EntityMovementDirection.None && lastEntityMovementDirection == EntityMovementDirection.None) 
        { 
            lastEntityMovementDirection = EntityMovementDirection.East; 
        }
        else if (entityMovementDirection != EntityMovementDirection.None)
        {
            lastEntityMovementDirection = entityMovementDirection;
        }
        positionLastFrame = entityObject.transform.position;
        lastSimpleEntityMovementDirection = GetSimpleEntityMovementDirection(entityMovementDirection, lastSimpleEntityMovementDirection);
    }

    protected SimpleEntityMovementDirection GetSimpleEntityMovementDirection(EntityMovementDirection entityMovementDirection, SimpleEntityMovementDirection lastSimpleEntityMovementDirection)
    {
        //returns new Value (East/West) as current facing direction
        if (entityMovementDirection == EntityMovementDirection.None ||
            entityMovementDirection == EntityMovementDirection.North ||
            entityMovementDirection == EntityMovementDirection.South)
        {
            if (lastSimpleEntityMovementDirection == SimpleEntityMovementDirection.None)
            {
                return SimpleEntityMovementDirection.East;
            }
            else
            {
                return lastSimpleEntityMovementDirection;
            }
        }
        else if (entityMovementDirection == EntityMovementDirection.NorthEast ||
            entityMovementDirection == EntityMovementDirection.East ||
            entityMovementDirection == EntityMovementDirection.SouthEast)
        {
            return SimpleEntityMovementDirection.East;
        }
        else
        {
            return SimpleEntityMovementDirection.West;
        }
    }

    protected bool IsGrounded()
    {
        RaycastHit2D downCheck = Physics2D.Raycast(
            (Vector2)entityObject.transform.position,
            Vector2.down,
            entityObject.transform.localScale.y / 2 + 0.05f,
            groundLayer
        );
        RaycastHit2D downCheckRight = Physics2D.Raycast(
            (Vector2)entityObject.transform.position + Vector2.right * entityObject.transform.localScale.x / 2 * 0.95f,
            Vector2.down,
            entityObject.transform.localScale.y / 2 + 0.05f,
            groundLayer
        );
        RaycastHit2D downCheckLeft = Physics2D.Raycast(
            (Vector2)entityObject.transform.position + Vector2.left * entityObject.transform.localScale.x / 2 * 0.95f,
            Vector2.down,
            entityObject.transform.localScale.y / 2 + 0.05f,
            groundLayer
        );

        bool grounded = (downCheck || downCheckLeft || downCheckRight);
        return grounded;
    }
    protected bool IsTopFree(Vector2 targetPosition)
    {
        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                targetPosition + Vector2.right * (-entityObject.transform.localScale.x / 2 + (entityObject.transform.localScale.x * i / 10)),
                Vector2.up,
                entityObject.transform.localScale.y / 2 + 0.1f,
                groundLayer
                );
            if (topHit == true)
            {
                return false;
            }
        }
        return true;
    }
    protected bool IsBotFree(Vector2 targetPosition)
    {
        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                targetPosition + Vector2.right * (-entityObject.transform.localScale.x / 2 + (entityObject.transform.localScale.x * i / 10)),
                Vector2.down,
                entityObject.transform.localScale.y / 2 + 0.05f,
                groundLayer
                );
            if (topHit == true)
            {
                return false;
            }
        }
        return true;
    }

    protected bool IsTouchingWall(EntityMovementDirection eMD, SimpleEntityMovementDirection lastSimpleEMD)
    {
        if (GetEntityMovementDirectionVector(GetSimpleEntityMovementDirection(eMD, lastSimpleEMD)) == Vector2.zero) return false;

        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                (Vector2)entityObject.transform.position + Vector2.up * (-entityObject.transform.localScale.y / 2 + (entityObject.transform.localScale.y * i / 10)),
                GetEntityMovementDirectionVector(GetSimpleEntityMovementDirection(eMD, lastSimpleEMD)),
                entityObject.transform.localScale.x / 2 + 0.1f,
                groundLayer
                );
            if (topHit == true)
            {
                return true;
            }
        }
        return false;
    }
    protected float IsAtLedge(SimpleEntityMovementDirection sEMD, float maxLedgeCheckDepth = 10f)
    {
        if (sEMD == SimpleEntityMovementDirection.None)
            return 0f;

        float halfWidth = entityObject.transform.localScale.x * 0.5f;
        float direction = sEMD == SimpleEntityMovementDirection.East ? 1f : -1f;

        Vector2 origin = (Vector2)entityRoot.position + Vector2.right * halfWidth * direction;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, maxLedgeCheckDepth, groundLayer);

        if (hit.collider == null)
        {
            return -1f;
        }

        float groundDistance = hit.distance;

        if (groundDistance <= 0.05f)
            return 0f;

        return groundDistance;
    }

    public static Vector2 GetEntityMovementDirectionVector(EntityMovementDirection eMD)
    {
        switch (eMD)
        {
            case EntityMovementDirection.North: return Vector2.up;
            case EntityMovementDirection.NorthEast: return new Vector2(1, 1).normalized;
            case EntityMovementDirection.East: return Vector2.right;
            case EntityMovementDirection.SouthEast: return new Vector2(1, -1).normalized;
            case EntityMovementDirection.South: return Vector2.down;
            case EntityMovementDirection.SouthWest: return new Vector2(-1, -1).normalized;
            case EntityMovementDirection.West: return Vector2.left;
            case EntityMovementDirection.NorthWest: return new Vector2(-1, 1).normalized;
            default: return Vector2.zero;
        }
    }
    public static Vector2 GetEntityMovementDirectionVector(SimpleEntityMovementDirection eMD)
    {
        switch (eMD)
        {
            case SimpleEntityMovementDirection.East: return GetEntityMovementDirectionVector(EntityMovementDirection.East);
            case SimpleEntityMovementDirection.West: return GetEntityMovementDirectionVector(EntityMovementDirection.West);
            default: return GetEntityMovementDirectionVector(EntityMovementDirection.None);
        }
    }
}
