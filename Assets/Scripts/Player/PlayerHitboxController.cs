using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxController : MonoBehaviour
{
    public HitboxTrigger playerHitboxTrigger;
    public HitboxContext playerHitboxContext;
    PlayerStatsController playerStatsController;

    void Start()
    {
        playerHitboxTrigger.onHit += OnHitboxHit;
        playerStatsController = Manager.m.playerManager.playerStatsController;
    }
    void Update()
    {
        if (playerStatsController.GetInvincibil())
        {
            playerHitboxTrigger.GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            playerHitboxTrigger.GetComponent<Collider2D>().enabled = true;
        }
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

            rq = new PlayerMovementRequest();
            rq.type = PlayerMovementRequestType.UnlockVelocity;
            rq.priority = 3;
            pc.AddRequest(rq);

            var rq2 = new PlayerStatsRequest();
            rq2.type = PlayerStatsRequestType.AddHealth;
            rq2.priority = 2;
            rq2.intValue = -ctx.damage;
            playerStatsController.AddRequest(rq2);

            var rq3 = new PlayerStatsRequest();
            rq3.type = PlayerStatsRequestType.SetInvicibility;
            rq3.priority = 3;
            rq3.floatValue = Manager.m.playerManager.playerController.hitInvincibilityTime;
            playerStatsController.AddRequest(rq3);
            
        }
    }
}
