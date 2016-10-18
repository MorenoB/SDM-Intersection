using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    [Range(1, 15)]
    public int moveSpeed = 1;

    void Update()
    {

        Vector3 curPosition = transform.position;

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(curPosition.x, curPosition.y, curPosition.z + moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(curPosition.x, curPosition.y, curPosition.z - moveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(curPosition.x - moveSpeed, curPosition.y, curPosition.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(curPosition.x + moveSpeed, curPosition.y, curPosition.z);
        }
    }
}
