using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuartetUI : MonoBehaviour
{
    [SerializeField] private Transform quartetDisplayTransform; // UI container for quartet cards. this is a Fallback or Default option, i.e. since the transform is passed from the quartetmanager

     // New method to set the quartetDisplayTransform
    public void SetQuartetDisplayTransform(Transform newDisplayTransform)
    {
        quartetDisplayTransform = newDisplayTransform;
    }
    
    // Method to be called to update the UI representation of the quartet
    public void UpdateQuartetUIWithIDs(List<int> cardIDs)
    {
        // Deactivate all child GameObjects to reset the state
        foreach (Transform child in quartetDisplayTransform)
        {
            child.gameObject.SetActive(false); // Disable all cards initially
        }

        // Activate and reparent the CardUI objects based on the current quartet
        foreach (int cardID in cardIDs)
        {
            CardUI cardUI = CardManager.Instance.FetchCardUIById(cardID);
            if (cardUI != null)
            {
                cardUI.gameObject.SetActive(true);
                cardUI.transform.SetParent(quartetDisplayTransform, false);
            }
            else
            {
                Debug.LogWarning($"No CardUI found for card ID: {cardID}");
            }
        }
    }
    
}