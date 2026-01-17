using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public AbilityContext abilityContext;

    public void Init(AbilityContext ctx)
    {
        this.abilityContext = ctx;
        InitIndiv();
    }
    public abstract void InitIndiv();
}
