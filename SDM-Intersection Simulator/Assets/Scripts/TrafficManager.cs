using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TrafficLaneData
{
    public string name;
    [HideInInspector] public string id;

    public Trafficlight trafficLight;
    public WaypointManager waypointManager;
    public Transform spawnLocation;

    [HideInInspector] public List<WaypointAgent> waypointAgents = new List<WaypointAgent>();

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

public class TrafficManager : Singleton<TrafficManager> {

    public List<TrafficLaneData> trafficLanes = new List<TrafficLaneData>();

    public Transform carHierarchyParent;

    public GameObject testPrefab;

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

    public void OnValidate()
    {
        for (int i = 0; i < trafficLanes.Count; i++)
        {
            TrafficLaneData trafficLane = trafficLanes[i];
            if (trafficLane == null) continue;

            if (trafficLane.id == null) continue;

            if (trafficLane.trafficLight == null) continue;

            trafficLane.id = trafficLane.trafficLight.Id;

            trafficLane.name = trafficLane.id + "(" + trafficLane.trafficLight.transform.parent.name + ")";
        }
    }

    /// <summary>
    /// Used for spawning entities on the map & configuring the correct lane data & Waypoint data.
    /// NOTE:: NEEDS TO MAKE USE OF OBJECT POOLING LATER!
    /// </summary>
    /// <param name="laneId"></param>
    /// <param name="objectToSpawn"></param>
    public void SpawnEntityAtLane(string laneId, GameObject objectToSpawn)
    {
        TrafficLaneData laneData = FindLaneDataById(laneId);

        Transform spawnLocation = laneData.spawnLocation;
        WaypointManager waypointManager = laneData.waypointManager;

        WaypointAgent waypointAgent = objectToSpawn.GetComponent<WaypointAgent>();

        if(waypointAgent == null)
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

        //Assign waypoint systems.
        waypointAgent.trafficLight = laneData.trafficLight;
        waypointAgent.WaypointSystem = laneData.waypointManager;

        

        // TODO: Make use of object pooling instead of instantiating!
        GameObject instatiatedObject = (GameObject)Instantiate(objectToSpawn, spawnLocation.position, Quaternion.identity, carHierarchyParent);

        laneData.AddWaypointAgent(instatiatedObject.GetComponent<WaypointAgent>());
    }

    public void SetTrafficLightState(string id, Trafficlight.eTrafficState newTrafficLightState)
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

    private TrafficLaneData FindLaneDataById(string id)
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
        if(Input.GetKeyDown(KeyCode.T))
        {
            SpawnEntityAtLane("N4", testPrefab);
        }

        if(Input.GetKeyDown(KeyCode.F5))
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
