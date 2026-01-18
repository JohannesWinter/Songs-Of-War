using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    public Action<HitboxTrigger> onHit;
    public HitboxContext hitboxContext;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == this.gameObject.layer)
        {
            onHit?.Invoke(other.GetComponent<HitboxTrigger>());
        }
    }
}
