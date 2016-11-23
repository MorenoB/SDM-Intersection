﻿using UnityEngine;
using System.Collections;
using EZObjectPools;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn settings.")]
    [Range(2, 10)]
    public int spawnRate = 2;
    public int maximumCarsInLane = 6;
    public int maximumBicyclesInLane = 6;
    public int maximumBussesInLane = 1;
    public int maximumTrainsInLane = 1;


    [Header("Prefab settings.")]
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

    private bool randomCarSpawningLoopActive;
    private bool randomBicycleSpawningLoopActive;
    private bool randomBusSpawningLoopActive;


    void Awake()
    {
        carObjectPool = EZObjectPool.CreateObjectPool(carPrefab, "Cars", poolSize, true, true, true);
        bicycleObjectPool = EZObjectPool.CreateObjectPool(bicyclePrefab, "Bicycles", poolSize, true, true, true);
        trainObjectPool = EZObjectPool.CreateObjectPool(trainPrefab, "Trains", poolSize, true, true, true);
        busObjectPool = EZObjectPool.CreateObjectPool(busPrefab, "Busses", poolSize, true, true, true);
        pedestrianObjectPool = EZObjectPool.CreateObjectPool(pedestrianPrefab, "Pedestrians", poolSize, true, true, true);
    }

    private void Start()
    {
        StartCoroutine(RandomSpawnLoop());
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

    private IEnumerator RandomSpawnLoop()
    {
        while (true)
        {
            if (randomCarSpawningLoopActive)
            {
                TrafficLaneData randomCarLane = TrafficManager.Instance.GetLane(SpawnType.CAR, maximumCarsInLane);


                if (randomCarLane != null)                
                SpawnObject(SpawnType.CAR, randomCarLane.id);

            }

            if(randomBicycleSpawningLoopActive)
            {
                TrafficLaneData randomBicycleLane = TrafficManager.Instance.GetLane(SpawnType.BICYCLE, maximumBicyclesInLane);

                if (randomBicycleLane != null)
                SpawnObject(SpawnType.BICYCLE, randomBicycleLane.id);
            }

            if(randomBusSpawningLoopActive)
            {
                TrafficLaneData randomBusLane = TrafficManager.Instance.GetLane(SpawnType.BUS, maximumBussesInLane);

                if (randomBusLane != null)
                    SpawnObject(SpawnType.BUS, randomBusLane.id);
            }


            yield return new WaitForSeconds(spawnRate);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            randomCarSpawningLoopActive = !randomCarSpawningLoopActive;
            Debug.Log("Random car loop is " + randomCarSpawningLoopActive);

        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            randomBicycleSpawningLoopActive = !randomBicycleSpawningLoopActive;
            Debug.Log("Random bicycle loop is " + randomBicycleSpawningLoopActive);

        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            randomBusSpawningLoopActive = !randomBusSpawningLoopActive;
            Debug.Log("Random bus loop is " + randomBusSpawningLoopActive);
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            TrafficLaneData randomTrainLane = TrafficManager.Instance.GetLane(SpawnType.TRAIN, maximumTrainsInLane);

            if(randomTrainLane == null)
            {
                Debug.LogError("No available train lanes");
                return;
            }

            SpawnObject(SpawnType.TRAIN, randomTrainLane.id);
        }
    }
}
