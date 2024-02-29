using Unity.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerImage;
    [SerializeField] private TextMeshProUGUI scoreText; // Add UI element for score
    [SerializeField] private GameObject hasTurnIndicator; // Add UI element for turn indicator

    [SerializeField] private Transform cardDisplayTransform; // Container for card UI elements

    private const string DefaultImagePath = "Images/character_01";

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
    
    /*public void UpdatePlayerHandUI(List<Card> handCards)
    {
        // Deactivate all card UIs currently being displayed to reset the state.
        foreach (Transform child in cardDisplayTransform)
        {
            child.gameObject.SetActive(false);
        }

        // For each Card in the player's hand, find and activate the corresponding CardUI from the pool
        foreach (Card card in handCards)
        {
            // Use FetchCardUIById instead of FetchCardUIByName
            CardUI cardUI = CardManager.Instance.FetchCardUIById(card.cardId.Value);
            if (cardUI != null)
            {
                cardUI.gameObject.SetActive(true); // Make the CardUI visible
                cardUI.transform.SetParent(cardDisplayTransform, false); // Reparent the CardUI to the player's UI
            }
            else
            {
                Debug.LogWarning($"No CardUI found for card ID: {card.cardId.Value}");
            }
        }
    }*/

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void UpdateHasTurnUI(bool hasTurn)
    {
        if (hasTurnIndicator != null)
        {
            hasTurnIndicator.SetActive(hasTurn);
        }
    }
}