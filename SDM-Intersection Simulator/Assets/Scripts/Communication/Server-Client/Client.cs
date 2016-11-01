using UnityEngine;
using System.Collections;
using System;

public class Client : MonoBehaviour {

    private WebSocket w;

    IEnumerator Start()
    {
        w = new WebSocket(new Uri("ws://localhost:8080/Laputa"));
        yield return StartCoroutine(w.Connect());
        Send("Hi there");
        while (true)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                Debug.Log("Received: " + reply);
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
        if(Input.GetKeyDown(KeyCode.U))
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

    private void Send(string message)
    {
        if (w == null)
            return;
        w.SendString(message);
        Debug.Log("Sent " + message);
    }
}

