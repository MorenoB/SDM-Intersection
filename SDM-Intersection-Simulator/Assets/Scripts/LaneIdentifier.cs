using System.Collections.Generic;
using UnityEngine;

public class LaneIdentifier : MonoBehaviour {

    public int Id { get; set; }

    private List<WaypointAgent> registeredWaypointAgents = new List<WaypointAgent>();
    public List<WaypointAgent> FrozenAgents
    {
        get
        {
            return registeredWaypointAgents;
        }
        set
        {
            if (value != registeredWaypointAgents)
                registeredWaypointAgents = value;
        }
    }

    private void OnDisable()
    {
        if (FrozenAgents.Count < 1)
            return;

        for (int i = 0; i < FrozenAgents.Count; i++)
        {
            FrozenAgents[i].WaypointSystemActivated = true;
        }
    }

}
