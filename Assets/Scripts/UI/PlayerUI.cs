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

    // In PlayerUI
    private List<ulong> playerIdsInDropdown = new List<ulong>();


    public Player currentPlayer;

    void Awake()
    {
        // Add an onClick handler to the guessButton
        guessButton.onClick.AddListener(OnEventGuessClick);
    }
    
    public void InitializePlayerUI(string playerName, string imagePath)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
            Debug.Log("UpdatePlayerDbAttributes_ClientRpc was called");
            playersDropdown.gameObject.SetActive(false);
            cardsDropdown.gameObject.SetActive(false);
            guessButton.gameObject.SetActive(false);
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

    public void UpdateTurnUI(bool hasTurn)
    {
        // This ensures that the dropdown is only active for the player with the turn.
        if (hasTurnIndicator != null)
        {
            hasTurnIndicator.SetActive(hasTurn);
        }
        ActivateTurnUI(hasTurn); // Make sure this method is responsible for showing/hiding relevant UI elements.
    }

    public void ActivateTurnUI(bool hasTurn)
    {
        // Assuming 'playersDropdown' is part of a larger turn UI GameObject
        playersDropdown.gameObject.SetActive(hasTurn); // Activate or deactivate the turn UI
        cardsDropdown.gameObject.SetActive(hasTurn);
        guessButton.gameObject.SetActive(hasTurn); 
        Debug.Log($"ActivateTurnUI running {hasTurn}");
    }

    public void UpdatePlayersDropdown(ulong[] playerIDs, string[] playerNames)
    {
        Debug.Log($"Updating players dropdown. IDs count: {playerIDs.Length}, Names count: {playerNames.Length}");
        playersDropdown.ClearOptions();
        List<string> playerNamesList = new List<string>(playerNames);

        /*foreach (var playerId in playerIDs)
        {
            Debug.Log($"player id is: {playerId}");
            string playerName = PlayerManager.Instance.GetPlayerNameByClientId(playerId);
            Debug.Log($"playerName from teh update method is: {playerName}");
            playerNamesList.Add(playerName); // Use the name from the dictionary lookup
            Debug.Log($"Player ID is the: {playerId}, Player Name is the: {playerName}"); // Debug to verify names
        }*/

        for(int i = 0; i < playerIDs.Length; i++)
        {
            playerIdsInDropdown.Add(playerIDs[i]);
        }
            
        playersDropdown.AddOptions(playerNamesList);
        // This log helps you verify that the dropdown options have been successfully updated.
        Debug.Log("Players Dropdown updated with names: " + string.Join(", ", playerNames));
        
    }

    /*public void UpdatePlayersDropdown(ulong[] playerIDs)
    {
        Debug.Log($"Updating players dropdown. IDs count: {playerIDs.Length}");
        playersDropdown.ClearOptions();
        List<string> playerNamesList = new List<string>();

        foreach (var playerId in playerIDs)
        {
            Debug.Log($"Print player ID: {playerId}");
            string playerName = PlayerManager.Instance.GetPlayerNameByClientId(playerId);
            Debug.Log($"Fetching name for Player ID {playerId}: {playerName}");

            playerNamesList.Add(playerName);
        }
        
        playersDropdown.AddOptions(playerNamesList);
        Debug.Log("Players Dropdown updated with names: " + string.Join(", ", playerNamesList));
    }*/

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
            this.cardIDs.Add(id); // Maintain ID mapping
        }

        cardsDropdown.AddOptions(cardNames);
    }

    //event
    public void OnEventGuessClick()
    {
        Debug.Log("Guess button clicked!");
        // Check if it's the player's turn
        /*if (!currentPlayer.HasTurn.Value)
        {
            Debug.LogWarning("Not the player's turn.");
            return;
        }*/
        // Disable the button to prevent multiple clicks
        //guessButton.interactable = false; // Disable the guess button

        // Get the selected index from the dropdowns
        int selectedPlayerIndex = playersDropdown.value;
        Debug.Log($"selected player value is: {selectedPlayerIndex}");
        int selectedCardIndex = cardsDropdown.value;
        Debug.Log($"selected card value is: {selectedCardIndex}");

        //if (selectedPlayerIndex < 0 || selectedPlayerIndex >= playerIDs.Count ||
        if (selectedCardIndex < 0 || selectedCardIndex >= cardIDs.Count)
        {
            Debug.LogError("Invalid card dropdown selection.");
            return;
        }

        if (selectedPlayerIndex >= 0 && selectedPlayerIndex < playerIdsInDropdown.Count)
        {
            ulong selectedPlayerId = playerIdsInDropdown[selectedPlayerIndex];
            Debug.Log($"Selected Player Client ID: {selectedPlayerId}");

            // Now you can use selectedPlayerId for further actions
        }
        else
        {
            Debug.LogError("Invalid player selection.");
        }
            //ulong selectedPlayerId = playerIDs[selectedPlayerIndex];
            //Debug.Log($"Selected Player Client ID: {selectedPlayerId}");
            
            // Further actions based on selectedPlayerId
        

        int selectedCardID = cardIDs[selectedCardIndex];
        Debug.Log($"Selected Card ID: {selectedCardID}"); // This should now display correctly

        //Debug.Log($"Selected Card: {selectedCard.cardName}");
        //Debug.Log($"Selected Player: {selectedPlayer.playerName}");

        // Directly call the HandlePlayerTurn method in TurnManager
        var turnManager = TurnManager.Instance; 
        if (turnManager != null)
        {
            //turnManager.OnEventGuessClick(selectedCard, selectedPlayer);
        }
        else
        {
            Debug.LogError("TurnManager not found.");
        }
        
        // Re-enable the button at the end of the method
        guessButton.interactable = true;
    }

}