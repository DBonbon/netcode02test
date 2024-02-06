using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager3 : NetworkBehaviour
{
    public static int TotalPlayerPrefabs = 2;
    private int connectedPlayers = 0;

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
                    player3.UpdatePlayerNameUI(); // Update name on new client

                    // Distribute non-player objects if enough players have connected
                    if (connectedPlayers == TotalPlayerPrefabs)
                    {
                        DistributeCards();
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
                player3.ParentCard(CardManager3.SpawnedCards[index++]);
                player3.ParentCard(CardManager3.SpawnedCards[index++]);
            }
        }

        // Remaining object stays with the ServerStartSpawner
    }
}
