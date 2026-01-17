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

    float remainingDuration;
    void Awake()
    {
        nCol.SetActive(false);
        neCol.SetActive(false);
        eCol.SetActive(false);
        seCol.SetActive(false);
        sCol.SetActive(false);
        swCol.SetActive(false);
        wCol.SetActive(false);
        nwCol.SetActive(false);
    }
    void Update()
    {
        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0)
        {
            Destroy(this.gameObject);
        }
        this.gameObject.transform.position = abilityContext.originObject.transform.position;
    }

    override public void InitIndiv()
    {
        this.remainingDuration = duration;
        PlayerRequest rq = new PlayerRequest();
        rq.type = PlayerRequestType.LockVelocity;
        rq.duration = remainingDuration;
        abilityContext.playerController.AddRequest(rq);

        PlayerRequest rq2 = new PlayerRequest();
        rq2.type = PlayerRequestType.CancelHold;
        abilityContext.playerController.AddRequest(rq2);


        this.gameObject.transform.position = abilityContext.originObject.transform.position;
        switch (abilityContext.direction)
        {
            case AbilityDirection.North:
                nCol.SetActive(true);
                break;
            case AbilityDirection.NorthEast:
                neCol.SetActive(true);
                break;
            case AbilityDirection.East:
                eCol.SetActive(true);
                break;
            case AbilityDirection.SouthEast:
                seCol.SetActive(true);
                break;
            case AbilityDirection.South:
                sCol.SetActive(true);
                break;
            case AbilityDirection.SouthWest:
                swCol.SetActive(true);
                break;
            case AbilityDirection.West:
                wCol.SetActive(true);
                break;
            case AbilityDirection.NorthWest:
                nwCol.SetActive(true);
                break;
        }
    }
}
