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
    [SerializeField] private Transform deckUIContainer;

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
        //Debug.Log("Player data loaded into PlayerManager.");
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
                    //DistributeCards();
                    CardManager.Instance.DistributeCards(players);
                    //Debug.Log("CardManager.Instance.DistributeCards(players)");
                    AssignTurnToPlayer();
                    TurnManager.Instance.StartTurnManager();
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

    private void UpdatePlayerToAsk()
    {
        // Iterate through each player in the 'players' list
        foreach (var player in players)
        {
            // Call a method on the player instance to update its PlayerToAsk list
            player.UpdatePlayerToAskList(players);
        }
    }

    private void AssignTurnToPlayer()
    {
        if (!IsServer || players.Count == 0) return;

        // Reset all players' HasTurn to false first
        foreach (var player in players)
        {
            player.HasTurn.Value = false;
        }

        // Randomly select a player to assign the turn
        int randomIndex = Random.Range(0, players.Count);
        players[randomIndex].HasTurn.Value = true;
        players[randomIndex].ActivateTurnUIForPlayerClientRpc();

        Debug.Log($"Turn assigned to player: {players[randomIndex].PlayerName.Value}");
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

    public string GetPlayerNameByClientId(ulong clientId)
    {
        var player = players.Find(p => p.GetComponent<NetworkObject>().NetworkObjectId == clientId);
        return player != null ? player.PlayerName.Value.ToString() : "Unknown Player";
    }

}