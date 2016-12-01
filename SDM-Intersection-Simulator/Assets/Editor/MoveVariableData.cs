using UnityEditor;
using UnityEngine;

public class MoveVariableData : EditorWindow
{
    [MenuItem("Window/MoveVariableData")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MoveVariableData));
    }

    void OnGUI()
    {
        if (GUILayout.Button("MoveData"))
        {
            /*TrafficManager[] dts = Resources.FindObjectsOfTypeAll<TrafficManager>();
            foreach (TrafficManager dt in dts)
            {
                //dt.carLanes = dt.trafficLanes;
            }*/
        }
    }
}