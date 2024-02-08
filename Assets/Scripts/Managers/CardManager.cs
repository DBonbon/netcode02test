using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    public GameObject cardPrefab; // Assign this in the inspector
    public static List<GameObject> SpawnedCards = new List<GameObject>(); // List of spawned objects

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnCards;
    }

    public void SpawnCards()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i < 5; i++) // Spawn 5 objects
            {
                GameObject spawnedCard = Instantiate(cardPrefab);
                spawnedCard.GetComponent<NetworkObject>().Spawn();

                // Initialize each card with its properties
                Card cardComponent = spawnedCard.GetComponent<Card>();
                if (cardComponent != null)
                {
                    string cardName = "Card " + i;
                    string suit = "AA"; // Test suit
                    string hint = "abc"; // Test hint
                    cardComponent.InitializeCard(cardName, suit, hint);
                }

                SpawnedCards.Add(spawnedCard);
                Debug.Log($"SpawnedCard {i} set: {spawnedCard.name}");
            }
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnCards;
        }
    }

    public void InitializeCards(List<CardData> namecardDataList)
    {
        //return null;
    }
}
