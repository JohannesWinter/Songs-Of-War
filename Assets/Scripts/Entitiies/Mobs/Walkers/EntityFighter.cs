using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFighter : EntityWalker
{
    public float hitCooldown;
    public float waitForHitTimer;
    public float outOfCombatSpeed;
    //overrides normal speed
    public float inCombatSpeed;
    public float hitTriggerRange;
    float currentHitCooldown;
    public float minDistance;
    GameObject currentEnemyObject;
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
        }
        if (SimpleRaycastCheck(simpleEntityMovementDirection, 360, entityObject, hitTriggerRange, LayerMask.GetMask("Player"), LayerMask.GetMask("Enviroment")) && dead == false && IsGrounded())
        {
            if (currentHitCooldown <= 0)
            {
                castingHitRoutine = StartCoroutine(CastHit());
                currentHitCooldown = hitCooldown;
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
            var playerCast = RaycastCheck(GetVector2FromSimpleEntityMovementDirection(currentMovementDirection), 200, entityObject, 7, LayerMask.GetMask("Player"), LayerMask.GetMask("Enviroment"), 100);
            currentEnemyObject = playerCast.collider.gameObject;
            inCombat = true;
        }
        else
        {
            inCombat = false;
            currentEnemyObject = null;
        }
        if (currentHitCooldown > 0)
        {
            currentHitCooldown -= Time.fixedDeltaTime;
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

    protected virtual void FightMovement()
    {
        if (movementLocked == false && currentEnemyObject != null)
        {
            print(currentEnemyObject);
            print((entityObject.transform.position - currentEnemyObject.transform.position).magnitude);
            if (currentMovementDirection == SimpleEntityMovementDirection.East)
            {
                if (velocity.x < maxSpeed)
                {
                    velocity.x += 1 * Time.fixedDeltaTime * acceleration;
                }
                else if (velocity.x > maxSpeed + 0.1f)
                {
                    velocity.x -= 1 * Time.fixedDeltaTime * acceleration;
                }
                if (IsGrounded() && (IsAtLedge(currentMovementDirection, distance: walkwayEndReactionSpeed / acceleration, minLedgeDepth: stepHeight) > 0 || IsTouchingWall(currentMovementDirection, maxDistance: walkwayEndReactionSpeed / acceleration)))
                {
                    currentMovementDirection = SimpleEntityMovementDirection.West;
                }

            }
            if (currentMovementDirection == SimpleEntityMovementDirection.West)
            {
                if (velocity.x > -maxSpeed)
                {
                    velocity.x -= 1 * Time.fixedDeltaTime * acceleration;
                }
                else if (velocity.x < -maxSpeed - 0.1f)
                {
                    velocity.x += 1 * Time.fixedDeltaTime * acceleration;
                }
                if (IsGrounded() && (IsAtLedge(currentMovementDirection, distance: walkwayEndReactionSpeed / acceleration, minLedgeDepth: stepHeight) > 0 || IsTouchingWall(currentMovementDirection, maxDistance: walkwayEndReactionSpeed / acceleration)))
                {
                    currentMovementDirection = SimpleEntityMovementDirection.East;
                }
            }
        }
    }

    IEnumerator CastHit()
    {
        var rq = new EntityRequest();
        rq.type = EntityRequestType.LockMovement;
        rq.duration = waitForHitTimer + 0.2f;
        rq.priority = 2;
        AddRequest(rq);
        rq = new EntityRequest();
        rq.type = EntityRequestType.SetVelocity ;
        rq.vector = Vector2.zero;
        rq.priority = 2;
        AddRequest(rq);
        rq = new EntityRequest();
        rq.type = EntityRequestType.LockUnstoppable;
        rq.duration = waitForHitTimer + 0.2f;
        rq.priority = 2;
        AddRequest(rq);
        var startMovementDirection = currentMovementDirection;
        yield return new WaitForSeconds(waitForHitTimer);
        if (dead)
        {
            castingHitRoutine = null;
            yield break;
        }

        AbilityContext ctx = Instantiate(Manager.m.abilityManager.abilityContext);
        ctx.abilityDef = AbilityDef.Hit;
        ctx.direction = GetAbilityDirectionFromEntityMovementDirction(GetEntityMovementDirectionFromSimpleEntityMovementDirection(startMovementDirection));
        ctx.origin = AbilityOrigin.Entity;
        ctx.originObject = entityObject;
        ctx.entityController = this;
        ctx.damage = this.damage;
        ctx.abilityDef = AbilityDef.EntityFighterHit;
        ctx.knockBack = this.knockBack;
        (_, var hitAbility) = Manager.m.abilityManager.RunAbility(ctx);
        while (hitAbility != null) yield return null;
        castingHitRoutine = null;
    }
}
