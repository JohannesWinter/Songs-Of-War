using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWalker : EntityController
{
    public float acceleration;
    public float maxSpeed;
    public float walkwayEndReactionSpeed;
    public float gottenKnockback;
    public SimpleEntityMovementDirection currentMovementDirection = SimpleEntityMovementDirection.East;
    public HitboxTrigger hitboxTrigger;
    public float knockBack;
    public float damage;
    public void Start()
    {
        hitboxTrigger.onHit += OnHitboxHit;
        hitboxTrigger.hitboxContext.knockback = knockBack;
        hitboxTrigger.hitboxContext.damage = damage;
    }


    protected override void FixedUpdate()
    {
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
            if (IsGrounded() && (IsAtLedge(currentMovementDirection, distance: walkwayEndReactionSpeed / acceleration) > 0 || IsTouchingWall(currentMovementDirection, maxDistance: walkwayEndReactionSpeed / acceleration)))
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
            if (IsGrounded() && (IsAtLedge(currentMovementDirection, distance: walkwayEndReactionSpeed / acceleration) > 0 || IsTouchingWall(currentMovementDirection, maxDistance: walkwayEndReactionSpeed / acceleration)))
            {
                currentMovementDirection = SimpleEntityMovementDirection.East;
            }
        }
        base.FixedUpdate();
    }

    private void OnHitboxHit(HitboxTrigger other)
    {
        var ctx = other.hitboxContext;
        if (ctx.hitboxHolder == HitboxHolder.Entity || ctx.hitboxHolder == HitboxHolder.Player)
        {
            return;
        }
        Vector2 relativePosition = (ctx.originObject.transform.position - entityObject.transform.position).normalized;
        Vector2 oppositePosition = relativePosition * -1; 
        Vector2 knockBack = oppositePosition * gottenKnockback;
        if (IsGrounded())
        {
            knockBack.y = gottenKnockback;
        }
        velocity = knockBack;
    }
}
