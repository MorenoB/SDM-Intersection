using System;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianController : MonoBehaviour, IMovingEntity
{
    [Range(0,100)]
    public int speed = 5;

    public Vector3 centerOfMass;

    private Rigidbody rigidBody;
    private Rigidbody RigidBody
    {
        get
        {
            if (rigidBody == null)
                rigidBody = GetComponent<Rigidbody>();

            return rigidBody;
        }
    }

    public float CurrentSpeed { get { return RigidBody.velocity.magnitude * 2.23693629f; } }

    private void OnEnable()
    {
        RigidBody.centerOfMass = centerOfMass;
    }


    public void Move(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);

        RigidBody.velocity = transform.forward * (speed / 2.23693629f );

    }

    public void SetFreeze(bool value)
    {
        RigidBody.isKinematic = !value;
    }
}
