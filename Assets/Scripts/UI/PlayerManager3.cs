using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager3 : NetworkBehaviour
{
    public static PlayerManager3 Instance;
    public static int TotalPlayerPrefabs = 2;
    private int connectedPlayers = 0;
    public List<Player3> players = new List<Player3>();

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
                var player3 = playerObject.GetComponent<Player3>();
                if (player3 != null)
                {
                    // Initialize player with name, dbId, and image path
                    string playerName = "Player " + clientId.ToString();
                    int playerDbId = 23; // Default database ID
                    string playerImagePath = "Images/character_01"; // Default image path

                    player3.InitializePlayer(playerName, playerDbId, playerImagePath);

                    players.Add(player3);

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
            var player3 = playerObject?.GetComponent<Player3>();
            if (player3 != null)
            {
                // Assign two objects to each player
                player3.AddCardToHand(CardManager3.SpawnedCards[index++]);
                player3.AddCardToHand(CardManager3.SpawnedCards[index++]);
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

}
