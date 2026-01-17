using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerAbilityController playerAbilityController;

    public InputState right;
    public InputState left;
    public InputState up;
    public InputState down;
    public InputState jump;
    public InputState hit;
    public InputState ability1;
    public InputState ability2;
    public InputState ability3;
    public InputState ability4;

    private void Update()
    {
        UpdatePublicPlayerInputData();
    }

    public Vector2 getPlayerPosition()
    {
        return playerController.playerObject.transform.position;
    }
    public Vector2 getPlayerScale()
    {
        return playerController.playerObject.transform.localScale;
    }
    public Vector2 getPlayerVelocity()
    {
        return playerController.velocity;
    }

    void UpdatePublicPlayerInputData()
    {
        // RIGHT
        right.hold = GameInputManager.GetManagerKey("Right");
        right.press = GameInputManager.GetManagerKeyDown("Right");
        right.release = GameInputManager.GetManagerKeyUp("Right");

        // LEFT
        left.hold = GameInputManager.GetManagerKey("Left");
        left.press = GameInputManager.GetManagerKeyDown("Left");
        left.release = GameInputManager.GetManagerKeyUp("Left");

        // UP
        up.hold = GameInputManager.GetManagerKey("Up");
        up.press = GameInputManager.GetManagerKeyDown("Up");
        up.release = GameInputManager.GetManagerKeyUp("Up");

        // DOWN
        down.hold = GameInputManager.GetManagerKey("Down");
        down.press = GameInputManager.GetManagerKeyDown("Down");
        down.release = GameInputManager.GetManagerKeyUp("Down");

        // JUMP
        jump.hold = GameInputManager.GetManagerKey("Jump");
        jump.press = GameInputManager.GetManagerKeyDown("Jump");
        jump.release = GameInputManager.GetManagerKeyUp("Jump");

        // HIT
        hit.hold = GameInputManager.GetManagerKey("Hit");
        hit.press = GameInputManager.GetManagerKeyDown("Hit");
        hit.release = GameInputManager.GetManagerKeyUp("Hit");

        // ABILITIES
        ability1.hold = GameInputManager.GetManagerKey("Ability1");
        ability1.press = GameInputManager.GetManagerKeyDown("Ability1");
        ability1.release = GameInputManager.GetManagerKeyUp("Ability1");

        ability2.hold = GameInputManager.GetManagerKey("Ability2");
        ability2.press = GameInputManager.GetManagerKeyDown("Ability2");
        ability2.release = GameInputManager.GetManagerKeyUp("Ability2");

        ability3.hold = GameInputManager.GetManagerKey("Ability3");
        ability3.press = GameInputManager.GetManagerKeyDown("Ability3");
        ability3.release = GameInputManager.GetManagerKeyUp("Ability3");

        ability4.hold = GameInputManager.GetManagerKey("Ability4");
        ability4.press = GameInputManager.GetManagerKeyDown("Ability4");
        ability4.release = GameInputManager.GetManagerKeyUp("Ability4");
    }
}

[System.Serializable]
public struct InputState
{
    public bool hold;
    public bool press;
    public bool release;
}
public enum PlayerMovementDirection
{
    None,
    Right,
    Left,
}
public enum PlayerRequestType
{
    AddVelocity,
    SetVelocity,
    SetPosition,
    AddPosition,
    LockMovement,
    UnlockMovement,
    LockGravity,
    UnlockGravity,
    LockVelocity,
    UnlockVelocity,
    OverrideGravity,
    CancelHold,
}
public struct PlayerRequest
{
    public PlayerRequestType type;
    public int priority; // 0 - irrelevent, 1 - low, 2 - normal, 3 - high, 4 - critical
    public Vector2 vector;
    public float duration;
    public float[] values;
}

public class PlayerRequestTimer
{
    public float remaining;
    public int priority;
}