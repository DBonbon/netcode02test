using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private Transform deckDisplayTransform; // UI container for deck cards. this is a Fallback or Default option, i.e. since the transform is passed from the deckmanager

     // New method to set the deckDisplayTransform
    public void SetDeckDisplayTransform(Transform newDisplayTransform)
    {
        deckDisplayTransform = newDisplayTransform;
    }
    
    // Method to be called to update the UI representation of the deck
    public void UpdateDeckUIWithIDs(List<int> cardIDs)
    {
        // Deactivate all child GameObjects to reset the state
        foreach (Transform child in deckDisplayTransform)
        {
            child.gameObject.SetActive(false); // Disable all cards initially
        }

        // Activate and reparent the CardUI objects based on the current deck
        foreach (int cardID in cardIDs)
        {
            CardUI cardUI = CardManager.Instance.FetchCardUIById(cardID);
            if (cardUI != null)
            {
                cardUI.gameObject.SetActive(true);
                cardUI.transform.SetParent(deckDisplayTransform, false);
            }
            else
            {
                Debug.LogWarning($"No CardUI found for card ID: {cardID}");
            }
        }
    }
    
}