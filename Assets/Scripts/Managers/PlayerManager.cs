using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<Player> players = new List<Player>();
    
    private List<PlayerData> playerDataList = new List<PlayerData>();

    private int spawnedPlayerIndex = 0;

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

        InitializePlayerData();
    }

    private void InitializePlayerData()
    {
        // Hard-coded player data
        playerDataList.Add(new PlayerData("Dana", 3));
        playerDataList.Add(new PlayerData("Alice", 3));
        //playerDataList.Add(new PlayerData("Bar", 3));
        //playerDataList.Add(new PlayerData("Ravit", 3));
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        spawnedPlayerIndex++;  // Increment the counter
        Debug.Log("Added player. Now showing player data:");
        ShowListPlayerData();

        // Assign name and score from playerDataList, if available
        if (playerDataList.Count > players.Count - 1)
        {
            var playerData = playerDataList[players.Count - 1];
            player.playerName.Value = playerData.playerName;
            player.playerScore.Value = playerData.playerScore;
        }

        CheckAllPlayersSpawned();
    }


    private void ShowListPlayerData()
    {
        foreach (var playerData in playerDataList)
        {
            Debug.Log("Player Name: " + playerData.playerName + ", Score: " + playerData.playerScore);
        }
    }

    // In PlayerManager.cs
    public int TotalPlayerCount()
    {
        return playerDataList.Count;
    }

    private void CheckAllPlayersSpawned()
    {
        Debug.Log("CheckAllPlayersSpawned is called"); 
        if (spawnedPlayerIndex == TotalPlayerCount())
        {
            Debug.Log("CheckAllPlayersSpawned is called");
            // All players have been spawned, start the card manager
            CardManager.Instance.StartCardManager();
            Debug.Log("chackallplayerspwaned calls cardstartmanager");
        }
    }

    // Method to clean up players, if necessary
    public void CleanupPlayers()
    {
        players.Clear();
    }
}
