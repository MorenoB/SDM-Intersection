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
            waypointAgent.WaypointSystemActivated = false;
        }
    }

    private void OnTriggerLeave(Collider other)
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
