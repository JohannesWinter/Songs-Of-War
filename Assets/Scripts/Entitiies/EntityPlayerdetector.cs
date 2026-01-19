using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayerdetector : MonoBehaviour
{
    public bool detected {get; private set;}
    public bool enableDetectionUpdate;
    public HitboxTrigger[] entityHitboxes;
    public float triggerDistance;
    public float triggerHoldDistance;
    GameObject playerObject = Manager.m.playerManager.playerController.playerObject;
    public GameObject entityObject;

    private void Start()
    {
        for (int i = 0; i < entityHitboxes.Length; i++)
        {
            entityHitboxes[i].onHit += OnHitboxHit;
        }
        enableDetectionUpdate = true;
    }

    private void Update()
    {
        if (enableDetectionUpdate)
        {
            for (int i = 0; i < entityHitboxes.Length; i++)
            {
                if (Vector2.Distance(entityHitboxes[i].transform.position, playerObject.transform.position) <= triggerDistance)
                {
                    detected = true;
                    break;
                }
                if (Vector2.Distance(entityHitboxes[i].transform.position, playerObject.transform.position) > triggerHoldDistance)
                {
                    detected = false;
                }
            }
        }
    }

    private void OnHitboxHit(HitboxTrigger other)
    {
        if (enableDetectionUpdate)
        {
            var ctx = other.hitboxContext;
            if (ctx.hitboxHolder == HitboxHolder.Player || ctx.abilityOrigin == AbilityOrigin.Player)
            {
                detected = true;
            }
        }
    }
}
