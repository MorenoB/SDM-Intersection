using UnityEngine;

public class Trafficlight : MonoBehaviour {

    public string Id;
    public MeshRenderer headModel;
    public enum eTrafficState { NONE, GREEN, ORANGE, RED };

    private eTrafficState trafficState = eTrafficState.GREEN;
    public eTrafficState TrafficState
    {
        get { return TrafficState; }
        set
        {
            if (trafficState == value)
                return;
        }
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

    private void UpdateColor()
    {
        Color baseColor = Color.cyan;
        byte lightIndex = 0;

        switch(TrafficState)
        {
            case eTrafficState.GREEN:
                lightIndex = 4;
                break;

            case eTrafficState.ORANGE:
                lightIndex = 1;
                break;

            case eTrafficState.RED:
                lightIndex = 3;
                break;
        }

        for (int i = 0; i < headModel.materials.Length; i++)
        {
            float emission = 0;

            if (i == lightIndex)
                emission = 1;

            baseColor = GetColorByLightIndex(lightIndex);

            Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

            headModel.materials[i].SetColor("_EmissionColor", finalColor);

        }

    }
}
