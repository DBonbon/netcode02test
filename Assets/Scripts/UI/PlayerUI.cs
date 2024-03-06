using Unity.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PlayerUI : MonoBehaviour
{
    // Personal UI Elements
    #region Personal
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerImage;
    [SerializeField] private TextMeshProUGUI scoreText;
    #endregion

    // Turn UI Elements
    #region Turn
    [SerializeField] private TMP_Dropdown cardsDropdown; 
    [SerializeField] private TMP_Dropdown playersDropdown;
    [SerializeField] private GameObject hasTurnIndicator;
    [SerializeField] private Button guessButton;
    #endregion

    // Hand UI Elements
    #region Hand
    [SerializeField] private Transform cardDisplayTransform;
    #endregion

    //lists to retreive cards value in the guessbuttonclickhandler
    private List<Card> CardsPlayerCanAsk;
    private List<Player> PlayerToAsk;

    private List<int> playerIDs = new List<int>();
    private List<int> cardIDs = new List<int>();

    private const string DefaultImagePath = "Images/character_01";

    public Player currentPlayer;

    public void InitializePlayerUI(string playerName, string imagePath)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
            Debug.Log("UpdatePlayerDbAttributes_ClientRpc was called");
        }

        if (!string.IsNullOrEmpty(imagePath))
        {
            var imageSprite = Resources.Load<Sprite>(imagePath);
            if (playerImage != null && imageSprite != null)
            {
                playerImage.sprite = imageSprite;
            }
        }
    }

    public void InitializeTurnUI(Player player)
    {
        currentPlayer = player;

        // Assign CardsPlayerCanAsk and PlayerToAsk lists
        CardsPlayerCanAsk = player.CardsPlayerCanAsk;
        PlayerToAsk = player.PlayerToAsk;
        
        //UpdatePlayersDropdown(currentPlayer.PlayerToAsk); // Pass the list of player names

        // Update the cardsDropdown with cards that the player can ask
        //UpdateCardsDropdown(currentPlayer.CardsPlayerCanAsk);

        // Deactivate the turn UI objects for players without turn
        if (!player.HasTurn.Value) // Changed from !player.hasTurn to !player.HasTurn.Value
        {
            cardsDropdown.gameObject.SetActive(false);
            playersDropdown.gameObject.SetActive(false);
            guessButton.gameObject.SetActive(false);
        }
        else
        {
            cardsDropdown.gameObject.SetActive(true);
            playersDropdown.gameObject.SetActive(true);
            guessButton.gameObject.SetActive(true);
        }

    }

    // New method to update UI based on card IDs
    public void UpdatePlayerHandUIWithIDs(List<int> cardIDs)
    {
        foreach (Transform child in cardDisplayTransform)
        {
            child.gameObject.SetActive(false);
        }
        Debug.Log("playerui UpdatePlayerHandUIWithIDs is called");
        foreach (int cardID in cardIDs)
        {
            Debug.Log("playerui UpdatePlayerHandUIWithIDs is called in the loop");
            CardUI cardUI = CardManager.Instance.FetchCardUIById(cardID);
            if (cardUI != null)
            {
                cardUI.gameObject.SetActive(true);
                cardUI.transform.SetParent(cardDisplayTransform, false);
            }
            else
            {
                Debug.LogWarning($"No CardUI found for card ID: {cardID}");
            }
        }
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void UpdateHasTurnUI(bool hasTurn)
    {
        // Assuming hasTurnIndicator is a GameObject that indicates the player's turn
        if (hasTurnIndicator != null)
        {
            hasTurnIndicator.SetActive(hasTurn);
        }

        // Activate or deactivate turn UI objects based on hasTurn value
        cardsDropdown.gameObject.SetActive(hasTurn);
        playersDropdown.gameObject.SetActive(hasTurn);
        guessButton.gameObject.SetActive(hasTurn);
    }

    public void UpdatePlayersDropdownWithIDs(ulong[] playerIDs)
    {
        Debug.Log($"Updating players dropdown. IDs count: {playerIDs.Length}");
        playersDropdown.ClearOptions();
        List<string> playerNames = new List<string>();

        foreach (ulong id in playerIDs)
        {
            string playerName = PlayerManager.Instance.GetPlayerNameByClientId(id); // Ensure this method is correctly implemented
            Debug.Log($"Player ID: {id}, Name: {playerName}");
            playerNames.Add(playerName);
        }

        playersDropdown.AddOptions(playerNames);
    }

    public void UpdateCardsDropdownWithIDs(int[] cardIDs)
    {
        Debug.Log($"Updating cards dropdown. IDs count: {cardIDs.Length}");
        cardsDropdown.ClearOptions();
        List<string> cardNames = new List<string>();

        foreach (int id in cardIDs)
        {
            string cardName = CardManager.Instance.GetCardNameById(id); // Ensure this method is correctly implemented
            Debug.Log($"Card ID: {id}, Name: {cardName}");
            cardNames.Add(cardName);
        }

        cardsDropdown.AddOptions(cardNames);
    }

    /*public void UpdatePlayersDropdown(List<Player> updatedPlayersToAsk)
    {
        Debug.Log("UpdatePlayersDropdown is called");
        if (updatedPlayersToAsk != null && updatedPlayersToAsk.Count > 0)
        {
            // Clear the current dropdown options and lists
            playersDropdown.ClearOptions();
            playerIDs.Clear();

            // Add the player names to the dropdown and store their IDs
            List<string> playerNames = updatedPlayersToAsk.Select(player =>
            {
                playerIDs.Add(player.PlayerDbId.Value);
                return player.PlayerName.Value.ToString();
            }).ToList();

            playersDropdown.AddOptions(playerNames);
        }
        else
        {
            Debug.LogWarning("Updated players list is null or empty.");
        }
    }

    public void UpdateCardsDropdown(List<Card> cards)
    {
        if (cardsDropdown != null)
        {
            cardsDropdown.ClearOptions(); // Clearing the dropdown options
            cardIDs.Clear(); // Clearing the associated IDs list

            if (cards != null)
            {
                List<string> cardNames = new List<string>();

                foreach (Card card in cards)
                {
                    cardIDs.Add(card.cardId.Value);

                    // Add the card name to the dropdown options
                    string cardName = card.cardName.Value.ToString();
                    cardNames.Add(cardName); 

                    // Debug the card names to ensure they are correct
                    Debug.Log($"Added card name: {cardName} to dropdown options.");
                }

                // Debug the options passed to the dropdown
                Debug.Log($"Options passed to cardsDropdown: {string.Join(", ", cardNames)}");

                // Update the dropdown options with the card names
                cardsDropdown.AddOptions(cardNames);
            }
            else
            {
                Debug.LogWarning("UpdateCardsDropdown - cards is null");
            }
        }
    }*/


}