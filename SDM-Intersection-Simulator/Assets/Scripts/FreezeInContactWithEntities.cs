using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FreezeInContactWithEntities : MonoBehaviour
{
    
    private WaypointAgent waypointAgent;

    public void OnEnable()
    {
        if (waypointAgent == null)
            waypointAgent = GetComponentInParent<WaypointAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (waypointAgent == null)
        {
            Debug.LogError("No waypoint agent is assigned to this object " + name);
            return;
        }
            
        
        if (other.CompareTag("MovingEntity"))
        {
            WaypointAgent otherAgent = other.GetComponentInParent<WaypointAgent>();

            if (otherAgent == null) return;

            if(waypointAgent.TrafficLaneId == otherAgent.TrafficLaneId)
                waypointAgent.WaypointSystemActivated = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (waypointAgent == null)
        {
            Debug.LogError("No waypoint agent is assigned to this object " + name);
            return;
        }

        if (other.CompareTag("MovingEntity"))
        {
            WaypointAgent otherAgent = other.GetComponentInParent<WaypointAgent>();

            if (otherAgent == null) return;

            if (waypointAgent.TrafficLaneId == otherAgent.TrafficLaneId)
                waypointAgent.WaypointSystemActivated = true;
        }
    }
}
