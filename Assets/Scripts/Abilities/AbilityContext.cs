using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityContext : MonoBehaviour
{
    public AbilityDef abilityDef;
    public AbilityOrigin origin;
    public GameObject originObject;
    public AbilityDirection direction;
    public PlayerController playerController;
    public EntityController entityController;
    public Ability ability;
    public int damage;
}