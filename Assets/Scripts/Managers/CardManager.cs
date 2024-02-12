using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    public GameObject cardPrefab; // Assign this in the inspector
    public static List<GameObject> SpawnedCards = new List<GameObject>(); // List of spawned objects
    public List<CardData> cardDataList = new List<CardData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DataManager.OnCardDataLoaded += LoadCardDataLoaded; // Subscribe to event
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadCardDataLoaded(List<CardData> loadedCardDataList)
    {
        this.cardDataList = loadedCardDataList;

        // Shuffle the cards before spawning them
        ShuffleCards();

        Debug.Log("Card data loaded into CardManager. Total cards: " + cardDataList.Count);
        // Optionally, log details of the first card to verify shuffle integrity
        if (cardDataList.Any())
        {
            var firstCard = cardDataList.First();
            Debug.Log($"First card name after shuffle: {firstCard.cardName}, Suit: {firstCard.suit}");
        }
    }

    private void ShuffleCards()
    {
        System.Random rng = new System.Random();  
        int n = cardDataList.Count;  
        while (n > 1) 
        {  
            n--;  
            int k = rng.Next(n + 1);  
            CardData value = cardDataList[k];  
            cardDataList[k] = cardDataList[n];  
            cardDataList[n] = value;  
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () => StartCoroutine(StartCardSpawningProcess());
    }


    System.Collections.IEnumerator StartCardSpawningProcess()
    {
        // Wait until the deck is confirmed to be spawned
        while (DeckManager.Instance.DeckInstance == null)
        {
            yield return null; // Wait for one frame
        }

        // Now that we have the deck, proceed to spawn cards
        SpawnCards();
    }

    private void SpawnCards()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject deck = DeckManager.Instance.DeckInstance; // Access the deck instance
            if (deck == null)
            {
                Debug.LogError("Deck instance is not available for parenting.");
                return;
            }

            for (int i = 0; i < cardDataList.Count; i++)
            {
                GameObject spawnedCard = Instantiate(cardPrefab);
                spawnedCard.GetComponent<NetworkObject>().Spawn();

                // Parent to deck after spawning and reset local position to align exactly with the deck
                spawnedCard.transform.SetParent(deck.transform, false);
                //spawnedCard.transform.localPosition = Vector3.zero;

                Card cardComponent = spawnedCard.GetComponent<Card>();
                if (cardComponent != null)
                {
                    CardData data = cardDataList[i];
                    cardComponent.InitializeCard(data.cardName, data.suit, data.hint, data.siblings);
                }

                SpawnedCards.Add(spawnedCard);
            }
        }
    }




    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnCards;
        }

        DataManager.OnCardDataLoaded -= LoadCardDataLoaded; 
    }

    public void InitializeCards(List<CardData> namecardDataList)
    {
        //return null;
    }
}