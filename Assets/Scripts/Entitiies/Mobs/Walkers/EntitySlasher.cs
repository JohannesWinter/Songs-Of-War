using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySlasher : EntityWalker
{
    public float slashCooldown;
    public float upJumpTimer;
    public float noJumpTimer;
    public float forwardSlashTimer;
    public float outOfCombatSpeed;
    //overrides normal speed
    public float inCombatSpeed;
    public float slashTriggerRange;
    float currentDashCooldown;
    public float minDistance;
    public Vector2 thowUpwards;
    public float throwForwardSpeed;
    public float throwForwardMaxAngel;
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
            int inverted = currentDashCooldown > 0 && (Manager.m.playerManager.playerController.playerObject.transform.position - entityObject.transform.position).magnitude < slashTriggerRange ? -1 : 1;

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
        if (ray.collider != null && dead == false)
        {
            inCombat = true;
            maxSpeed = inCombatSpeed;
            if ((ray.collider.gameObject.transform.position - entityObject.transform.position).magnitude < slashTriggerRange && currentDashCooldown <= 0)
            {
                currentDash = StartCoroutine(SlashAttack(Random.Range(0,2) == 0));
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

    protected virtual IEnumerator SlashAttack(bool jump)
    {
        float upJumpTimeMult = jump ? 1 : 0;
        var throwUpVec = thowUpwards;
        float forward = GetVector2FromSimpleEntityMovementDirection(currentMovementDirection).x;
        float remainingWait = upJumpTimer;
        GenerateAndAddRequest(EntityRequestType.LockUnstoppable, upJumpTimer * upJumpTimeMult + forwardSlashTimer, 2);
        GenerateAndAddRequest(EntityRequestType.LockMovement, upJumpTimer * upJumpTimeMult + forwardSlashTimer, 2);
        if (jump)
        {
            GenerateAndAddRequest(EntityRequestType.SetVelocity, throwUpVec, null, 0, 3);
            yield return new WaitForFixedUpdate();
            bool falling = false;
            while (velocity.y > 0 || remainingWait > 0)
            {
                remainingWait -= Time.deltaTime;
                if ((velocity.y <= 0 || falling) && remainingWait < upJumpTimer - 0.1f)
                {
                    falling = true;
                    GenerateAndAddRequest(EntityRequestType.LockVelocity, Vector2.zero, null, 1f, 3);
                    GenerateAndAddRequest(EntityRequestType.LockGravity, Vector2.zero, null, forwardSlashTimer, 3);
                }
                yield return null;

            }
            yield return new WaitForFixedUpdate();
                    GenerateAndAddRequest(EntityRequestType.UnlockVelocity, 0, 3);
        }
        else
        {
            yield return new WaitForSeconds(noJumpTimer);
        }


        Vector2 currentDashDirection = Vector2.down;
        float directionScale = 0;
        RaycastHit2D currentRay = RaycastCheck(currentDashDirection, 0, this.entityObject, minDistance, LayerMask.GetMask("Enviroment"), LayerMask.GetMask("Nothing"), 1);
        float floorHeight = currentRay.point.y;
        while (currentRay.point.y >= floorHeight - stepHeight && currentRay.point.y <= floorHeight + stepHeight && directionScale < Mathf.Asin(-currentDashDirection.y) * 180 / Mathf.PI)
        {
            //send new ray
            directionScale += 0.01f;
            currentDashDirection = new Vector2(Mathf.Sin(directionScale) * Mathf.PI / 2 * forward, -Mathf.Cos(directionScale * Mathf.PI / 2));
            currentRay = RaycastCheck(currentDashDirection, 0, this.entityObject, 10, LayerMask.GetMask("Enviroment"), LayerMask.GetMask("Nothing"), 1);
        }

        GenerateAndAddRequest(EntityRequestType.AddVelocity, currentDashDirection * throwForwardSpeed, null, 0, 3);
        float timer = forwardSlashTimer;
        while (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            if (IsAtLedge(forward == 1 ? SimpleEntityMovementDirection.East : SimpleEntityMovementDirection.West, 10, 0.05f, 0.3f) > stepHeight && IsGrounded())
            {
                GenerateAndAddRequest(EntityRequestType.SetVelocity, Vector2.zero, null, 0, 3);
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        currentDash = null;
    }
}
