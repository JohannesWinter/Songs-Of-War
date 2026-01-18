using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    public Action<Collider2D> OnHit;
    public HitboxContext parentHitboxContext;

    void OnTriggerEnter2D(Collider2D other)
    {
        OnHit?.Invoke(other);
    }
}
