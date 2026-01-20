using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class EntityController : MonoBehaviour
{
    public GameObject entityObject;
    public Transform entityRoot;
    public Transform entityTop;
    public BoxCollider2D entityCollider;
    public Collider2D entityHitbox;
    public Rigidbody2D rb;
    public Vector2 velocity;
    Vector2 positionLastFrame;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundLayer;

    List<EntityRequest> requests = new List<EntityRequest>();
    public bool movementLocked { get; private set; }
    List<PlayerRequestTimer> movementLockTimers = new List<PlayerRequestTimer>();
    public bool gravityLocked { get; private set; }
    List<PlayerRequestTimer> gravityLockTimers = new List<PlayerRequestTimer>();
    public bool velocityLocked { get; private set; }
    List<PlayerRequestTimer> velocityLockTimers = new List<PlayerRequestTimer>();

    public EntityMovementDirection entityMovementDirection;
    public SimpleEntityMovementDirection simpleEntityMovementDirection;
    EntityMovementDirection lastEntityMovementDirection;
    SimpleEntityMovementDirection lastSimpleEntityMovementDirection;
    public HitboxTrigger[] hitboxTriggers;
    public EntityPlayerdetector playerdetector;

    public float gravity;
    public float maxFallSpeed;
    public float stepHeight;

    public int health;
    public bool dead;
    public float bodyTimer;
    float remainingBodyTimer;
    public float friction;
    public float gottenKnockback;
    void Start()
    {
        rb.gravityScale = 0;
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        BaseControllerUpdate();
    }

    protected void BaseControllerUpdate()
    {
        CheckDeath();
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
        //resets on-ground parameters
        if (IsGrounded())
        {
            if (velocity.y < 0) velocity.y = 0;
        }
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
    protected Vector2 CheckFriction(Vector2 velocity)
    {
        velocity.x *= Mathf.Pow(1 - friction, Time.fixedDeltaTime);
        return velocity;
    }

    protected void TryStepUp(Vector2 currentVelocity)
    {
        //check if stepup (movement) is enabled
        if (movementLocked) return;

        //check stepup logic
        if (IsGrounded() == false) { return; }

        Vector2 origin = (Vector2)entityRoot.transform.position + Vector2.up * 0.1f;

        Vector2 direction = Vector2.zero;
        switch (simpleEntityMovementDirection)
        {
            case SimpleEntityMovementDirection.East:
                direction = Vector2.right;
                break;
            case SimpleEntityMovementDirection.West:
                direction = Vector2.left;
                break;
            case SimpleEntityMovementDirection.None:
                return;
        }
        //check if entity can stepup
        RaycastHit2D lowerHit = Physics2D.Raycast(
            origin,
            direction,
            entityCollider.size.x * entityObject.transform.localScale.x / 2 + 0.1f,
            groundLayer
        );
        if (!lowerHit) return;

        RaycastHit2D upperHit = Physics2D.Raycast(
            origin + Vector2.up * stepHeight,
            direction,
            entityCollider.size.x * entityObject.transform.localScale.x / 2 + 0.1f,
            groundLayer
        );
        float currentStepHeight = stepHeight;
        int counter = 10;
        while (counter > 0)
        {
            RaycastHit2D newHit = Physics2D.Raycast(
                origin + Vector2.up * (currentStepHeight - stepHeight / 10),
                direction,
                entityCollider.size.x * entityObject.transform.localScale.x / 2 + 0.1f,
                groundLayer
            );
            if (!newHit)
            {
                upperHit = newHit;
                currentStepHeight -= stepHeight / 10;
            }
            else
            {
                break;
            }
            counter--;
        }
        //entity steps up
        if (!upperHit)
        {
            entityObject.transform.position += Vector3.up * (currentStepHeight + stepHeight / 5) + (Vector3)direction * 0.15f;
        }
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
        simpleEntityMovementDirection = GetSimpleEntityMovementDirection(entityMovementDirection);
        lastSimpleEntityMovementDirection = GetLastSimpleEntityMovementDirection(entityMovementDirection, lastSimpleEntityMovementDirection);
        positionLastFrame = entityObject.transform.position;
    }

    protected SimpleEntityMovementDirection GetLastSimpleEntityMovementDirection(EntityMovementDirection entityMovementDirection, SimpleEntityMovementDirection lastSimpleEntityMovementDirection)
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
    protected SimpleEntityMovementDirection GetSimpleEntityMovementDirection(EntityMovementDirection entityMovementDirection)
    {
        switch (entityMovementDirection)
        {
            case EntityMovementDirection.East:
            case EntityMovementDirection.NorthEast:
            case EntityMovementDirection.SouthEast:
                return SimpleEntityMovementDirection.East;
            case EntityMovementDirection.West:
            case EntityMovementDirection.NorthWest:
            case EntityMovementDirection.SouthWest:
                return SimpleEntityMovementDirection.West;
            default:
                return SimpleEntityMovementDirection.None;
        }
    }

    protected bool IsGrounded(float distance = 0.05f)
    {
        RaycastHit2D downCheck = Physics2D.Raycast(
            (Vector2)entityObject.transform.position,
            Vector2.down,
            entityObject.transform.localScale.y / 2 + distance,
            groundLayer
        );
        RaycastHit2D downCheckRight = Physics2D.Raycast(
            (Vector2)entityObject.transform.position + Vector2.right * entityObject.transform.localScale.x / 2 * 0.95f,
            Vector2.down,
            entityObject.transform.localScale.y / 2 + distance,
            groundLayer
        );
        RaycastHit2D downCheckLeft = Physics2D.Raycast(
            (Vector2)entityObject.transform.position + Vector2.left * entityObject.transform.localScale.x / 2 * 0.95f,
            Vector2.down,
            entityObject.transform.localScale.y / 2 + distance,
            groundLayer
        );

        bool grounded = (downCheck || downCheckLeft || downCheckRight);
        return grounded;
    }
    protected bool IsTopFree(Vector2 targetPosition, float distance = 0.1f)
    {
        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                targetPosition + Vector2.right * (-entityObject.transform.localScale.x / 2 + (entityObject.transform.localScale.x * i / 10)),
                Vector2.up,
                entityObject.transform.localScale.y / 2 + distance,
                groundLayer
                );
            if (topHit == true)
            {
                return false;
            }
        }
        return true;
    }
    protected bool IsBotFree(Vector2 targetPosition, float distance = 0.05f)
    {
        for (int i = 0; i < 10; i++)
        {
            bool topHit = Physics2D.Raycast(
                targetPosition + Vector2.right * (-entityObject.transform.localScale.x / 2 + (entityObject.transform.localScale.x * i / 10)),
                Vector2.down,
                entityObject.transform.localScale.y / 2 + distance,
                groundLayer
                );
            if (topHit == true)
            {
                return false;
            }
        }
        return true;
    }

    protected bool IsTouchingWall(SimpleEntityMovementDirection lastSimpleEMD, EntityMovementDirection eMD = EntityMovementDirection.None, float maxDistance = 0.1f)
    {
        if (GetEntityMovementDirectionVector(GetLastSimpleEntityMovementDirection(eMD, lastSimpleEMD)) == Vector2.zero) return false;

        Vector2 startPosition = (Vector2)entityObject.transform.position + Vector2.up * (-entityObject.transform.localScale.y / 2 + stepHeight);
        for (int i = 0; i < 10; i++)
        {
            Vector2 testPosition = (Vector2)entityObject.transform.position + Vector2.up * (-entityObject.transform.localScale.y / 2 + (entityObject.transform.localScale.y * i / 10));
            if (testPosition.y < startPosition.y && stepHeight < 1) continue;
            bool topHit = Physics2D.Raycast(
                testPosition,
                GetEntityMovementDirectionVector(GetLastSimpleEntityMovementDirection(eMD, lastSimpleEMD)),
                entityObject.transform.localScale.x / 2 + maxDistance,
                groundLayer
                );
            if (topHit == true)
            {
                return true;
            }
        }
        return false;
    }
    protected float IsAtLedge(SimpleEntityMovementDirection sEMD, float maxLedgeCheckDepth = 10f, float minLedgeDepth = 0.05f, float distance = 0f)
    {
        if (sEMD == SimpleEntityMovementDirection.None)
            return 0f;

        float halfWidth = entityObject.transform.localScale.x * 0.5f;
        float direction = sEMD == SimpleEntityMovementDirection.East ? 1f : -1f;

        Vector2 origin = (Vector2)entityRoot.position + Vector2.right * (halfWidth + distance) * direction;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, maxLedgeCheckDepth, groundLayer);

        if (hit.collider == null)
        {
            return float.MaxValue;
        }

        float groundDistance = hit.distance;

        if (groundDistance <= minLedgeDepth)
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

    protected bool CheckDeath()
    {
        if (health < 0 && dead == false)
        {
            dead = true;
            remainingBodyTimer = bodyTimer;
            return true;
        }
        if (health < 0)
        {
            remainingBodyTimer -= Time.fixedDeltaTime;
            if (remainingBodyTimer <= 0)
            {
                Destroy(this.gameObject);
            }
            entityHitbox.enabled = false;
        }
        return false;
    }

    public static bool SimpleRaycastCheck(
        SimpleEntityMovementDirection direction,
        float spreadAngle,
        GameObject originObject,
        float distance,
        LayerMask layerMask,
        int rayCount = 10
    )
    {
        if (direction == SimpleEntityMovementDirection.None)
            return false;

        Vector2 baseDir = direction == SimpleEntityMovementDirection.East ? Vector2.right : Vector2.left;
        Vector2 origin = originObject.transform.position;

        if (rayCount < 1) rayCount = 1;
        float halfSpread = spreadAngle * 0.5f;

        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount == 1 ? 0.5f : (float)i / (rayCount - 1);
            float angle = Mathf.Lerp(-halfSpread, halfSpread, t);

            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, layerMask);
            Debug.DrawRay(origin, dir * distance, hit ? Color.red : Color.green, 0.05f);


            if (hit.collider != null)
            {
                return true;
            }
        }

        return false;
    }
}
