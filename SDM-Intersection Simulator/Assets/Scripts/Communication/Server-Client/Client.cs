using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public List<Trafficlight> trafficData = new List<Trafficlight>();

    private WebSocket w;

    private int lastNumber = -1;

    IEnumerator Start()
    {
        w = new WebSocket(new Uri("ws://localhost:8080/Laputa"));
        yield return StartCoroutine(w.Connect());
        while (true)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                Debug.Log("Received: " + reply);
                DecodeJSON(reply);
            }
            if (w.error != null)
            {
                Debug.LogError("Error: " + w.error);
                break;
            }
            yield return 0;
        }
        w.Close();
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

        for (int i = 0; i < TrafficManager.Instance.trafficLanes.Count; i++)
        {
            TrafficLaneData laneData = TrafficManager.Instance.trafficLanes[i];

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
        if (w == null)
            return;
        w.SendString(message);
        Debug.Log("Sent " + message);
    }
}

