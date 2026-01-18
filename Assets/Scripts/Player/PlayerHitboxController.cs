using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxController : MonoBehaviour
{
    public HitboxTrigger playerHitboxTrigger;
    public HitboxContext playerHitboxContext;
    void Start()
    {
        playerHitboxTrigger.onHit += OnHitboxHit;
    }
    void Update()
    {
        
    }

    public void OnHitboxHit(HitboxTrigger other)
    {
        var ctx = other.hitboxContext;
        if (ctx.hitboxHolder == HitboxHolder.Player || ctx.abilityOrigin == AbilityOrigin.Player) { return; }
        if (ctx.hitboxHolder == HitboxHolder.Entity)
        {
            PlayerController pc = Manager.m.playerManager.playerController;
            Vector2 relativePosition = (pc.playerObject.transform.position - ctx.originObject.transform.position).normalized;
            Vector2 knockback = relativePosition * ctx.knockback;
            knockback.y *= 2;
            knockback.y = knockback.y < 5 ? 5 : knockback.y;

            PlayerMovementRequest rq = new PlayerMovementRequest();
            rq.type = PlayerMovementRequestType.SetVelocity;
            rq.priority = 3;
            rq.vector = knockback;
            pc.AddRequest(rq);

            rq = new PlayerMovementRequest();
            rq.type = PlayerMovementRequestType.LockMovement;
            rq.priority = 3;
            rq.duration = 0.3f;
            pc.AddRequest(rq);

            var rq2 = new PlayerStatsRequest();
            rq2.type = PlayerStatsRequestType.AddHealth;
            rq2.priority = 2;
            rq2.intValue = -ctx.damage;
            Manager.m.playerManager.playerStatsController.AddRequest(rq2);
            
        }
    }
}
