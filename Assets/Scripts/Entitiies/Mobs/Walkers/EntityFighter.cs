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
            BaseControllerUpdate();
        }
        if (SimpleRaycastCheck(simpleEntityMovementDirection, 30, entityObject, 3, LayerMask.GetMask("Player")))
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
        yield return new WaitForSeconds(waitForHitTimer);

        AbilityContext ctx = Instantiate(Manager.m.abilityManager.abilityContext);
        ctx.abilityDef = AbilityDef.Hit;
        switch (this.simpleEntityMovementDirection)
        {
            case SimpleEntityMovementDirection.East:
                ctx.direction = AbilityDirection.East;
                break;
            case SimpleEntityMovementDirection.West:
                ctx.direction = AbilityDirection.West;
                break;
            default:
                break;

        }
        ctx.origin = AbilityOrigin.Entity;
        ctx.originObject = entityObject;
        ctx.entityController = this;
        ctx.damage = this.damage;

        yield return null;
    }
}
