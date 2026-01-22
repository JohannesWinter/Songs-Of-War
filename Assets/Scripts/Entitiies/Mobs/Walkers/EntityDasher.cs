using System.Collections;
using UnityEngine;

public class EntityDasher : EntityWalker
{
    public float dashCooldown;
    public float backDashTimer;
    public float forwardDashTimer;
    public float outOfCombatSpeed;
    //overrides normal speed
    public float inCombatSpeed;
    public float dashTriggerRange;
    float currentDashCooldown;
    public float minDistance;
    public Vector2 throwBack;
    public Vector2 throwForward;
    Coroutine currentDash;
    bool inCombat;

    protected override void FixedUpdate()
    {
        if (dead)
        {
            if (currentDash != null)
            {
                StopCoroutine(currentDash);
                currentDash = null;
            }
            GenerateAndAddRequest(EntityRequestType.LockMovement, Vector2.zero, null, bodyTimer, 3);
            GenerateAndAddRequest(EntityRequestType.UnlockGravity, 0, 3);
            GenerateAndAddRequest(EntityRequestType.UnlockVelocity, 0, 3);
            GenerateAndAddRequest(EntityRequestType.UnlockUnstoppable, 0, 3);
        }
        if (currentDash == null)
        {
            base.FixedUpdate();
            int inverted = currentDashCooldown > 0 ? -1 : 1;

            if (IsAtLedge(currentMovementDirection) <= stepHeight && IsAtLedge(GetSimpleMovementDirectionInverted(currentMovementDirection)) <= stepHeight && SimpleRaycastCheck(GetVector2FromSimpleEntityMovementDirection(currentMovementDirection) * -1 * inverted, 140, entityObject, inCombat ? minDistance : 6, LayerMask.GetMask("Player"), LayerMask.GetMask("Enviroment"), 45))
            {
                switch (currentMovementDirection)
                {
                    case SimpleEntityMovementDirection.West:
                        currentMovementDirection = SimpleEntityMovementDirection.East;
                        break;
                    case SimpleEntityMovementDirection.East:
                        currentMovementDirection = SimpleEntityMovementDirection.West;
                        break;
                }
            }
        }
        else
        {
            BaseControllerUpdate();
            velocity = CheckFriction(velocity);
        }
        RaycastHit2D ray = RaycastCheck(GetVector2FromSimpleEntityMovementDirection(currentMovementDirection), 360, entityObject, minDistance, LayerMask.GetMask("Player"), LayerMask.GetMask("Enviroment"), 90);
        if (ray.collider != null)
        {
            inCombat = true;
            maxSpeed = inCombatSpeed;
            if ((ray.collider.gameObject.transform.position - entityObject.transform.position).magnitude < dashTriggerRange && currentDashCooldown <= 0)
            {
                currentDash = StartCoroutine(DashAttack());
                currentDashCooldown = dashCooldown;
            }
        }
        else
        {
            inCombat = false;
            maxSpeed = outOfCombatSpeed;
        }
        


        if (currentDashCooldown > 0)
        {
            currentDashCooldown -= Time.fixedDeltaTime;
        }
    }

    protected virtual IEnumerator DashAttack()
    {
        var throwBackVec = throwBack;
        var throwForwardVec = throwForward;
        throwBackVec.x = GetVector2FromSimpleEntityMovementDirection(currentMovementDirection).x * throwBackVec.x;
        throwForwardVec.x = GetVector2FromSimpleEntityMovementDirection(currentMovementDirection).x * throwForwardVec.x;
        GenerateAndAddRequest(EntityRequestType.LockUnstoppable, backDashTimer + forwardDashTimer, 2);
        GenerateAndAddRequest(EntityRequestType.LockMovement, backDashTimer + forwardDashTimer, 2);
        GenerateAndAddRequest(EntityRequestType.SetVelocity, Vector2.zero, null, 0, 3);
        GenerateAndAddRequest(EntityRequestType.AddVelocity, throwBackVec, null, 0, 3);
        yield return new WaitForSeconds(backDashTimer);
        GenerateAndAddRequest(EntityRequestType.AddVelocity, throwForwardVec, null, 0, 3);
        yield return new WaitForSeconds(forwardDashTimer);
        currentDash = null;
    }
}
