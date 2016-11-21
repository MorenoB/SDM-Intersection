﻿using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(IMovingEntity))]
public class WaypointAgent : MonoBehaviour {


	//New
	public bool randomizeExactTarget = false;

	public Trafficlight trafficLight;

	private IMovingEntity movingEntity;
	private bool waypointSystemActivated = true;
	private bool hasLeftLane = false;

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

	public virtual void Start()
	{
		speed = Random.Range(minAgentSpeed, maxAgentSpeed);
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

	private void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("StopLine"))
		{
			
			WaypointSystemActivated = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("HasLeftLane"))
		{
			if (hasLeftLane)
				return;

			hasLeftLane = true;

			if (trafficLight == null)
			{
				Debug.LogError("WaypointAgent " + gameObject.name + " does not have a valid TrafficLight object assigned!");
				return;
			}

			TrafficManager.Instance.DecreaseNumberOfCarsInLaneByOne(trafficLight.Id);
		}
	}

	protected void WaypointMovementUpdate()
	{
		Vector3 relativeVector = Vector3.one;

		float steerAngle = 0;
		float speed = 0;
		float brakeforce = 0;

		// If the agent has a game object target assigned then move towards it otherwise 
		// get a target position in 3d space and move towards that
		if (currentTarget == null)
		{
			relativeVector = transform.InverseTransformPoint(currentNodeTarget);

			steerAngle = relativeVector.x / relativeVector.magnitude;
			speed = relativeVector.z / relativeVector.magnitude;

			if (movingEntity.CurrentSpeed > 0 && speed < 0)
			{
				brakeforce = speed;
			}

			movingEntity.Move(steerAngle, speed, brakeforce, brakeforce);

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
			relativeVector = transform.InverseTransformPoint(currentTarget.transform.position);

			steerAngle = relativeVector.x / relativeVector.magnitude;
			speed = relativeVector.z / relativeVector.magnitude;

			if (movingEntity.CurrentSpeed > 0 && speed < 0)
			{
				brakeforce = speed;
			}

			movingEntity.Move(steerAngle, speed, brakeforce, brakeforce);
		}
			
		
	}

}