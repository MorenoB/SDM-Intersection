using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TrafficLaneData
{
    public string name;
    public int id;

    public List<Trafficlight> trafficLights = new List<Trafficlight>();
    public List<WaypointManager> waypointManagers = new List<WaypointManager>();
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

    public void AddWaypointAgent(WaypointAgent newAgent)
    {
        waypointAgents.Add(newAgent);
    }

    public Trafficlight GetAvailableTrafficLight()
    {
        if (trafficLights.Count < 1)
        {
            Debug.LogError("No trafficlights assigned to lane " + id);
            return null;
        }

        for (int i = 0; i < trafficLights.Count; i++)
        {
            if (trafficLights[i] != null) return trafficLights[i];
        }

        Debug.LogError("No trafficlights assigned to lane " + id);
        return null;
    }

    public WaypointManager GetRandomWaypointManager()
    {
        if(waypointManagers.Count < 1)
        {
            Debug.LogError("No waypointmanagers assigned to lane " + id);
            return null;
        }

        int randomWaypointmanagerIndex = Random.Range(0, waypointManagers.Count);

        return waypointManagers[randomWaypointmanagerIndex];
    }
}

public class TrafficManager : Singleton<TrafficManager>
{

    public List<TrafficLaneData> trafficLanes = new List<TrafficLaneData>();


    private Client client;

    public void Start()
    {
        client = GetComponent<Client>();
        SetAllTrafficLights(Trafficlight.eTrafficState.RED);
    }

    private void SetAllTrafficLights(Trafficlight.eTrafficState newState)
    {
        for (int i = 0; i < trafficLanes.Count; i++)
        {
            for (int j = 0; j < trafficLanes[i].trafficLights.Count; j++)
            {
                Trafficlight trafficLight = trafficLanes[i].trafficLights[j];
                if (trafficLight == null) continue;

                SetTrafficLightState(trafficLight.Id, newState);
            }

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

            if (trafficLane.trafficLights.Count < 1) return;

            if (trafficLane.trafficLights[0] == null) continue;

            trafficLane.name = trafficLane.id + "(" + trafficLane.trafficLights[0].transform.parent.name + ")";
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

        if(laneData == null)
        {
            Debug.LogError("Lane id " + laneId + " not found!");
            return;
        }

        Transform spawnLocation = laneData.spawnLocation;

        objectToSpawn.transform.rotation = spawnLocation.rotation;

        WaypointManager waypointManager = laneData.GetRandomWaypointManager();

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
        Trafficlight trafficLight = laneData.GetAvailableTrafficLight();

        if (trafficLight == null) return;

        waypointAgent.trafficLight = trafficLight;

        waypointAgent.WaypointSystem = waypointManager;

        waypointAgent.ResetWaypointTargetToFirst();

        //Notify the lane data
        laneData.AddWaypointAgent(waypointAgent);

        laneData.NumberOfEntitiesInLane++;

        //Send update to controller
        client.SendStateData();
    }

    public void SetTrafficLightState(int id, Trafficlight.eTrafficState newTrafficLightState)
    {
        for (int i = 0; i < trafficLanes.Count; i++)
        {
            TrafficLaneData laneData = trafficLanes[i];

            if (laneData == null) continue;

            for (int j = 0; j < laneData.trafficLights.Count; j++)
            {
                Trafficlight trafficLight = laneData.trafficLights[j];

                if (trafficLight.Id != id) continue;

                trafficLight.TrafficState = newTrafficLightState;
            }


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
