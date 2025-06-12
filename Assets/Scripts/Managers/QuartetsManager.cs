using Unity.Netcode;
using UnityEngine;

public class QuartetManager : MonoBehaviour
{
    public static QuartetManager Instance;
    public GameObject quartetPrefab;
    public GameObject QuartetInstance { get; private set; } // Store the spawned quartets instance

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
        NetworkManager.Singleton.OnServerStarted += SpawnQuartetPrefab;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnQuartetPrefab;
        }
    }

    private void SpawnQuartetPrefab()
    {
        if (NetworkManager.Singleton.IsServer && quartetPrefab != null)
        {
            QuartetInstance = Instantiate(quartetPrefab);
            QuartetInstance.GetComponent<NetworkObject>().Spawn();
            Debug.Log("Quartets prefab spawned on server start.");
        }
    }
}
