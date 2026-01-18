using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    public Action<HitboxTrigger> OnHit;
    public HitboxContext parentHitboxContext;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == this.gameObject.layer)
        {
            OnHit?.Invoke(other.GetComponent<HitboxTrigger>());
        }
    }
}
