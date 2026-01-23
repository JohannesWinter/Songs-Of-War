using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySlasher : EntityWalker
{
    public float slashCooldown;
    public float upJumpTimer;
    public float forwardSlashTimer;
    public float outOfCombatSpeed;
    //overrides normal speed
    public float inCombatSpeed;
    public float slashTriggerRange;
    float currentDashCooldown;
    public float minDistance;
    public Vector2 thowUpwards;
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
            if ((ray.collider.gameObject.transform.position - entityObject.transform.position).magnitude < slashTriggerRange && currentDashCooldown <= 0)
            {
                currentDash = StartCoroutine(SlashAttack());
                currentDashCooldown = slashCooldown;
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

    protected virtual IEnumerator SlashAttack()
    {
        var throwUpVec = thowUpwards;
        var throwForwardVec = throwForward;
        float remainingWait = upJumpTimer;
        throwForwardVec.x = GetVector2FromSimpleEntityMovementDirection(currentMovementDirection).x * throwForwardVec.x;
        GenerateAndAddRequest(EntityRequestType.LockUnstoppable, upJumpTimer + forwardSlashTimer, 2);
        GenerateAndAddRequest(EntityRequestType.LockMovement, upJumpTimer + forwardSlashTimer, 2);
        GenerateAndAddRequest(EntityRequestType.SetVelocity, throwUpVec, null, 0, 3);
        print("up");
        yield return new WaitForFixedUpdate();
        bool falling = false;
        while(velocity.y > 0 || remainingWait > 0)
        {
            remainingWait -=Time.deltaTime;
            if ((velocity.y <= 0 || falling) && remainingWait < upJumpTimer - 0.1f)
            {
                falling = true;
                GenerateAndAddRequest(EntityRequestType.LockVelocity, Vector2.zero, null, 1f, 3);
            }
            yield return null;

        }
        yield return new WaitForFixedUpdate();
        print("forward");
        GenerateAndAddRequest(EntityRequestType.UnlockVelocity,0, 3);
        GenerateAndAddRequest(EntityRequestType.AddVelocity, throwForwardVec, null, 0, 3);
        yield return new WaitForSeconds(forwardSlashTimer);
        currentDash = null;
    }
}
