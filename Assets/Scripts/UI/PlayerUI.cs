using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playerTestText;
    public TextMeshProUGUI playerNameText; // New field for player name
    public TextMeshProUGUI playerScoreText; // New field for player score
    [SerializeField] private Transform playerHandTransform;

    public void SetupPlayer(ulong playerId, string name, int score, bool isLocalPlayer, int totalPlayers, bool isServer)
    {
        playerNameText.text = name;
        playerScoreText.text = "Score: " + score;
        //UpdateHandUI(List<ulong>);

        Vector3 position = DeterminePosition(playerId, isLocalPlayer, totalPlayers, isServer);
        transform.position = position;
    }

    // PlayerUI objects - Local
    public void UpdateHandUI(List<CardUI> handCardUIs)
    {
        Debug.Log($"Updating player UI hand with {handCardUIs.Count} cards. PlayerHandTransform is null: {playerHandTransform == null}");
        foreach (var cardUI in handCardUIs)
        {
            cardUI.transform.SetParent(playerHandTransform, false);
        }
    }

    //General local, remote display
    private Vector3 DeterminePosition(ulong playerId, bool isLocalPlayer, int totalPlayers, bool isServer)
    {
        // Handle positioning differently on the server
        if (isServer)
        {
            int order = (int)(playerId % (ulong)totalPlayers);
            switch (order)
            {
                case 0: return new Vector3(Screen.width / 2, 50, 0); // Bottom
                case 1: return new Vector3(Screen.width - 50, Screen.height / 2, 0); // Right
                case 2: return new Vector3(Screen.width / 2, Screen.height - 50, 0); // Top
                case 3: return new Vector3(50, Screen.height / 2, 0); // Left
                default: return Vector3.zero;
            }
        }

        // Local player is always at the bottom
        if (isLocalPlayer)
        {
            return new Vector3(Screen.width / 2, 50, 0); // Adjust as needed
        }

        // Logic for remote players
        switch (totalPlayers)
        {
            case 2:
                return new Vector3(Screen.width / 2, Screen.height - 50, 0); // Top
            case 3:
                if (playerId % 3 == 1)
                    return new Vector3(Screen.width - 50, Screen.height / 2, 0); // Right
                else
                    return new Vector3(Screen.width / 2, Screen.height - 50, 0); // Top
            case 4:
                if (playerId % 4 == 1)
                    return new Vector3(Screen.width - 50, Screen.height / 2, 0); // Right
                else if (playerId % 4 == 2)
                    return new Vector3(Screen.width / 2, Screen.height - 50, 0); // Top
                else
                    return new Vector3(50, Screen.height / 2, 0); // Left
            default:
                return Vector3.zero; // Default position
        }
    }

    public void UpdatePlayerTest(int newValue)
    {
        playerTestText.text = "Player Test Value: " + newValue;
    }
}