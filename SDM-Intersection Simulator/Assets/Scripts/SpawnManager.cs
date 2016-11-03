using UnityEngine;
using System.Collections;
using EZObjectPools;

public class SpawnManager : MonoBehaviour
{

    [Range(1, 500)]
    public int poolSize = 200;

    public GameObject carPrefab = null;
    public GameObject bicyclePrefab = null;
    public GameObject trainPrefab = null;
    public GameObject busPrefab = null;
    public GameObject pedestrianPrefab = null;

    public enum SpawnType { CAR, TRAIN, PEDESTRIAN, BICYCLE, BUS };

    private EZObjectPool carObjectPool;
    private EZObjectPool bicycleObjectPool;
    private EZObjectPool trainObjectPool;
    private EZObjectPool busObjectPool;
    private EZObjectPool pedestrianObjectPool;


    void Awake()
    {
        carObjectPool = EZObjectPool.CreateObjectPool(carPrefab, "Cars", poolSize, true, true, true);
        bicycleObjectPool = EZObjectPool.CreateObjectPool(bicyclePrefab, "Bicycles", poolSize, true, true, true);
        trainObjectPool = EZObjectPool.CreateObjectPool(trainPrefab, "Trains", poolSize, true, true, true);
        busObjectPool = EZObjectPool.CreateObjectPool(busPrefab, "Busses", poolSize, true, true, true);
        pedestrianObjectPool = EZObjectPool.CreateObjectPool(pedestrianPrefab, "Pedestrians", poolSize, true, true, true);
    }

    public void SpawnObject(SpawnType spawnType, int trafficLaneId)
    {
        GameObject outObject = null;
        switch (spawnType)
        {
            case SpawnType.CAR:
                if (carObjectPool.TryGetNextObject(Vector3.one, Quaternion.identity, out outObject))
                {
                    TrafficManager.Instance.InitEntityAtLane(trafficLaneId, outObject);
                }

                break;

            case SpawnType.BICYCLE:
                if (bicycleObjectPool.TryGetNextObject(Vector3.one, Quaternion.identity, out outObject))
                {
                    TrafficManager.Instance.InitEntityAtLane(trafficLaneId, outObject);
                }
                break;

            case SpawnType.BUS:
                if (busObjectPool.TryGetNextObject(Vector3.one, Quaternion.identity, out outObject))
                {
                    TrafficManager.Instance.InitEntityAtLane(trafficLaneId, outObject);
                }
                break;

            case SpawnType.PEDESTRIAN:
                if (pedestrianObjectPool.TryGetNextObject(Vector3.one, Quaternion.identity, out outObject))
                {
                    TrafficManager.Instance.InitEntityAtLane(trafficLaneId, outObject);
                }
                break;

            case SpawnType.TRAIN:
                if (trainObjectPool.TryGetNextObject(Vector3.one, Quaternion.identity, out outObject))
                {
                    TrafficManager.Instance.InitEntityAtLane(trafficLaneId, outObject);
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
            int randomLane = Random.Range(1, 11);
            SpawnObject(SpawnType.CAR, randomLane);
           
        }
    }
}
