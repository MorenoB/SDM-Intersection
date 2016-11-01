using UnityEngine;
using System.Collections;
using EZObjectPools;

public class SpawnManager : MonoBehaviour {

    [Range(1,500)]
    public int poolSize = 200;

    public GameObject carPrefab = null;

    public enum SpawnType { CAR, TRAIN, PEDESTRIAN, BICYCLE };

    private EZObjectPool carObjectPool;


    void Awake()
    {
        carObjectPool = EZObjectPool.CreateObjectPool(carPrefab, "Cars", poolSize, true, true, true);
    }

    public void SpawnObject(SpawnType spawnType, string trafficLaneId)
    {
        GameObject outObject = null;
        switch(spawnType)
        {
            case SpawnType.CAR:
                if(carObjectPool.TryGetNextObject(Vector3.one, Quaternion.identity, out outObject))
                {
                    TrafficManager.Instance.SpawnEntityAtLane(trafficLaneId, outObject);
                }

                break;

            default:

                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnObject(SpawnType.CAR, "N4");
        }
    }
}
