using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrafficLightStateObject
{
    public GameObject gameObject;
    public Trafficlight.eTrafficState trafficState = Trafficlight.eTrafficState.NONE;
}

public class Trafficlight : MonoBehaviour {

    public int Id;
    public MeshRenderer headModel;
    public enum eTrafficState { NONE, GREEN, ORANGE, RED };

    public LaneIdentifier stoplineCollider;
    public LaneIdentifier hasLeftLaneCollider;

    public bool useGameObjectSwitching = false;

    public List<TrafficLightStateObject> LightStates = new List<TrafficLightStateObject>();

    public List<WaypointAgent> WaitingAgents = new List<WaypointAgent>();

    private eTrafficState trafficState = eTrafficState.NONE;
    public eTrafficState TrafficState
    {
        get { return trafficState; }
        set
        {
            if (trafficState == value)
                return;

            trafficState = value;

            UpdateColor();
        }
    }

    private void Start()
    {
        if(hasLeftLaneCollider != null)
            hasLeftLaneCollider.Id = Id;

        if (stoplineCollider != null)
            stoplineCollider.Id = Id;
    }

    private Color GetColorByLightIndex(byte lightIndex)
    {
        Color ret = Color.cyan;
        switch(lightIndex)
        {
            case 1:
                ret = Color.yellow;
                break;

            case 3:
                ret = Color.red;
                break;

            case 4:
                ret = Color.green;
                break;
        }

        return ret;
    }

    private void EnableLightStateObject(eTrafficState trafficState)
    {
        for (int i = 0; i < LightStates.Count; i++)
        {
            TrafficLightStateObject obj = LightStates[i];
            if (obj == null) continue;

            if (obj.trafficState == trafficState)
            {
                if (!obj.gameObject.activeSelf)
                    obj.gameObject.SetActive(true);

                continue;
            }

            if(obj.gameObject.activeSelf)
                obj.gameObject.SetActive(false);
        }
    } 

    private void UpdateColor()
    {
        Color baseColor = Color.cyan;
        byte lightIndex = 0;

        switch(TrafficState)
        {
            case eTrafficState.GREEN:
                lightIndex = 4;

                if (stoplineCollider != null)
                    stoplineCollider.gameObject.SetActive(false);

                break;

            case eTrafficState.ORANGE:
                lightIndex = 1;

                if (stoplineCollider != null)
                    stoplineCollider.gameObject.SetActive(false);

                break;

            case eTrafficState.RED:
                lightIndex = 3;

                if(stoplineCollider != null)
                    stoplineCollider.gameObject.SetActive(true);

                break;
        }


        //If property is checked, only switch states by activating or deactivating gameobjects rather than modifying materials.
        if(useGameObjectSwitching)
        {
            EnableLightStateObject(TrafficState);
            return;
        }

        if(headModel == null)
        {
            Debug.LogError("No headmodel set on this object while modifying material!");
            return;
        }

        for (int i = 0; i < headModel.materials.Length; i++)
        {
            float emission = 0;

            if (i != 1 && i != 3 && i != 4) continue;

            if (i == lightIndex)
                emission = 1;

            baseColor = GetColorByLightIndex(lightIndex);

            Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

            headModel.materials[i].SetColor("_EmissionColor", finalColor);
           
            headModel.materials[i].EnableKeyword("_EMISSION");
        }

    }
}
