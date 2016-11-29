﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TrafficLaneData
{
    public string name;
    public int id;

    public List<Trafficlight> trafficLights = new List<Trafficlight>();
    public List<WaypointManager> waypointManagers = new List<WaypointManager>();

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
    public List<TrafficLaneData> carLanes = new List<TrafficLaneData>();
    public List<TrafficLaneData> bicycleLanes = new List<TrafficLaneData>();
    public List<TrafficLaneData> busLanes = new List<TrafficLaneData>();
    public List<TrafficLaneData> trainLanes = new List<TrafficLaneData>();
    public List<TrafficLaneData> pedestrianLanes = new List<TrafficLaneData>();

    private List<Trafficlight> trafficLights = new List<Trafficlight>();
    public List<Trafficlight> Trafficlights
    {
        get
        {
            return trafficLights;
        }
    }


    private List<TrafficLaneData> cachedTrafficLanes = new List<TrafficLaneData>();
    public List<TrafficLaneData> TrafficLanes
    {
        get
        {
            return cachedTrafficLanes;

        }
    }

    private Client client;

    public void Start()
    {
        PopulateTrafficLanes();

        trafficLights = FindObjectsOfType<Trafficlight>().ToList();

        client = GetComponent<Client>();
        SetAllTrafficLights(Trafficlight.eTrafficState.RED);
    }

    private void PopulateTrafficLanes()
    {
        cachedTrafficLanes.AddRange(carLanes);
        cachedTrafficLanes.AddRange(bicycleLanes);
        cachedTrafficLanes.AddRange(busLanes);
        cachedTrafficLanes.AddRange(trainLanes);
        cachedTrafficLanes.AddRange(pedestrianLanes);
    }

    private void SetAllTrafficLights(Trafficlight.eTrafficState newState)
    {
        for (int i = 0; i < TrafficLanes.Count; i++)
        {
            for (int j = 0; j < TrafficLanes[i].trafficLights.Count; j++)
            {
                Trafficlight trafficLight = TrafficLanes[i].trafficLights[j];
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
        for (int i = 0; i < TrafficLanes.Count; i++)
        {
            TrafficLaneData trafficLane = TrafficLanes[i];
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

        WaypointManager waypointManager = laneData.GetRandomWaypointManager();

        if (waypointManager == null)
        {
            Debug.LogError("No waypointmanager assigned for lane " + laneId);
            return;
        }
        

        if (waypointManager.waypointNodes.Count < 1)
        {
            Debug.LogError("No waypoint nodes set!");
            return;
        }

        Transform spawnLocation = waypointManager.waypointNodes[0].transform;

        objectToSpawn.transform.rotation = spawnLocation.rotation;

        WaypointAgent waypointAgent = objectToSpawn.GetComponent<WaypointAgent>();

        if (waypointAgent == null)
        {
            Debug.LogError("Entity " + objectToSpawn + " is missing WaypointAgent component!");
            return;
        }

        objectToSpawn.transform.position = spawnLocation.position;

        //Assign waypoint systems.
        waypointAgent.WaypointSystem = waypointManager;

        waypointAgent.ResetWaypointTargetToFirst();

        //Notify the lane data
        laneData.AddWaypointAgent(waypointAgent);

        laneData.NumberOfEntitiesInLane++;

        //Add updated lane data object to the waypointagent
        waypointAgent.TrafficLaneId = laneData.id;

        for (int i = 0; i < laneData.trafficLights.Count; i++)
        {
            waypointAgent.TrafficlightQueue.Enqueue(laneData.trafficLights[i]);
        }

        waypointAgent.AssignNextTrafficlight();

        waypointAgent.WaypointSystemActivated = true;


        //Send update to controller
        client.SendStateData();
    }

    public void SetTrafficLightState(int id, Trafficlight.eTrafficState newTrafficLightState)
    {
        for (int i = 0; i < Trafficlights.Count; i++)
        {
            Trafficlight trafficLight = Trafficlights[i];

            if (trafficLight == null) continue;

            if (trafficLight.Id != id) continue;

            trafficLight.TrafficState = newTrafficLightState;
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

    public TrafficLaneData GetLane(SpawnManager.SpawnType entityType, int maxNrOfEnttitiesInLane = -1)
    {
        int randomIndex = 0;
        TrafficLaneData trafficLane = null;

        switch(entityType)
        {
            case SpawnManager.SpawnType.BICYCLE:
                randomIndex = Random.Range(0, bicycleLanes.Count);
                trafficLane = bicycleLanes[randomIndex];
                break;

            case SpawnManager.SpawnType.BUS:
                randomIndex = Random.Range(0, busLanes.Count);
                trafficLane = busLanes[randomIndex];
                break;

            case SpawnManager.SpawnType.CAR:
                randomIndex = Random.Range(0, carLanes.Count);
                trafficLane = carLanes[randomIndex];
                break;

            case SpawnManager.SpawnType.TRAIN:
                randomIndex = Random.Range(0, trainLanes.Count);
                trafficLane = trainLanes[randomIndex];

                break;

            case SpawnManager.SpawnType.PEDESTRIAN:
                randomIndex = Random.Range(0, pedestrianLanes.Count);
                trafficLane = pedestrianLanes[randomIndex];

                break;

            default:
                Debug.LogError("Spawntype " + entityType +  " not recognized!");

                break;
        }

        if (maxNrOfEnttitiesInLane == -1)
            return trafficLane;

        if (trafficLane.NumberOfEntitiesInLane >= maxNrOfEnttitiesInLane)
            return null;

        return trafficLane;
    }

    public TrafficLaneData FindLaneDataById(int id)
    {
        for (int i = 0; i < TrafficLanes.Count; i++)
        {
            TrafficLaneData trafficLane = TrafficLanes[i];

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
