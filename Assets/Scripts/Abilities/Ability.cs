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

        ctx.gameObject.transform.parent = this.gameObject.transform;
        hitboxContext.transform.parent = this.gameObject.transform;

        Collider2D[] colliders = gameObject.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.AddComponent<HitboxTrigger>();
        }

        HitboxTrigger[] hitboxes = gameObject.GetComponentsInChildren<HitboxTrigger>();
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i].parentHitboxContext = this.hitboxContext;
            hitboxes[i].OnHit += HandleHit;
        }

        InitIndiv();
    }
    public abstract void InitIndiv();
    public abstract void HandleHit(Collider2D collider);
}
