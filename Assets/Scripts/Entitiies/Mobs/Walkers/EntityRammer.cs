using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRammer : EntityWalker
{
    public float ramAcceleration;
    public float ramMaxSpeed;
    public bool ramming;
    public float startRamDistance;
    public float ramCooldown;
    float currentRamCooldown;
    

    protected override void FixedUpdate()
    {
        UpdateRam();
        base.FixedUpdate();
    }

    protected virtual void UpdateRam()
    {
        if (currentRamCooldown <= 0 && CheckRam())
        {
            currentRamCooldown = ramCooldown;
            ramming = true;
        }
        if (currentRamCooldown > 0)
        {
            currentRamCooldown -= Time.fixedDeltaTime;
        }
    }

    protected virtual bool CheckRam()
    {
        if (SimpleRaycastCheck(currentMovementDirection, 30, entityObject, startRamDistance, LayerMask.GetMask("Player")))
        {
            return true;
        }
        return false;
    }

    protected override void BaseMovement()
    {
        if (ramming == false)
        {
            base.BaseMovement();
        }
        else
        {
            if (currentMovementDirection == SimpleEntityMovementDirection.East)
            {
                if (velocity.x < ramMaxSpeed)
                {
                    velocity.x += 1 * Time.fixedDeltaTime * ramAcceleration;
                }
                else if (velocity.x > ramMaxSpeed + 0.1f)
                {
                    velocity.x -= 1 * Time.fixedDeltaTime * ramAcceleration;
                }
                if (IsGrounded() && (IsAtLedge(currentMovementDirection, distance: walkwayEndReactionSpeed * (ramMaxSpeed/maxSpeed) / acceleration, minLedgeDepth: stepHeight) > 0 || IsTouchingWall(currentMovementDirection, maxDistance: walkwayEndReactionSpeed / acceleration)))
                {
                    currentMovementDirection = SimpleEntityMovementDirection.West;
                    ramming = false;
                }

            }
            if (currentMovementDirection == SimpleEntityMovementDirection.West)
            {
                if (velocity.x > -ramMaxSpeed)
                {
                    velocity.x -= 1 * Time.fixedDeltaTime * ramAcceleration;
                }
                else if (velocity.x < -ramMaxSpeed - 0.1f)
                {
                    velocity.x += 1 * Time.fixedDeltaTime * ramAcceleration;
                }
                if (IsGrounded() && (IsAtLedge(currentMovementDirection, distance: walkwayEndReactionSpeed *  (ramMaxSpeed / maxSpeed) / acceleration, minLedgeDepth: stepHeight) > 0 || IsTouchingWall(currentMovementDirection, maxDistance: walkwayEndReactionSpeed / acceleration)))
                {
                    currentMovementDirection = SimpleEntityMovementDirection.East;
                    ramming = false;
                }
            }
        }
    }

    protected override void OnHitboxHit(HitboxTrigger other)
    {
        if (CheckRam())
        {
            ramming = false;
        }
        base.OnHitboxHit(other);
    }
}
