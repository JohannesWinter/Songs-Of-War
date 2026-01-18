using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWalker : EntityController
{
    SimpleEntityMovementDirection movementDirection = SimpleEntityMovementDirection.East;

    void FixedUpdate()
    {
        if (movementDirection == SimpleEntityMovementDirection.East)
        {
            if (velocity.x <= 10)
            {
                velocity.x += 1 * Time.deltaTime;
            }
            else if (velocity.x > 10.1f)
            {
                velocity.x =- 1 * Time.deltaTime;
            }
            if ()
        }
    }
}
