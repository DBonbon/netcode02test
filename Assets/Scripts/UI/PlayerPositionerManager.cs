using Unity.Netcode;
using UnityEngine;

public class PlayerPositionerManager : NetworkBehaviour
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
                var playerPositioner = playerObject.GetComponent<PlayerPositioner>();
                if (playerPositioner != null)
                {
                    playerPositioner.UpdatePlayerNameUI(); // Update name on new client

                    // Distribute non-player objects if enough players have connected
                    if (connectedPlayers == TotalPlayerPrefabs)
                    {
                        DistributeNonPlayerObjects();
                    }
                }
            }
        }
    }

    private void DistributeNonPlayerObjects()
    {
        int index = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            var playerPositioner = playerObject?.GetComponent<PlayerPositioner>();
            if (playerPositioner != null)
            {
                // Assign two objects to each player
                playerPositioner.ParentNonPlayerObject(ServerStartSpawner.SpawnedObjects[index++]);
                playerPositioner.ParentNonPlayerObject(ServerStartSpawner.SpawnedObjects[index++]);
            }
        }

        // Remaining object stays with the ServerStartSpawner
    }
}
