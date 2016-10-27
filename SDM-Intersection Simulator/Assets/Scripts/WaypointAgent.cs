using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

[System.Serializable]
public class WaypointAgent : MonoBehaviour {


    //New
    public bool randomizeExactTarget = false;

    public Trafficlight trafficLight;

    private CarController carController;
    private bool hasCollidedWithStopLine = false;
    private bool waypointSystemActivated = true;

    ///
    [SerializeField] protected float minAgentSpeed = 10;
    [SerializeField] protected float maxAgentSpeed = 20;
    [SerializeField] protected WaypointManager m_waypointManager;

	[HideInInspector] protected int currentIndex = 0;
    [HideInInspector] public GameObject currentTarget;
    [HideInInspector] public Vector3 currentNodeTarget = Vector3.zero;
	[HideInInspector] public bool waypointUpdatingEntity = false;

	protected float speed = 10;
    protected Vector3 directionVector = new Vector3(0,1,0);
    protected float m_nodeProximityDistance = 0.1f;
    protected float m_slerpRotationSpeed = 0.1f;
    protected WaypointRotationMode m_waypointRotationMode;

    public bool WaypointSystemActivated
    {
        get
        {
            return waypointSystemActivated;
        }
        set
        {
            if (waypointSystemActivated == value)
                return;

            waypointSystemActivated = value;
        }
    }

    public WaypointManager WaypointSystem {  set { m_waypointManager = value; } }
    public WaypointRotationMode WaypointRotation { set { m_waypointRotationMode = value; } }
    public float SlerpSpeed { set { m_slerpRotationSpeed = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float NodeProximityDistance { set { m_nodeProximityDistance = value; } }
    public int CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }

    public virtual void Start()
    {
        speed = Random.Range(minAgentSpeed, maxAgentSpeed);
        carController = GetComponent<CarController>();

        currentNodeTarget = m_waypointManager.GetNodePosition(currentIndex);
    }

    public virtual void Update()
    {
        if (m_waypointManager == null) return;

        if(WaypointSystemActivated)
            WaypointMovementUpdate();
    }

    protected IEnumerator DieAnimDelay()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    protected IEnumerator DieWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

	public virtual void SwitchTarget(GameObject newTarget)
	{
		currentTarget = newTarget;
	}

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(currentNodeTarget, 0.15f);
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.position, currentNodeTarget);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("StopLine"))
        {
            carController.Move(0, 0, 1, 1);
            WaypointSystemActivated = false;
        }
    }

    protected void WaypointMovementUpdate()
    {
        // If the agent has a gameobject target assigned then move towards it otherwise 
        // get a target position in 3d sapce and move torawrds that
        if (currentTarget == null)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(currentNodeTarget);

            float steerAngle = relativeVector.x / relativeVector.magnitude;
            float speed = relativeVector.z / relativeVector.magnitude;
            float brakeforce = 0;

            if (carController.CurrentSpeed > 0 && speed < 0)
            {
                brakeforce = speed;
            }

            carController.Move(steerAngle, speed, brakeforce, 0);

            if (m_waypointManager.ObjectIsOnNode(this))
            {

                currentIndex++;

                if (currentIndex >= m_waypointManager.NodeQuantity)
                {
                    if (!m_waypointManager.looping)
                    {
                        m_waypointManager.RemoveEntity(this);
                        Destroy(gameObject);
                        return;
                    }
                    else
                        currentIndex = 0;
                }

                if (!randomizeExactTarget)
                {
                    currentNodeTarget = m_waypointManager.GetNodePosition(currentIndex);

                    return;
                }

                // Get a position high enough that the agent wont clip the terrain
                // (NOTE: The pivot point must be in the center)
                Vector3 targetPosition = new Vector3(((Random.insideUnitSphere.x * 2) * m_nodeProximityDistance),
                                                        0 + (GetComponent<Collider>().bounds.extents.magnitude) / 2 + 0.5f,
                                                        ((Random.insideUnitSphere.z * 2) * m_nodeProximityDistance));

                currentNodeTarget = targetPosition + m_waypointManager.GetNodePosition(currentIndex);

            }
        }
        else
        {
            Vector3 relativeVector = transform.InverseTransformPoint(currentTarget.transform.position);

            float steerAngle = relativeVector.x / relativeVector.magnitude;
            float speed = relativeVector.z / relativeVector.magnitude;
            float brakeforce = 0;

            if (carController.CurrentSpeed > 0 && speed < 0)
            {
                brakeforce = speed;
            }
        }
            
        
    }

}
