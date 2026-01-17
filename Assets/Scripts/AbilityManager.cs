using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public GameObject[] abilityObjects;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public (GameObject, Ability) GetAbility(AbilitiesDef ability)
    {
        switch (ability)
        {
            case AbilitiesDef.Hit:
                GameObject loaded = Instantiate(abilityObjects[0]);
                return (loaded, loaded.GetComponent<Ability>());
        }
        Debug.LogError("Did not find Ability <" + ability + ">");
        return (null, null);
    }
}
public enum AbilitiesDef
{
    Hit,
}
