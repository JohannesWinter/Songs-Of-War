using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public List<EntityController> entityControllers;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
public enum EntityMovementDirection
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
public enum SimpleEntityMovementDirection
{
    None,
    East,
    West,
}

public enum EntityRequestType
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
    LockUnstoppable,
    UnlockUnstoppable,
}
public struct EntityRequest
{
    public EntityRequestType type;
    public int priority; // 0 - irrelevent, 1 - low, 2 - normal, 3 - high, 4 - critical
    public Vector2 vector;
    public float duration;
    public float[] values;
}
