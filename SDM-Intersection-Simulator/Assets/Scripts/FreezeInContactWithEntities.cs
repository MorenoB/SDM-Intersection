using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FreezeInContactWithEntities : MonoBehaviour
{
    
    private IMovingEntity movingEntity;

    public void OnEnable()
    {
        if (movingEntity == null)
            movingEntity = GetComponentInParent<IMovingEntity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovingEntity"))
        {
            movingEntity.SetFreeze(true);
        }
    }

    private void OnTriggerLeave(Collider other)
    {
        if (other.CompareTag("MovingEntity"))
        {
            movingEntity.SetFreeze(false);
        }
    }
}
