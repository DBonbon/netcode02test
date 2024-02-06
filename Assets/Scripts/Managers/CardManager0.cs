using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] private GameObject cardPrefab; // NetworkObject prefab, data container
    //[SerializeField] private GameObject cardUIPrefab; // MonoBehaviour prefab, UI representation
    [SerializeField] private Transform deckTransform; // Assign in Inspector

    private List<CardData> cardDataList = new List<CardData>();
    //private List<Card> instantiatedCards = new List<Card>();
    private List<Card> allCards = new List<Card>();
    private List<Card> deckCards = new List<Card>();

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

        InitializeCardData();
    }

    private void InitializeCardData()
    {
        string[] suits = { "AA", "BB", "CC", "DD", "EE", "FF" };
        for (int i = 0; i < 24; i++)
        {
            string cardName = ((char)('A' + i)).ToString();
            string suit = suits[i / 4];
            cardDataList.Add(new CardData(cardName, suit));
        }
        Debug.Log("carddatalist initialized");
    }

    public void StartCardManager()
    {
        Debug.Log("StartCardManager was called");
        SpawnAndInitializeCards();
        // Use a coroutine or a delayed invocation to distribute cards
        StartCoroutine(DelayedDistributeCards());
    }

    private System.Collections.IEnumerator DelayedDistributeCards()
    {
        // Wait for a short duration to ensure all CardUI instances are ready
        yield return new WaitForSeconds(10f);
        DistributeCards();
    }

    private void SpawnAndInitializeCards()
    {
        foreach (var cardData in cardDataList)
        {
            GameObject cardObject = Instantiate(cardPrefab, deckTransform.position, Quaternion.identity);
            NetworkObject networkObject = cardObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            Card card = cardObject.GetComponent<Card>();
            card.CardName.Value = new FixedString32Bytes(cardData.CardName);
            card.Suit.Value = new FixedString32Bytes(cardData.Suit);

            allCards.Add(card);
        }

        // Shuffle the cards after all have been instantiated
        ShuffleCards();

        // After shuffling, assign the shuffled order to deckCards
        deckCards.AddRange(allCards);
    }

    private void ShuffleCards()
    {
        System.Random random = new System.Random();
        int n = allCards.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);

            // Swap instantiatedCards[i] and instantiatedCards[j]
            Card temp = allCards[i];
            allCards[i] = allCards[j];
            allCards[j] = temp;
        }

        // Optionally, update the positions or other properties of the cards
        //UpdateCardPositionsAfterShuffle();
    }

    public void DistributeCards()
    {
        // Example: Distribute one card to each player
        foreach (var player in PlayerManager.Instance.players)
        {
            if (deckCards.Count > 0)
            {
                Card card = deckCards[0];
                deckCards.RemoveAt(0);
                player.ReceiveCard(card);

                // Update PlayerUI or other game logic as needed
            }
        }

        // Further logic for distributing cards
    }

    private void UpdateCardPositionsAfterShuffle()
    {
        // Implement the logic to update card positions or other properties post-shuffle
        // For example, reposition cards within the deckTransform
    }

    // Additional methods as needed...
}