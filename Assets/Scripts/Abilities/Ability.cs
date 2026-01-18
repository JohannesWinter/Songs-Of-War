using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public AbilityContext abilityContext;
    public HitboxContext hitboxContext;

    public void Init(AbilityContext ctx)
    {
        this.abilityContext = ctx;

        hitboxContext = this.gameObject.AddComponent<HitboxContext>();
        hitboxContext.hitboxHolder = HitboxHolder.Ability;
        hitboxContext.abilityOrigin = ctx.origin;
        hitboxContext.originObject = ctx.originObject;
        hitboxContext.damage = ctx.damage;

        this.transform.parent = Manager.m.abilityManager.abilityFolder.transform;
        ctx.gameObject.transform.parent = this.gameObject.transform;
        hitboxContext.transform.parent = this.gameObject.transform;

        Collider2D[] colliders = gameObject.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.AddComponent<HitboxTrigger>();
            colliders[i].gameObject.AddComponent<Rigidbody2D>();
            colliders[i].gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        HitboxTrigger[] hitboxes = gameObject.GetComponentsInChildren<HitboxTrigger>();
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i].hitboxContext = this.hitboxContext;
            hitboxes[i].onHit += HandleHit;
        }

        InitIndiv();
    }
    public abstract void InitIndiv();
    public abstract void HandleHit(HitboxTrigger collider);
}
