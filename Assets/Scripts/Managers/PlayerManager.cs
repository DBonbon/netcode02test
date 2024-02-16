using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;
    public static int TotalPlayerPrefabs = 2; // Adjust based on your game's needs
    private int connectedPlayers = 0;
    public List<Player> players = new List<Player>();
    private List<PlayerData> playerDataList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DataManager.OnPlayerDataLoaded += LoadPlayerDataLoaded; // Subscribe to event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        DataManager.OnPlayerDataLoaded -= LoadPlayerDataLoaded; // Unsubscribe from event
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public void LoadPlayerDataLoaded(List<PlayerData> loadedPlayerDataList)
    {
        this.playerDataList = loadedPlayerDataList; 
        Debug.Log("Player data loaded into PlayerManager.");
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        connectedPlayers++;
        var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (playerObject != null)
        {
            var player = playerObject.GetComponent<Player>();
            if (player != null && connectedPlayers <= playerDataList.Count)
            {
                string playerImagePath = "Images/character_01"; // Default image path
                var playerData = playerDataList[connectedPlayers - 1]; // Example, adjust as necessary
                player.InitializePlayer(playerData.playerName, playerData.playerDbId, playerData.playerImagePath);
                // New logic to broadcast existing player names to the newly connected client
                BroadcastPlayerNamesToNewClient(clientId);

                players.Add(player);
                if (connectedPlayers == TotalPlayerPrefabs)
                {
                    DistributeCards();
                    UpdatePlayerToAsk();
                }
            }
        }
    }

    private void BroadcastPlayerNamesToNewClient(ulong newClientId)
    {
        foreach (var player in players)
        {
            // Use the new method to broadcast both attributes
            player.BroadcastPlayerDbAttributes();
        }
    }

    private void DistributeCards()
    {
        int cardsPerPlayer = 5; // Assuming 5 cards per player for this example

        // Check if there are enough cards for all players
        Debug.Log($"the number of spawnedCards is: {CardManager.Instance.spawnedCards.Count}");
        Debug.Log($"the number of cards needed to be distributed: {cardsPerPlayer * players.Count}");
        if (CardManager.Instance.spawnedCards.Count < cardsPerPlayer * players.Count)
        {
            Debug.LogError("Not enough cards for all players.");
            return;
        }

        foreach (var player in players)
        {
            for (int i = 0; i < cardsPerPlayer; i++)
            {
                // Ensure there's a card to distribute
                if (CardManager.Instance.spawnedCards.Count > 0)
                {
                    GameObject cardGameObject = CardManager.Instance.spawnedCards[0]; // Get the first card
                    Card cardComponent = cardGameObject.GetComponent<Card>(); // Get the Card component

                    if (cardComponent != null)
                    {
                        player.AddCardToHand(cardGameObject); // Add the card to the player's hand
                        CardManager.Instance.spawnedCards.RemoveAt(0); // Remove the card from the spawned list
                    }
                    else
                    {
                        Debug.LogError("Spawned card does not have a Card component.");
                    }
                }
            }

            // After adding cards to the player's hand, update the player's UI
            if (player.TryGetComponent(out PlayerUI playerUI))
            {
                playerUI.UpdatePlayerHandUI(player.HandCards);
            }
        }
    }


    private void UpdatePlayerToAsk()
    {
        // Iterate through each player in the 'players' list
        foreach (var player in players)
        {
            // Call a method on the player instance to update its PlayerToAsk list
            player.UpdatePlayerToAskList(players);
        }
    }

    // Method to clean up players, if necessary
    public void CleanupPlayers()
    {
        players.Clear();
    }

    public void GenerateGamePlayers(List<PlayerData> name)
    {
        //return null;
    }

}