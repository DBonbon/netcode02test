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
                    
                    //Debug.Log($"Card initialized and its Name is: {cardData.cardName}");
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

    public void DistributeCards(List<Player> players) 
    {       
        //Debug.Log("distributecards started");
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
                    //Debug.Log("playerAddCardToHand Card");
                } else {
                    Debug.LogWarning("Deck is out of cards.");
                    break; // Stop if there are no more cards
                }
            }
            player.SendCardIDsToClient();
        }
    }

    public void DrawCardFromDeck(Player currentPlayer)
    {
        if (currentPlayer != null)
        {
            /*if (deckCards.Count > 0)
            {
                Card card = deckCards[0]; // Get the top card from the deck
                deckCards.RemoveAt(0); // Remove the card from the deckCards list

                currentPlayer.AddCardToHand(card); // Add the card to the player's hand

                // Update the card's parent transform to the player's hand
                card.gameObject.transform.SetParent(currentPlayer.PlayerHand);

                 

                // You can apply an offset if needed
                Vector3 positionOffset = new Vector3(0, 0, currentPlayer.HandCards.Count * 0.02f);
                card.gameObject.transform.localPosition += positionOffset;

                // You might want to update the visual representation of the deck.
                UpdateDeck();
            }
            else
            {
                Debug.LogWarning("No cards left in the deck.");
            }*/
        }
        else
        {
            Debug.LogWarning("Invalid player.");
        }
    }

    public string GetCardNameById(int cardId)
    {
        var cardData = allCardsList.Find(card => card.cardId == cardId);
        return cardData != null ? cardData.cardName : "Unknown Card";
    }
       // Utility methods for CardUI pool management can be added here if needed
}