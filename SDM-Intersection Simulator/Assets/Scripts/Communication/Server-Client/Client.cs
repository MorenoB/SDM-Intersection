using UnityEngine;
using System.Collections;
using System;

public class Client : MonoBehaviour {

    WebSocket w;
    IEnumerator Start()
    {
        w = new WebSocket(new Uri("ws://localhost:8080/Laputa"));
        yield return StartCoroutine(w.Connect());
        Send("Hi there");
        int i = 0;
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
    { }

    private void Send(string message)
    {
        if (w == null)
            return;
        w.SendString(message);
        Debug.Log("Sent " + message);
    }
}

