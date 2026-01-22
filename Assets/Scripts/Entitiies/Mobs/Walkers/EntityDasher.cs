using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDasher : EntityWalker
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
        if (inCombat == false)
        {
            base.FixedUpdate();
        }
    }
}
