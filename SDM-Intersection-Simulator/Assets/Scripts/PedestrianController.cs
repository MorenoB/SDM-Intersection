using System;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianController : MonoBehaviour, IMovingEntity
{
    [Range(0,100)]
    public int speed = 5;

    public Vector3 centerOfMass;

    private Rigidbody rigidBody;

    public float CurrentSpeed { get { return rigidBody.velocity.magnitude * 2.23693629f; } }

    private void Start()
    {
       
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass;
    }


    public void Move(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);

        rigidBody.velocity = transform.forward * (speed / 2.23693629f );

    }

    public void SetFreeze(bool value)
    {
        rigidBody.isKinematic = !value;
    }
}
