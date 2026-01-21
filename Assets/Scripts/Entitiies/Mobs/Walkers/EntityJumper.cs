using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityJumper : EntityWalker
{
    public float jumpCooldown;
    public float waitForJumpTimer;
    public float waitForJumpFinish;
    public float outOfCombatSpeed;
    //overrides normal speed
    public float inCombatSpeed;
    public float jumpTriggerRange;
    public float jumpPower;
    public float jumpHeight;
    float currentJumpCooldown;
    bool inCombat;

    Coroutine castingHitRoutine;

    protected override void FixedUpdate()
    {
        if (castingHitRoutine == null)
        {
            base.FixedUpdate();
        }
        else
        {
            BaseControllerUpdate();
            if (IsGrounded())
            {
                velocity = CheckFriction(velocity);
            }
        }
        if (SimpleRaycastCheck(simpleEntityMovementDirection, 360, entityObject, jumpTriggerRange, LayerMask.GetMask("Player"), LayerMask.GetMask("Enviroment"), 100))
        {
            if (currentJumpCooldown <= 0)
            {
                castingHitRoutine = StartCoroutine(JumpAttack());
                currentJumpCooldown = jumpCooldown;
            }
        }
        if (SimpleRaycastCheck(GetVector2FromSimpleEntityMovementDirection(currentMovementDirection) * -1, 30, entityObject, 6, LayerMask.GetMask("Player"), LayerMask.GetMask("Enviroment")))
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
        if (SimpleRaycastCheck(GetVector2FromSimpleEntityMovementDirection(currentMovementDirection), 200, entityObject, 7, LayerMask.GetMask("Player"), LayerMask.GetMask("Enviroment"), 100))
        {
            inCombat = true;
        }
        else
        {
            inCombat = false;
        }
        if (currentJumpCooldown > 0)
        {
            currentJumpCooldown -= Time.fixedDeltaTime;
        }
        if (inCombat)
        {
            maxSpeed = inCombatSpeed;
        }
        else
        {
            maxSpeed = outOfCombatSpeed;
        }
    }

    protected virtual IEnumerator JumpAttack()
    {
        var rq = new EntityRequest();
        rq.type = EntityRequestType.LockUnstoppable;
        rq.duration = waitForJumpTimer + 1f;
        rq.priority = 2;
        AddRequest(rq);

        rq = new EntityRequest();
        rq.type = EntityRequestType.SetVelocity;
        Vector2 veloc = this.velocity;
        rq.vector = new Vector2(0, veloc.y);
        rq.priority = 2;
        AddRequest(rq);
        yield return new WaitForSeconds(waitForJumpTimer);
        if (dead)
        {
            rq = new EntityRequest();
            rq.type = EntityRequestType.UnlockUnstoppable;
            rq.priority = 3;
            AddRequest(rq);
            castingHitRoutine = null;
            yield break;
        }
        Vector2 jumpDir = GetVector2FromSimpleEntityMovementDirection(currentMovementDirection);
        jumpDir.y = jumpHeight;
        jumpDir.x *= jumpPower;


        rq = new EntityRequest();
        rq.type = EntityRequestType.SetVelocity;
        rq.priority = 2;
        rq.vector = jumpDir;
        AddRequest(rq);
        yield return new WaitForSeconds(waitForJumpFinish);
        castingHitRoutine = null;
    }
}
