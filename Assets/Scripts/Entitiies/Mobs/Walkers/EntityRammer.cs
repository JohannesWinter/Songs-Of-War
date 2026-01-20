using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRammer : EntityWalker
{
    public float ramAcceleration;
    public float ramMaxSpeed;
    public bool ramming;
    

    protected override void FixedUpdate()
    {
        if (SimpleRaycastCheck(currentMovementDirection, 30, entityObject, 1,layerMask: 9)) ramming = true;
        base.FixedUpdate();
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
}
