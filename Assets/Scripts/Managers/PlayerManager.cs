using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;
    public static int TotalPlayerPrefabs = 2;
    private int connectedPlayers = 0;
    public List<Player> players = new List<Player>();

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
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            connectedPlayers++;
            var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (playerObject != null)
            {
                var player = playerObject.GetComponent<Player>();
                if (player != null)
                {
                    // Initialize player with name, dbId, and image path
                    string playerName = "Player " + clientId.ToString();
                    int playerDbId = 23; // Default database ID
                    string playerImagePath = "Images/character_01"; // Default image path

                    player.InitializePlayer(playerName, playerDbId, playerImagePath);

                    players.Add(player);

                    if (connectedPlayers == TotalPlayerPrefabs)
                    {
                        DistributeCards();
                        UpdatePlayerToAsk();
                    }
                    
                }
            }
        }
    }

    private void DistributeCards()
    {
        int index = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            var player = playerObject?.GetComponent<Player>();
            if (player != null)
            {
                // Assign two objects to each player
                player.AddCardToHand(CardManager.SpawnedCards[index++]);
                player.AddCardToHand(CardManager.SpawnedCards[index++]);
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

}
