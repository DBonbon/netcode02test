using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ServerStartSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Assign this in the inspector
    public static List<GameObject> SpawnedObjects = new List<GameObject>(); // List of spawned objects

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnObjects;
    }

    private void SpawnObjects()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i < 5; i++) // Spawn 5 objects
            {
                GameObject spawnedObject = Instantiate(objectToSpawn);
                spawnedObject.GetComponent<NetworkObject>().Spawn();
                SpawnedObjects.Add(spawnedObject);
                Debug.Log($"SpawnedObject {i} set: {spawnedObject.name}");
            }
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnObjects;
        }
    }
}
    