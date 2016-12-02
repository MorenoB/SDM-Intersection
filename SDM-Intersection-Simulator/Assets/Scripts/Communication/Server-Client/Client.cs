using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Client : Singleton<Client>
{
    public bool dontDestroyOnLoad;

    [Header("Configurable connection info")]
    public string address = "localhost";
    public int port = 8080;
    public string optionalWebhook = "Laputa";

    [Header("Optional configuration.")]
    public bool onlySendStatesWithACount;

    private WebSocket webSocket;

    private void Start()
    {
        if(dontDestroyOnLoad)
            DontDestroyOnLoad(this);
    }


    public void StartClient()
    {
        StartCoroutine(ClientLoop());
    }

    private IEnumerator ClientLoop()
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
        //Force sending of state data when key U is pressed.
        if (Input.GetKeyDown(KeyCode.U))
        {
            SendStateData();
        }
    }

    public void SendStateData()
    {
        if (webSocket == null)
        {
            Debug.LogError("Unable to send data, client was not set up or does not have an active connection!");
            return;
        }

        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);

        for (int i = 0; i < TrafficManager.Instance.Trafficlights.Count; i++)
        {
            Trafficlight trafficlight = TrafficManager.Instance.Trafficlights[i];

            //Ignore trafficlights who got count at 0 if 'onlySendStatesWithACount' is checked.
            if (onlySendStatesWithACount && trafficlight.WaitingAgents.Count < 1)
                continue;

            JSONObject arrayObject = new JSONObject();
            arrayObject.AddField("trafficLight", trafficlight.Id);
            arrayObject.AddField("count", trafficlight.WaitingAgents.Count);
            j.Add(arrayObject);
        }


        string encodedString = "{\"state\":" + j.Print() + "}";

        Send(encodedString);
    }

    private void HandleRecievedArrayObject(JSONObject jsonObj)
    {
        int trafficLightId = -1;
        string trafficState = "";

        if(!jsonObj.GetField(ref trafficLightId, "trafficLight"))
        {
            Debug.LogError("Unable to retrieve trafficLight id integer! data : " + jsonObj.ToString());
            return;
        }
        if (!jsonObj.GetField(ref trafficState, "status"))
        {
            Debug.LogError("Unable to retrieve status! data : " + jsonObj.ToString());
            return;
        }

        switch (trafficState)
        {
            case "green":
                TrafficManager.Instance.SetTrafficlightStateById(trafficLightId, Trafficlight.eTrafficState.GREEN);
                break;

            case "orange":
                TrafficManager.Instance.SetTrafficlightStateById(trafficLightId, Trafficlight.eTrafficState.ORANGE);
                break;

            case "yellow":
                Debug.LogWarning("Recieved 'Yellow' instead of 'Orange'!");
                TrafficManager.Instance.SetTrafficlightStateById(trafficLightId, Trafficlight.eTrafficState.ORANGE);
                break;

            case "red":
                TrafficManager.Instance.SetTrafficlightStateById(trafficLightId, Trafficlight.eTrafficState.RED);
                break;
        }
    }

    private void DecodeJSON(string incomingMsg)
    {
        JSONObject obj = new JSONObject(incomingMsg);

        for (int i = 0; i < obj.list.Count; i++)
        {
            List<JSONObject> arrayInsideJsonData = obj.list[i].list;
            for (int j = 0; j < arrayInsideJsonData.Count; j++)
            {
                JSONObject jsonTrafficlightDataString = arrayInsideJsonData[j];
                HandleRecievedArrayObject(jsonTrafficlightDataString);
            }
          
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

