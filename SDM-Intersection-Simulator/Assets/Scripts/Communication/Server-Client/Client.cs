using UnityEngine;
using System.Collections;
using System;

public class Client : MonoBehaviour
{
    public string address = "localhost";
    public int port = 8080;
    public string optionalWebhook = "Laputa";

    private WebSocket webSocket;
    private int lastNumber = -1;

    IEnumerator Start()
    {
        webSocket = new WebSocket(new Uri("ws://" + address + ":" + port + "/" + optionalWebhook));
        yield return StartCoroutine(webSocket.Connect());
        while (true)
        {
            string reply = webSocket.RecvString();
            if (reply != null)
            {
                Debug.Log("Received: " + reply);
                DecodeJSON(reply);
            }
            if (webSocket.error != null)
            {
                Debug.LogError("Error: " + webSocket.error);
                break;
            }
            yield return null;
        }
        webSocket.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SendStateData();
        }
    }

    public void SendStateData()
    {

        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        //array
        JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
        j.Add(arr);

        for (int i = 0; i < TrafficManager.Instance.TrafficLanes.Count; i++)
        {
            TrafficLaneData laneData = TrafficManager.Instance.TrafficLanes[i];

            arr.AddField("trafficLight", laneData.id);
            arr.AddField("count", laneData.NumberOfEntitiesInLane);
        }


        string encodedString = "{\"state\":" + j.Print() + "}";

        Send(encodedString);
    }

    private void DecodeJSON(string incomingMsg)
    {
        JSONObject obj = new JSONObject(incomingMsg);
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    JSONObject j = obj.list[i];
                    DecodeJSON(j.ToString());
                }
                break;
            case JSONObject.Type.ARRAY:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    JSONObject j = obj.list[i];
                    DecodeJSON(j.ToString());
                }
                break;
            case JSONObject.Type.STRING:
                switch (obj.str)
                {
                    case "green":
                        TrafficManager.Instance.SetTrafficLightState(lastNumber, Trafficlight.eTrafficState.GREEN);
                        break;

                    case "orange":
                        TrafficManager.Instance.SetTrafficLightState(lastNumber, Trafficlight.eTrafficState.ORANGE);
                        break;

                    case "yellow":
                        Debug.LogWarning("Recieved 'Yellow' instead of 'Orange'!");
                        TrafficManager.Instance.SetTrafficLightState(lastNumber, Trafficlight.eTrafficState.ORANGE);
                        break;

                    case "red":
                        TrafficManager.Instance.SetTrafficLightState(lastNumber, Trafficlight.eTrafficState.RED);
                        break;
                }
                break;
            case JSONObject.Type.NUMBER:

                lastNumber = (int)obj.n;

                break;
            case JSONObject.Type.BOOL:
                break;
            case JSONObject.Type.NULL:
                break;

        }
    }

    private void Send(string message)
    {
        if (webSocket == null)
            return;
        webSocket.SendString(message);
        Debug.Log("Sent " + message);
    }
}

