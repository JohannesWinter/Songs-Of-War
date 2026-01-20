using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : Ability
{
    public GameObject nCol;
    public GameObject eCol;
    public GameObject sCol;
    public GameObject wCol;
    public GameObject neCol;
    public GameObject nwCol;
    public GameObject seCol;
    public GameObject swCol;

    public float duration;
    public float knockBack;
    bool hitSomething;

    float remainingDuration;
    void Awake()
    {
        nCol.GetComponent<Collider2D>().enabled = false;
        neCol.GetComponent<Collider2D>().enabled = false;
        eCol.GetComponent<Collider2D>().enabled = false;
        seCol.GetComponent<Collider2D>().enabled = false;
        sCol.GetComponent<Collider2D>().enabled = false;
        swCol.GetComponent<Collider2D>().enabled = false;
        wCol.GetComponent<Collider2D>().enabled = false;
        nwCol.GetComponent<Collider2D>().enabled = false;

        nCol.GetComponent<SpriteRenderer>().enabled = false;
        neCol.GetComponent<SpriteRenderer>().enabled = false;
        eCol.GetComponent<SpriteRenderer>().enabled = false;
        seCol.GetComponent<SpriteRenderer>().enabled = false;
        sCol.GetComponent<SpriteRenderer>().enabled = false;
        swCol.GetComponent<SpriteRenderer>().enabled = false;
        wCol.GetComponent<SpriteRenderer>().enabled = false;
        nwCol.GetComponent<SpriteRenderer>().enabled = false;
    }   
    void Update()
    {
        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0)
        {
            Destroy(this.gameObject);
        }
        if (hitSomething == false)
        {
            this.gameObject.transform.position = abilityContext.originObject.transform.position;
        }
    }

    override public void InitIndiv()
    {
        this.remainingDuration = duration;
        PlayerMovementRequest rq = new PlayerMovementRequest();
        rq.type = PlayerMovementRequestType.LockVelocity;
        rq.duration = remainingDuration;
        rq.priority = 1;
        abilityContext.playerController.AddRequest(rq);

        PlayerMovementRequest rq2 = new PlayerMovementRequest();
        rq2.type = PlayerMovementRequestType.CancelHold;
        abilityContext.playerController.AddRequest(rq2);


        this.gameObject.transform.position = abilityContext.originObject.transform.position;
        switch (abilityContext.direction)
        {
            case AbilityDirection.North:
                nCol.GetComponent<Collider2D>().enabled = true;
                nCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case AbilityDirection.NorthEast:
                neCol.GetComponent<Collider2D>().enabled = true;
                neCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case AbilityDirection.East:
                eCol.GetComponent<Collider2D>().enabled = true;
                eCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case AbilityDirection.SouthEast:
                seCol.GetComponent<Collider2D>().enabled = true;
                seCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case AbilityDirection.South:
                sCol.GetComponent<Collider2D>().enabled = true;
                sCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case AbilityDirection.SouthWest:
                swCol.GetComponent<Collider2D>().enabled = true;
                swCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case AbilityDirection.West:
                wCol.GetComponent<Collider2D>().enabled = true;
                wCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case AbilityDirection.NorthWest:
                nwCol.GetComponent<Collider2D>().enabled = true;
                nwCol.GetComponent<SpriteRenderer>().enabled = true;
                break;
        }
    }

    override public void HandleHit(HitboxTrigger other)
    {
        HitboxContext hBC = other.hitboxContext;
        if (hBC == null) return;
        if (abilityContext.direction == AbilityDirection.South ||
        abilityContext.direction == AbilityDirection.SouthEast ||
        abilityContext.direction == AbilityDirection.SouthWest)
        {
            switch (hBC.hitboxHolder)
            {
                case HitboxHolder.Entity:
                case HitboxHolder.Enviroment:
                    PlayerMovementRequest rq = new PlayerMovementRequest();
                    rq.type = PlayerMovementRequestType.UnlockVelocity;
                    rq.priority = 2;
                    abilityContext.playerController.AddRequest(rq);

                    PlayerMovementRequest rq3 = new PlayerMovementRequest();
                    rq3.type = PlayerMovementRequestType.DisableJumpInterrupt;
                    abilityContext.playerController.AddRequest(rq3);

                    PlayerMovementRequest rq2 = new PlayerMovementRequest();
                    rq2.type = PlayerMovementRequestType.SetVelocity;
                    rq2.vector = Vector2.up * 12;
                    rq2.priority = 2;
                    if (abilityContext.direction == AbilityDirection.SouthEast)
                    {
                        rq2.vector += Vector2.left * 10;
                    }
                    else if (abilityContext.direction == AbilityDirection.SouthWest)
                    {
                        rq2.vector += Vector2.right * 10;
                    }
                    abilityContext.playerController.AddRequest(rq2);

                    PlayerMovementRequest rq5 = new PlayerMovementRequest();
                    rq5.type = PlayerMovementRequestType.LockMovement;
                    rq5.duration = 0.1f;
                    abilityContext.playerController.AddRequest(rq5);
                    break;
            }
        }
        hitSomething = true;
    }
}
