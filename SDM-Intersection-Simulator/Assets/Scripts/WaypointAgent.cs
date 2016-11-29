using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(IMovingEntity))]
public class WaypointAgent : MonoBehaviour {

	public bool randomizeExactTarget = false;
	public event System.Action OnWaypointsystemActivatedChanged;

	private Queue<Trafficlight> trafficlightQueue = new Queue<Trafficlight>();

	public Queue<Trafficlight> TrafficlightQueue
	{
		get
		{
			return trafficlightQueue;
		}
		set
		{
			if (value != trafficlightQueue)
				trafficlightQueue = value;
		}
	}

	private int trafficLaneId = -1;
	public int TrafficLaneId
	{
		get
		{
			return trafficLaneId;
		}

		set
		{
			trafficLaneId = value;
		}
	}

	public Trafficlight WaitingForTrafficLight = null;

	private IMovingEntity movingEntity;
	private bool waypointSystemActivated;
	private bool hasLeftLane = false;

	///
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

			if(OnWaypointsystemActivatedChanged != null)
				OnWaypointsystemActivatedChanged();


			movingEntity.SetFreeze(value);
			waypointSystemActivated = value;
		}
	}

	public WaypointManager WaypointSystem {  set { m_waypointManager = value; } }
	public WaypointRotationMode WaypointRotation { set { m_waypointRotationMode = value; } }
	public float SlerpSpeed { set { m_slerpRotationSpeed = value; } }
	public float Speed { get { return speed; } set { speed = value; } }
	public float NodeProximityDistance { set { m_nodeProximityDistance = value; } }
	public int CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }

	private void OnEnable()
	{
		movingEntity = GetComponent<IMovingEntity>();
	}

	public void ResetWaypointTargetToFirst()
	{
		hasLeftLane = false;

		currentIndex = 0;

		if (m_waypointManager == null) return;

		currentNodeTarget = m_waypointManager.GetNodePosition(currentIndex);
	}

	public virtual void Update()
	{
		if (m_waypointManager == null) return;        

		if (WaypointSystemActivated)
			WaypointMovementUpdate();
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

	private void OnDisable()
	{
		WaypointSystemActivated = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (WaitingForTrafficLight == null)
			return;

		if (other.CompareTag("StopLine"))
		{

			LaneIdentifier laneIdentifier = other.GetComponent<LaneIdentifier>();
			if (laneIdentifier == null)
			{
				//Does not matter
				return;
			}


			if (TrafficlightQueue == null)
			{
				Debug.LogError("WaypointAgent " + gameObject.name + " does not have a valid TrafficLane object assigned!");
				return;
			}

			//Make sure we stop at the correct trafficlight
			if (WaitingForTrafficLight.Id != laneIdentifier.Id)
				return;


			WaypointSystemActivated = false;
		}

		if (other.CompareTag("HasLeftLane"))
		{
			LaneIdentifier laneIdentifier = other.GetComponent<LaneIdentifier>();
			if(laneIdentifier == null)
			{
				Debug.LogError("Collider " + other + " has no LaneIdentifier! Check your collider tags");
				return;
			}


			if (TrafficlightQueue == null)
			{
				Debug.LogError("WaypointAgent " + gameObject.name + " does not have a valid TrafficLane object assigned!");
				return;
			}

			//Make sure we exit the correct lane
			if (WaitingForTrafficLight.Id != laneIdentifier.Id)
				return;

			WaitingForTrafficLight.WaitingAgents.Remove(this);

			AssignNextTrafficlight();

			hasLeftLane = true;
			TrafficManager.Instance.DecreaseNumberOfCarsInLaneByOne(TrafficLaneId);
		}
	}

	public void AssignNextTrafficlight()
	{
		if (TrafficlightQueue.Count < 1)
		{
			WaitingForTrafficLight = null;
			return;
		}

		WaitingForTrafficLight = TrafficlightQueue.Dequeue();

		if (!WaitingForTrafficLight.WaitingAgents.Contains(this))
			WaitingForTrafficLight.WaitingAgents.Add(this);
	}

	protected void WaypointMovementUpdate()
	{
		if (currentTarget == null)
		{

			movingEntity.Move(currentNodeTarget);

			if (m_waypointManager.ObjectIsOnNode(this))
			{

				currentIndex++;

				if (currentIndex >= m_waypointManager.NodeQuantity)
				{
					if (!m_waypointManager.looping)
					{
						m_waypointManager.RemoveEntity(this);
						gameObject.SetActive(false);
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

			movingEntity.Move(currentTarget.transform.position);
		}
			
		
	}

}
