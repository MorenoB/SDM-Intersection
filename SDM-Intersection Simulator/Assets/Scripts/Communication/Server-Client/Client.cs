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
            arr.AddField("count", 0);
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
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    //Debug.Log(key);
                    DecodeJSON(j.ToString());
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    DecodeJSON(j.ToString());
                }
                break;
            case JSONObject.Type.STRING:
                //Debug.Log(obj.str);
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
                //Debug.Log(obj.n);

                lastNumber = (int)obj.n;

                break;
            case JSONObject.Type.BOOL:
                //Debug.Log(obj.b);
                break;
            case JSONObject.Type.NULL:
                //Debug.Log("NULL");
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

