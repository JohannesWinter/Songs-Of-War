using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    public Transform playerAbilityFolder;
    public float hitCooldown;
    float currentHitCooldown;
    PlayerManager playerManager;
    AbilityDirection lastDirection = AbilityDirection.East;
    Ability currentAbility;



    private void Start()
    {
        playerManager = Manager.m.playerManager;
    }

    void Update()
    {
        UpdateLastDirection();
        CheckPlayerHit();
    }

    void UpdateLastDirection()
    {
        if (playerManager.right.press)
        {
            lastDirection = AbilityDirection.East;
        }
        if (playerManager.left.press)
        {
            lastDirection = AbilityDirection.West;
        }
    }

    void CheckPlayerHit()
    {
        currentHitCooldown -= Time.deltaTime;
        if (currentHitCooldown <= 0 && playerManager.hit.press && currentAbility == null)
        {
            currentHitCooldown = hitCooldown;

            bool up = playerManager.up.hold;
            bool down = playerManager.down.hold;
            bool left = playerManager.left.hold;
            bool right = playerManager.right.hold;

            AbilityDirection direction;

            if (up && right) direction = AbilityDirection.NorthEast;
            else if (up && left) direction = AbilityDirection.NorthWest;
            else if (down && right) direction = AbilityDirection.SouthEast;
            else if (down && left) direction = AbilityDirection.SouthWest;
            else if (up) direction = AbilityDirection.North;
            else if (down) direction = AbilityDirection.South;
            else if (right) direction = AbilityDirection.East;
            else if (left) direction = AbilityDirection.West;
            else direction = lastDirection;

            AbilityContext ctx = Instantiate(Manager.m.abilityManager.abilityContext);
            ctx.abilityDef = AbilityDef.Hit;
            ctx.direction = direction;
            ctx.origin = AbilityOrigin.Player;
            ctx.originObject = playerManager.playerController.playerObject;
            ctx.playerController = playerManager.playerController;

            (GameObject abilityObject, Ability ability) = Manager.m.abilityManager.RunAbility(ctx);
            abilityObject.transform.parent = playerAbilityFolder;
            currentAbility = ability;

        }
    }
}
