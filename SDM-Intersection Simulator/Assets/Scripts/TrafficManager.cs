using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficManager : Singleton<TrafficManager> {

    public List<Trafficlight> trafficLights = new List<Trafficlight>();

    public void SetTrafficLightState(string id, Trafficlight.eTrafficState newTrafficLightState)
    {
        for (int i = 0; i < trafficLights.Count; i++)
        {
            Trafficlight trafficLight = trafficLights[i];
            if (trafficLight == null) continue;

            if (trafficLight.Id != id) continue;

            trafficLight.TrafficState = newTrafficLightState;
        }
    }
}
