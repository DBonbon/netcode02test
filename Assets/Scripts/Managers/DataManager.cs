using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public PlayerManager playerManager; // Reference to the PlayerManager script
    //public CardManager cardManager; // Reference to the CardManager script
    
    // Define events for player data and card data loading
    public delegate void PlayerDataLoadedHandler(List<PlayerData> players);
    public static event PlayerDataLoadedHandler OnPlayerDataLoaded;

    public delegate void CardDataLoadedHandler(List<CardData> cards);
    public static event CardDataLoadedHandler OnCardDataLoaded;

    // Static properties to access loaded data
    public static List<PlayerData> LoadedPlayerData { get; private set; }
    //public static List<CardData> LoadedCardData { get; private set; }

    private bool playerDataLoaded = false;
    //private bool cardDataLoaded = false;

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
        DataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
        DataManager.OnCardDataLoaded += OnCardDataLoaded; // Ensure this line exists

    }
   

    private void Start()
    {
        LoadPlayersFromJson();
        LoadCardsFromJson();
        //Debug.Log($"{Time.time}:datamanager.start method has started");
    }

    private void LoadPlayersFromJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "players.json");

        StartCoroutine(LoadPlayersData(path));
    }

    private IEnumerator LoadPlayersData(string path)
    {
        //Debug.Log($"{Time.time}:LoadPlayersData started.");
        string json;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
            json = www.downloadHandler.text;
            // Use UnityWebRequest for mobile platforms
            // ... (code to load JSON using UnityWebRequest)
        }
        else
        {
            json = File.ReadAllText(path);
        }

        // Create a wrapper class to hold the "players" array from the JSON data
        PlayersDataWrapper dataWrapper = JsonUtility.FromJson<PlayersDataWrapper>(json);

        // Get the player data list from the wrapper
        List<PlayerData> playerDataList = dataWrapper.players;

        // Set the LoadedPlayerData property first
        LoadedPlayerData = playerDataList;

        // Pass the player data to the PlayerManager
        PlayerManager.Instance.LoadPlayerDataLoaded(playerDataList);

        // Notify subscribers that player data is loaded
        OnPlayerDataLoaded?.Invoke(playerDataList);
        playerDataLoaded = true;
        //Debug.Log("player loaded from dataM");
        //CheckStartManagers(); // Check if both player and card data are loaded

        // After loading data, notify the GameFlowManager
        //GameFlowManager.Instance.OnPlayerDataLoaded();
        
        Debug.Log($"{Time.time}: DataManager.LoadPlayersData coroutine ended.");
    }

    [System.Serializable]
    private class PlayersDataWrapper
    {
        public List<PlayerData> players;
        //Debug.Log($"{Time.time}: DataManager.PlayersDataWrapper method has been generated");
    }


    
    private void LoadCardsFromJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "cards2.json");

        StartCoroutine(LoadCardsData(path));
    }

    private IEnumerator LoadCardsData(string path)
    {
        Debug.Log($"{Time.time}:LoadCardsData started.");
        //Debug.Log("laodcardsdata called.");
        string json;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
            json = www.downloadHandler.text;
        }
        else
        {
            json = File.ReadAllText(path);
        }
        //Debug.Log("laodcardsdata after if loop.");
        CardsDataWrapper dataWrapper = JsonUtility.FromJson<CardsDataWrapper>(json);
        //Debug.Log("laodcardsdata afterdatawrapper.");
        List<CardData> cardDataList = dataWrapper.cards;
        //Debug.Log("laodcardsdata after carddatalist");
        // Populate siblingCardNames for each CardData object
        /*
        foreach (CardData cardData in cardDataList)
        {
            cardData.PopulateSiblings(cardDataList);
        }
        */
        //Debug.Log("laodcardsdata after foreachb loop.");
        // Notify subscribers that card data is loaded
        OnCardDataLoaded?.Invoke(cardDataList);
        Debug.Log($"{Time.time}: OnCardDataLoaded event invoked with card data.");
        //cardDataLoaded = true;
        CheckStartManagers(); // Check if both player and card data are loaded
        Debug.Log($"{Time.time}:LoadCardsData ended.");
        //cardManager.InitializeCards(cardDataList); // Initialize cards in CardManager
    }

    [System.Serializable]
    private class CardsDataWrapper
    {
        public List<CardData> cards;
    }

    // Check if both player and card data are loaded
    private void CheckStartManagers()
    {
        /*if (playerDataLoaded && cardDataLoaded)
        {
            // Both data sets are loaded, you can perform any necessary actions here
        }*/
    }
    
}