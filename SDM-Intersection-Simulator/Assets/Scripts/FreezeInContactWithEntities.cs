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
            WaypointAgent otherWaypointAgent = other.GetComponentInParent<WaypointAgent>();
            if(otherWaypointAgent == null)
            {
                Debug.LogError("Collidee has no waypoint agent component! collidee = " + other.name);
                return;
            }

            //Dont freeze the waypointagent if the collidee waypointagent is from an other lane.
            //if (otherWaypointAgent.AssignedTrafficLane != waypointAgent.AssignedTrafficLane)
            //    return;

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
            waypointAgent.WaypointSystemActivated = true;
        }
    }
}
