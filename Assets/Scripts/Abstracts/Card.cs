using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class Card : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> CardName = new NetworkVariable<FixedString32Bytes>("CardName");
    public NetworkVariable<FixedString32Bytes> Suit = new NetworkVariable<FixedString32Bytes>();

    public GameObject cardUIPrefab; // Assign in Inspector
    private CardUI cardUIInstance;

    public override void OnNetworkSpawn()
    {
        // Instantiate CardUI on all clients
        InstantiateCardUI();
    }

    private void InstantiateCardUI()
    {
        // Find the Canvas in the scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (cardUIPrefab != null && canvas != null)
        {
            // Instantiate the CardUI prefab under the Canvas
            GameObject uiObject = Instantiate(cardUIPrefab, canvas.transform);
            cardUIInstance = uiObject.GetComponent<CardUI>();

            // Set initial data for CardUI
            cardUIInstance.SetCardData(CardName.Value.ToString(), Suit.Value.ToString());

            // Set the position and any other properties for the UI object
            cardUIInstance.transform.localPosition = CalculateCardUIPosition();
        }
    }

    // Call this method to update the CardUI when card data changes
    public void UpdateCardUI()
    {
        UpdateCardUIClientRpc(CardName.Value.ToString(), Suit.Value.ToString());
    }

    [ClientRpc]
    private void UpdateCardUIClientRpc(string cardName, string suit)
    {
        if (cardUIInstance != null)
        {
            // Update the CardUI data
            cardUIInstance.SetCardData(cardName, suit);
        }
    }

    private Vector3 CalculateCardUIPosition()
    {
        // Implement logic to calculate the position of the CardUI
        return new Vector3(0, 0, 0);
    }
}