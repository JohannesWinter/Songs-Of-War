using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class AbilityManager : MonoBehaviour
{
    public GameObject[] abilityObjects;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public (GameObject, Ability) RunAbility(AbilityContext ctx)
    {
        GameObject toLoad = GetAbilityObject(ctx.abilityDef);
        if (toLoad != null)
        {
            GameObject loaded = Instantiate(abilityObjects[0]);
            loaded.GetComponent<Ability>().Init(ctx);
            return (loaded, loaded.GetComponent<Ability>());
        }
        Debug.LogError("Did not find Ability <" + ctx.abilityDef + ">");
        return (null, null);
    }

    GameObject GetAbilityObject(AbilityDef abilityDef)
    {
        switch (abilityDef)
        {
            case AbilityDef.Hit:
                return abilityObjects[0];
            default:
                return null;
        }
    }
}
public enum AbilityDef
{
    Hit,
}
public enum AbilityType
{
    None,
    Red,
    Yellow,
    Blue,
    Green
}
public enum AbilityOrigin
{
    Player,
    Mob,
    Enviroment
}

public enum AbilityDirection
{
    None,
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest,
}
