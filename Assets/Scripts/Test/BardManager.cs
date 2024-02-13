using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using System.Collections.Generic;

public class BardManager : MonoBehaviour
{
    /*public static BardManager Instance;
    public GameObject bardPrefab; // Assign this in the inspector
    public static List<GameObject> SpawnedBards = new List<GameObject>(); // List of spawned objects
    public List<BardData> bardDataList = new List<BardData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DataManager.OnBardDataLoaded += LoadBardDataLoaded; // Subscribe to event
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadBardDataLoaded(List<BardData> loadedBardDataList)
    {
        this.bardDataList = loadedBardDataList;

        // Shuffle the bards before spawning them
        ShuffleBards();

        Debug.Log("Bard data loaded into BardManager. Total bards: " + bardDataList.Count);
        // Optionally, log details of the first bard to verify shuffle integrity
        if (bardDataList.Any())
        {
            var firstBard = bardDataList.First();
            Debug.Log($"First bard name after shuffle: {firstBard.bardName}, Suit: {firstBard.suit}");
        }
    }

    private void ShuffleBards()
    {
        System.Random rng = new System.Random();  
        int n = bardDataList.Count;  
        while (n > 1) 
        {  
            n--;  
            int k = rng.Next(n + 1);  
            BardData value = bardDataList[k];  
            bardDataList[k] = bardDataList[n];  
            bardDataList[n] = value;  
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () => StartCoroutine(StartBardSpawningProcess());
    }


    System.Collections.IEnumerator StartBardSpawningProcess()
    {
        // Wait until the beck is confirmed to be spawned
        while (BeckManager.Instance.BeckInstance == null)
        {
            yield return null; // Wait for one frame
        }

        // Now that we have the beck, proceed to spawn bards
        SpawnBards();
    }

    private void SpawnBards()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject beck = BeckManager.Instance.BeckInstance; // Access the beck instance
            if (beck == null)
            {
                Debug.LogError("Beck instance is not available for parenting.");
                return;
            }

            for (int i = 0; i < bardDataList.Count; i++)
            {
                GameObject spawnedBard = Instantiate(bardPrefab);
                spawnedBard.GetComponent<NetworkObject>().Spawn();

                // Parent to beck after spawning and reset local position to align exactly with the beck
                spawnedBard.transform.SetParent(beck.transform, false);
                //spawnedBard.transform.SetParent(null); 
                //spawnedBard.transform.localPosition = Vector3.zero;

                Bard bardComponent = spawnedBard.GetComponent<Bard>();
                if (bardComponent != null)
                {
                    BardData data = bardDataList[i];
                    bardComponent.InitializeBard(data.bardName, data.suit, data.hint, data.biblings);
                }

                SpawnedBards.Add(spawnedBard);
            }
        }
    }*/


    /*public void SpawnBards()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i < bardDataList.Count; i++)
            {
                GameObject spawnedBard = Instantiate(bardPrefab);
                spawnedBard.GetComponent<NetworkObject>().Spawn();

                Bard bardComponent = spawnedBard.GetComponent<Bard>();
                if (bardComponent != null && i < bardDataList.Count)
                {
                    BardData data = bardDataList[i];
                    // Pass the sibling names along with other data
                    bardComponent.InitializeBard(data.bardName, data.suit, data.hint, data.biblings);
                }

                SpawnedBards.Add(spawnedBard);
            }
        }
    }*/


/*    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnBards;
        }

        DataManager.OnBardDataLoaded -= LoadBardDataLoaded; 
    }

    public void InitializeBards(List<BardData> namebardDataList)
    {
        //return null;
    }*/
}