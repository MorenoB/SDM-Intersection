using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(BoxCollider))]
public class FreezeInContactWithEntities : MonoBehaviour {

    public CarController carController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("MovingEntity"))
        {
            carController.SetFreezeCar(true);
        }
    }

    private void OnTriggerLeave(Collider other)
    {
        if (other.CompareTag("MovingEntity"))
        {
            carController.SetFreezeCar(false);
        }
    }
}
