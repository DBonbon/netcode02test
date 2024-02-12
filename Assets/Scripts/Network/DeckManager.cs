using Unity.Netcode;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    public GameObject deckPrefab;
    public GameObject DeckInstance { get; private set; } // Store the spawned deck instance

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
        NetworkManager.Singleton.OnServerStarted += SpawnDeckPrefab;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnDeckPrefab;
        }
    }

    private void SpawnDeckPrefab()
    {
        if (NetworkManager.Singleton.IsServer && deckPrefab != null)
        {
            DeckInstance = Instantiate(deckPrefab);
            DeckInstance.GetComponent<NetworkObject>().Spawn();
            Debug.Log("Deck prefab spawned on server start.");
        }
    }
}
