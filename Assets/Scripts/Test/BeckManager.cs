using Unity.Netcode;
using UnityEngine;

public class BeckManager : MonoBehaviour
{
    public static BeckManager Instance;
    public GameObject beckPrefab;
    public GameObject BeckInstance { get; private set; } // Store the spawned beck instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnBeckPrefab;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnBeckPrefab;
        }
    }

    private void SpawnBeckPrefab()
    {
        if (NetworkManager.Singleton.IsServer && beckPrefab != null)
        {
            BeckInstance = Instantiate(beckPrefab);
            BeckInstance.GetComponent<NetworkObject>().Spawn();
            Debug.Log("Beck prefab spawned on server start.");
        }
    }
}
