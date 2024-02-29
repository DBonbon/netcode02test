using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    public GameObject cardPrefab; // Network object prefab for cards
    public GameObject cardUIPrefab; // UI prefab for cards
    public Transform DecklTransform; // Parent object for Card UI instances

    private List<CardUI> cardUIPool = new List<CardUI>(); // Pool for Card UI
    public List<CardData> allCardsList = new List<CardData>(); // Loaded card data
    public List<GameObject> allSpawnedCards = new List<GameObject>(); // Inventory of spawned network card object

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
       allCardsList = loadedCardDataList;
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

        foreach (var cardData in allCardsList)
        {
            var cardUIObject = Instantiate(cardUIPrefab, DecklTransform);
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
        foreach (var cardData in allCardsList)
        {
            var spawnedCard = Instantiate(cardPrefab, transform); // Instantiate without parent to avoid hierarchy issues
            var networkObject = spawnedCard.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
                
                var cardComponent = spawnedCard.GetComponent<Card>();
                if (cardComponent != null)
                {
                    // Initialize the card
                    cardComponent.InitializeCard(cardData.cardId, cardData.cardName, cardData.suit, cardData.hint, cardData.siblings);
                    allSpawnedCards.Add(spawnedCard);

                    // Assuming DeckInstance holds the deck GameObject, access Deck component to call AddCardToDeck
                    if (DeckManager.Instance.DeckInstance != null)
                    {
                        var deckComponent = DeckManager.Instance.DeckInstance.GetComponent<Deck>();
                        if (deckComponent != null)
                        {
                            deckComponent.AddCardToDeck(spawnedCard); // Pass the GameObject directly
                        }
                        else
                        {
                            Debug.LogError("Deck component not found on DeckInstance.");
                        }
                    }
                    else
                    {
                        Debug.LogError("DeckInstance is null.");
                    }
                    
                    Debug.Log($"Card initialized and its Name is: {cardData.cardName}");
                }
            }
        }
    }

    private void ShuffleCards()
    {
        System.Random rng = new System.Random();
        int n =allCardsList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value =allCardsList[k];
           allCardsList[k] =allCardsList[n];
           allCardsList[n] = value;
        }
    }

    private void OnDestroy()
    {
        DataManager.OnCardDataLoaded -= LoadCardDataLoaded;
    }

    /*public void DistributeCards0(List<Player> players)
    {
        int cardsPerPlayer = 5; // Assuming 5 cards per player for this example

        // Check if there are enough cards for all players
        Debug.Log($"the number of spawnedCards is: {allSpawnedCards.Count}");
        Debug.Log($"the number of cards needed to be distributed: {cardsPerPlayer * players.Count}");
        if (allSpawnedCards.Count < cardsPerPlayer * players.Count)
        {
            Debug.LogError("Not enough cards for all players.");
            return;
        }

        foreach (var player in players)
        {
            for (int i = 0; i < cardsPerPlayer; i++)
            {
                // Ensure there's a card to distribute
                if (allSpawnedCards.Count > 0)
                {
                    GameObject cardGameObject = allSpawnedCards[0]; // Get the first card
                    Card cardComponent = cardGameObject.GetComponent<Card>(); // Get the Card component

                    if (cardComponent != null)
                    {
                        player.AddCardToHand(cardGameObject); // Add the card to the player's hand
                        allSpawnedCards.RemoveAt(0); // Remove the card from the spawned list
                    }
                    else
                    {
                        Debug.LogError("Spawned card does not have a Card component.");
                    }
                }
            }

            // Now, correctly send card IDs to the client after all cards have been added to the hand
            player.SendCardIDsToClient();
        }
    }*/

    // CardManager.cs
    // In CardManager.cs
    public void DistributeCards(List<Player> players) 
    {
        
        Debug.Log("distributecards started");
        int cardsPerPlayer = 5; // Assuming 5 cards per player

        Deck deck = DeckManager.Instance.DeckInstance.GetComponent<Deck>();
        if (deck == null) {
            Debug.LogError("Deck is not found.");
            return;
        }

        foreach (var player in players) {
            for (int i = 0; i < cardsPerPlayer; i++) {
                Card card = deck.RemoveCardFromDeck(); // This now returns a Card object
                if (card != null) {
                    player.AddCardToHand(card); // Adjusted to accept Card objects
                    Debug.Log("playerAddCardToHand Card");
                } else {
                    Debug.LogWarning("Deck is out of cards.");
                    break; // Stop if there are no more cards
                }
            }
            player.SendCardIDsToClient();
        }
    }




       // Utility methods for CardUI pool management can be added here if needed
}