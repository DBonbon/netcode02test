using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

public class CardManager3 : MonoBehaviour
{
    public GameObject cardPrefab; // Assign this in the inspector
    public static List<GameObject> SpawnedCards = new List<GameObject>(); // List of spawned objects

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
                Card3 cardComponent = spawnedCard.GetComponent<Card3>();
                if (cardComponent != null)
                {
                    cardComponent.InitializeCard("Card " + i); // Example initialization with a name
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
}
