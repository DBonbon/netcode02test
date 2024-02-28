using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    public GameObject cardPrefab; // Network object prefab for cards
    public GameObject cardUIPrefab; // UI prefab for cards
    public Transform cardUIPoolParent; // Parent object for Card UI instances

    private List<CardUI> cardUIPool = new List<CardUI>(); // Pool for Card UI
    public List<CardData> cardDataList = new List<CardData>(); // Loaded card data
    public List<GameObject> spawnedCards = new List<GameObject>(); // Inventory of spawned network card object

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DataManager.OnCardDataLoaded += LoadCardDataLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () => StartCoroutine(StartCardSpawningProcess());
    }

    private void LoadCardDataLoaded(List<CardData> loadedCardDataList)
    {
        cardDataList = loadedCardDataList;
        ShuffleCards();
        InitializeCardUIPool(); // Ensure CardUI pool is initialized after loading data
    }

    private void InitializeCardUIPool()
    {
        foreach (var cardUIInstance in cardUIPool)
        {
            Destroy(cardUIInstance.gameObject); // Clear existing pool
        }
        cardUIPool.Clear();

        foreach (var cardData in cardDataList)
        {
            var cardUIObject = Instantiate(cardUIPrefab, cardUIPoolParent);
            var cardUIComponent = cardUIObject.GetComponent<CardUI>();
            if (cardUIComponent)
            {
                cardUIComponent.UpdateCardUIWithCardData(cardData);
                cardUIPool.Add(cardUIComponent);
                cardUIObject.SetActive(false); // Start inactive
            }
        }
    }

    public CardUI FetchCardUIById(int cardId)
    {
        foreach (CardUI cardUI in cardUIPool)
        {
            // Match based on cardId instead of CardName
            if (cardUI.cardId == cardId && !cardUI.gameObject.activeInHierarchy)
            {
                return cardUI;
            }
        }
        return null;
    }

    System.Collections.IEnumerator StartCardSpawningProcess()
    {
        while (DeckManager.Instance.DeckInstance == null)
        {
            yield return null;
        }
        SpawnCards();
    }

    private void SpawnCards()
    {
        foreach (var cardData in cardDataList)
        {
            var spawnedCard = Instantiate(cardPrefab, DeckManager.Instance.DeckInstance.transform);
            var networkObject = spawnedCard.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn(); // Spawn the card on the network
                
                var cardComponent = spawnedCard.GetComponent<Card>();
                if (cardComponent != null)
                {
                    // Now that the card is spawned, initialize it
                    cardComponent.InitializeCard(cardData.cardId, cardData.cardName, cardData.suit, cardData.hint, cardData.siblings);                   
                    //DeckManager.Instance.DeckInstance.GetComponent<Deck>().AddCardToDeck(spawnedCard); // Parent to deck
                    spawnedCards.Add(spawnedCard);
                    Debug.Log($"Card initialized and its Name is: {cardData.cardName}");
                }
            }
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
            var value = cardDataList[k];
            cardDataList[k] = cardDataList[n];
            cardDataList[n] = value;
        }
    }

    private void OnDestroy()
    {
        DataManager.OnCardDataLoaded -= LoadCardDataLoaded;
    }

       // Utility methods for CardUI pool management can be added here if needed
}