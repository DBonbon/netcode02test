using Unity.Netcode;
using UnityEngine;

public class ServerStartSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Assign this in the inspector
    public static GameObject SpawnedObject { get; private set; } // Static reference

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnObject;
    }

    private void SpawnObject()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn);
            spawnedObject.GetComponent<NetworkObject>().Spawn(); // Spawn the object on the network
            SpawnedObject = spawnedObject; // Store the instance in the static reference
            Debug.Log($"SpawnedObject set: {SpawnedObject.name}"); // Debug log to confirm assignment
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnObject;
        }
    }
}
