using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TrafficLaneData
{
    public string name;
    [HideInInspector]
    public int id;

    public Trafficlight trafficLight;
    public WaypointManager waypointManager;
    public Transform spawnLocation;

    public int NumberOfEntitiesInLane
    {
        get
        {
            return numberOfEntitiesInLane;
        }
        set
        {
            if (numberOfEntitiesInLane == value)
                return;

            numberOfEntitiesInLane = value;
        }
    }
    private int numberOfEntitiesInLane = 0;

    [HideInInspector]
    public List<WaypointAgent> waypointAgents = new List<WaypointAgent>();

    public TrafficLaneData(Trafficlight trafficLight)
    {
        id = trafficLight.Id;
        this.trafficLight = trafficLight;
    }

    public void AddWaypointAgent(WaypointAgent newAgent)
    {
        waypointAgents.Add(newAgent);
    }
}

public class TrafficManager : Singleton<TrafficManager>
{

    public List<TrafficLaneData> trafficLanes = new List<TrafficLaneData>();

    public void Start()
    {
        SetAllTrafficLights(Trafficlight.eTrafficState.RED);
    }

    private void SetAllTrafficLights(Trafficlight.eTrafficState newState)
    {
        for (int i = 0; i < trafficLanes.Count; i++)
        {
            Trafficlight trafficLight = trafficLanes[i].trafficLight;
            if (trafficLight == null) continue;

            SetTrafficLightState(trafficLight.Id, newState);
        }
    }

    /// <summary>
    /// Will do the lazy work for me in the editor :p
    /// </summary>
    public void OnValidate()
    {
        for (int i = 0; i < trafficLanes.Count; i++)
        {
            TrafficLaneData trafficLane = trafficLanes[i];
            if (trafficLane == null) continue;

            if (trafficLane.trafficLight == null) continue;

            trafficLane.id = trafficLane.trafficLight.Id;

            trafficLane.name = trafficLane.id + "(" + trafficLane.trafficLight.transform.parent.name + ")";
        }
    }

    /// <summary>
    /// Used for initializing entities on the map & configuring the correct lane data & Waypoint data.
    /// </summary>
    /// <param name="laneId"></param>
    /// <param name="objectToSpawn"></param>
    public void InitEntityAtLane(int laneId, GameObject objectToSpawn)
    {
        TrafficLaneData laneData = FindLaneDataById(laneId);

        Transform spawnLocation = laneData.spawnLocation;
        WaypointManager waypointManager = laneData.waypointManager;

        WaypointAgent waypointAgent = objectToSpawn.GetComponent<WaypointAgent>();

        if (waypointAgent == null)
        {
            Debug.LogError("Entity " + objectToSpawn + " is missing WaypointAgent component!");
            return;
        }

        if (spawnLocation == null)
        {
            Debug.LogError("No spawnlocation assigned for lane " + laneId);
            return;
        }

        if (waypointManager == null)
        {
            Debug.LogError("No waypointmanager assigned for lane " + laneId);
            return;
        }

        objectToSpawn.transform.position = spawnLocation.position;

        //Assign waypoint systems.
        waypointAgent.trafficLight = laneData.trafficLight;
        waypointAgent.WaypointSystem = laneData.waypointManager;

        waypointAgent.ResetWaypointTargetToFirst();

        //Notify the lane data
        laneData.AddWaypointAgent(waypointAgent);

        laneData.NumberOfEntitiesInLane++;
    }

    public void SetTrafficLightState(int id, Trafficlight.eTrafficState newTrafficLightState)
    {
        for (int i = 0; i < trafficLanes.Count; i++)
        {
            TrafficLaneData laneData = trafficLanes[i];

            if (laneData == null) continue;

            if (laneData.id != id) continue;

            Trafficlight trafficLight = laneData.trafficLight;

            trafficLight.TrafficState = newTrafficLightState;

            if (newTrafficLightState == Trafficlight.eTrafficState.GREEN || newTrafficLightState == Trafficlight.eTrafficState.ORANGE)
                for (int j = 0; j < laneData.waypointAgents.Count; j++)
                {
                    WaypointAgent agentInLane = laneData.waypointAgents[j];
                    agentInLane.WaypointSystemActivated = true;

                }

        }
    }

    public void DecreaseNumberOfCarsInLaneByOne(int laneId)
    {
        TrafficLaneData laneData = FindLaneDataById(laneId);

        if (laneData == null)
        {
            Debug.LogError("Failed to decrease total count of cars in lane " + laneId);
            return;
        }

        laneData.NumberOfEntitiesInLane--;
    }

    private TrafficLaneData FindLaneDataById(int id)
    {
        for (int i = 0; i < trafficLanes.Count; i++)
        {
            TrafficLaneData trafficLane = trafficLanes[i];

            if (trafficLane == null) continue;

            if (trafficLane.id == id)
                return trafficLane;
        }

        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SetAllTrafficLights(Trafficlight.eTrafficState.RED);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SetAllTrafficLights(Trafficlight.eTrafficState.ORANGE);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SetAllTrafficLights(Trafficlight.eTrafficState.GREEN);
        }

    }
}
