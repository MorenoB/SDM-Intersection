using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TrafficLaneData
{
    public string id;
    public Trafficlight trafficLight;
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

public class TrafficManager : Singleton<TrafficManager> {

    [HideInInspector] [SerializeField] public List<TrafficLaneData> trafficLanes = new List<TrafficLaneData>();

    public List<Trafficlight> trafficLights = new List<Trafficlight>();

    public void Awake()
    {
        for (int i = 0; i < trafficLights.Count; i++)
        {
            Trafficlight trafficLight = trafficLights[i];
            if (trafficLight == null) continue;

            TrafficLaneData newLaneData = new TrafficLaneData(trafficLight);

            trafficLanes.Add(newLaneData);
        }

        return;
    }

    public void Start()
    {
        for (int i = 0; i < trafficLights.Count; i++)
        {
            Trafficlight trafficLight = trafficLights[i];
            if (trafficLight == null) continue;

            SetTrafficLightState(trafficLight.Id, Trafficlight.eTrafficState.GREEN);
        }
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

            if(newTrafficLightState == Trafficlight.eTrafficState.GREEN || newTrafficLightState == Trafficlight.eTrafficState.ORANGE)
                for (int j = 0; j < laneData.waypointAgents.Count; j++)
                {
                    WaypointAgent agentInLane = laneData.waypointAgents[j];
                    agentInLane.WaypointSystemActivated = true;

                }
            
        }
    }
}
