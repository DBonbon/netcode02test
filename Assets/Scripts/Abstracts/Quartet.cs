using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Quartet : NetworkBehaviour
{
    [SerializeField] public Transform cardsContainer; // Assign in the inspector

    public List<Card> QuartetCards { get; set; } = new List<Card>();

    private QuartetUI quartetUI;

    public override void OnNetworkSpawn()
    { 
        base.OnNetworkSpawn();
        quartetUI = GetComponent<QuartetUI>(); // Get the QuartetUI component
    }

    public void AddCardToQuartet(Card card)
    {
        if (card != null && IsServer)
        {
            QuartetCards.Add(card);
            Debug.Log($"Card {card.CardName} added to QuartetCards list.");

            // Immediately update the list of card IDs and UI
            UpdateQuartetUI();
        }
        else
        {
            Debug.LogError("Attempted to add a null card to Quartet.");
        }
    }

    private void UpdateQuartetUI()
    {
        if (IsServer)
        {
            List<int> cardIDs = QuartetCards.Select(card => card.cardId.Value).ToList();
            quartetUI.UpdateQuartetUIWithIDs(cardIDs);
            // Synchronize this list with clients
            UpdateQuartetUIOnClients_ClientRpc(cardIDs.ToArray());
        }
    }

    /*private void UpdateQuartetUI()
    {
        List<int> cardIDs = QuartetCards.Select(card => card.cardId.Value).ToList();
        if (quartetUI != null)
        {
            quartetUI.UpdateQuartetUIWithIDs(cardIDs);
            UpdateQuartetUIOnClients_ClientRpc(cardIDs.ToArray());
        }
    }*/

    [ClientRpc]
    private void UpdateQuartetUIOnClients_ClientRpc(int[] cardIDs)
    {
        if (quartetUI != null)
        {
            quartetUI.UpdateQuartetUIWithIDs(new List<int>(cardIDs));
        }
    }
    
    /*
    private void UpdateQuartetUIOnAllClients()
    {
        int[] cardIDs = QuartetCards.Select(card => card.GetComponent<Card>().cardId.Value).ToArray();
        quartetUI.UpdateQuartetUIWithIDs(cardIDs.ToList()); // Continue to use List on the server side for convenience

        UpdateQuartetUIOnClients_ClientRpc(cardIDs); // Convert to array for RPC call
    }

    [ClientRpc]
    private void UpdateQuartetUIOnClients_ClientRpc(int[] cardIDs)
    {
        if (quartetUI != null)
        {
            quartetUI.UpdateQuartetUIWithIDs(new List<int>(cardIDs)); // Convert back to List for the method call
        }
    }*/
}