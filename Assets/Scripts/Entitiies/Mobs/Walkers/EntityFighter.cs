using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFighter : EntityWalker
{
    public float hitCooldown;
    public float waitForHitTimer;
    float currentHitCooldown;
    
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
        if (SimpleRaycastCheck(simpleEntityMovementDirection, 30, entityObject, 1, LayerMask.GetMask("Player")))
        {
            if (currentHitCooldown <= 0)
            {
                castingHitRoutine = StartCoroutine(CastHit());
                currentHitCooldown = hitCooldown;
            }
        }
        if (currentHitCooldown > 0)
        {
            currentHitCooldown -= Time.fixedDeltaTime;
        }
    }

    IEnumerator CastHit()
    {
        var rq = new EntityRequest();
        rq.type = EntityRequestType.LockVelocity;
        rq.duration = waitForHitTimer;
        rq.priority = 3;
        AddRequest(rq);
        yield return new WaitForSeconds(waitForHitTimer);

        AbilityContext ctx = Instantiate(Manager.m.abilityManager.abilityContext);
        ctx.abilityDef = AbilityDef.Hit;
        ctx.direction = GetAbilityDirectionFromEntityMovementDirction(GetEntityMovementDirectionFromSimpleEntityMovementDirection(currentMovementDirection));
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
