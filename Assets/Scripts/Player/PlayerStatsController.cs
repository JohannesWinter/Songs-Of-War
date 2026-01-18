using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    public int health;
    public int damage;

    List <PlayerStatsRequest> requests = new List <PlayerStatsRequest>();

    private void Update()
    {
        ProcessRequests();
    }
    public void AddRequest(PlayerStatsRequest request)
    {
        requests.Add(request);
    }
    void ProcessRequests()
    {
        //sort requests by priority
        requests.Sort((a, b) => a.priority.CompareTo(b.priority));

        //process requests with low -> high priority
        while (requests.Count > 0)
        {
            var rq = requests[0];
            requests.RemoveAt(0);

            switch (rq.type)
            {
                case PlayerStatsRequestType.SetHealth:
                    health = rq.intValue;
                    break;
                case PlayerStatsRequestType.AddHealth:
                    health += rq.intValue;
                    break;
                case PlayerStatsRequestType.SetDamage:
                    damage = rq.intValue; 
                    break;
            }
        }
    }

    public int GetHealth()
    {
        return health;
    }
    public int GetDamage()
    {
        return damage;
    }
}
