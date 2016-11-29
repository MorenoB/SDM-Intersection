
using UnityEngine;

public interface IMovingEntity{


    float CurrentSpeed { get; }

    void SetFreeze(bool value);
    void Move(Vector3 targetPosition);
}
