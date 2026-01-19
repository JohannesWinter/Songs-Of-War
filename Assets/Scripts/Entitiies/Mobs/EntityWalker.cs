using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWalker : EntityController
{
    public float acceleration;
    public float maxSpeed;
    public float walkwayEndReactionSpeed;
    public SimpleEntityMovementDirection currentMovementDirection = SimpleEntityMovementDirection.East;
    public float knockBack;
    public int damage;
    public void Start()
    {
        hitboxTriggers[0].onHit += OnHitboxHit;
        hitboxTriggers[0].hitboxContext.knockback = knockBack;
        hitboxTriggers[0].hitboxContext.damage = damage;
    }


    protected override void FixedUpdate()
    {
        if (dead == false)
        {
            BaseMovement();
            TryStepUp(velocity);
        }
        else velocity = CheckFriction(velocity);
        base.FixedUpdate();
    }

    protected virtual void BaseMovement()
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
    protected virtual void OnHitboxHit(HitboxTrigger other)
    {
        var ctx = other.hitboxContext;
        if (ctx.hitboxHolder == HitboxHolder.Entity || ctx.hitboxHolder == HitboxHolder.Player)
        {
            return;
        }
        health -= ctx.damage;
        Vector2 relativePosition = (ctx.originObject.transform.position - entityObject.transform.position).normalized;
        Vector2 oppositePosition = relativePosition * -1; 
        Vector2 knockBack = oppositePosition * gottenKnockback;
        if (IsGrounded())
        {
            knockBack.y = gottenKnockback;
        }
        if (health < 0) knockBack *= 2;
        velocity = knockBack;
    }
}
