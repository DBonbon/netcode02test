using Unity.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerImage;
    [SerializeField] private TextMeshProUGUI scoreText; // Add UI element for score
    [SerializeField] private GameObject hasTurnIndicator; // Add UI element for turn indicator

    public TMP_Dropdown cardsDropdown; // Dropdown for cards player can ask
    public TMP_Dropdown playersDropdown; // Dropdown for active players
    //public Button guessButton; // Button for guess request
    [SerializeField] public Button guessButton; // Reference to the guess button

    [SerializeField] private Transform cardDisplayTransform; // Container for card UI elements

    //lists to retreive cards value in the guessbuttonclickhandler
    private List<Card> CardsPlayerCanAsk;
    private List<Player> PlayerToAsk;

    private List<int> playerIDs = new List<int>();
    private List<int> cardIDs = new List<int>();

    private const string DefaultImagePath = "Images/character_01";

    public Player currentPlayer
    {
        get { return _currentPlayer; }
        set
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.OnPlayerToAskListUpdated -= UpdatePlayersDropdownHandler;
                _currentPlayer.OnCardsPlayerCanAskListUpdated -= UpdateCardsDropdownHandler;
                Debug.Log("Unsubscribed from player events");
            }

            _currentPlayer = value;

            if (_currentPlayer != null)
            {
                _currentPlayer.OnPlayerToAskListUpdated += UpdatePlayersDropdownHandler;
                _currentPlayer.OnCardsPlayerCanAskListUpdated += UpdateCardsDropdownHandler;
                Debug.Log("Subscribed to new player events");
            }
        }
    }

    private Player _currentPlayer; // Backing field for currentPlayer

    private void OnDestroy()
    {
        // Clean up event subscriptions when the UI is destroyed
        if (_currentPlayer != null)
        {
            _currentPlayer.OnPlayerToAskListUpdated -= UpdatePlayersDropdownHandler;
            _currentPlayer.OnCardsPlayerCanAskListUpdated -= UpdateCardsDropdownHandler;
        }
    }

    public void InitializePlayerUI(string playerName, string imagePath)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
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
        
        UpdatePlayersDropdown(currentPlayer.PlayerToAsk); // Pass the list of player names

        // Update the cardsDropdown with cards that the player can ask
        UpdateCardsDropdown(currentPlayer.CardsPlayerCanAsk);

        // Deactivate the turn UI objects for players without turn
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

    private void UpdatePlayersDropdownHandler()
    {
        Debug.Log("Handling Player to Ask List Updated event.");
        UpdatePlayersDropdown(_currentPlayer.PlayerToAsk);
    }

    private void UpdateCardsDropdownHandler()
    {
        Debug.Log("Handling Cards to Ask List Updated event.");
        UpdateCardsDropdown(_currentPlayer.CardsPlayerCanAsk);
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


    public void UpdatePlayersDropdown(List<Player> updatedPlayersToAsk)
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
    }


}