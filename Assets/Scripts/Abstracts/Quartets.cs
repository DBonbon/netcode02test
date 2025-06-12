using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// ToDo change name and location as this acts as Quartetsneteworkmanager

public class Quartets : NetworkBehaviour
{
    [SerializeField] public Transform cardsContainer; // Assign in the inspector

    public List<CardInstance> QuartetCards { get; set; } = new List<CardInstance>();

    private QuartetUI quartetUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        quartetUI = GetComponent<QuartetUI>(); // Get the QuartetUI component
    }

    public void AddCardToQuartet(CardInstance card)
    {
        if (card != null && IsServer)
        {
            QuartetCards.Add(card);
            Debug.Log($"Card {card.cardName} added to QuartetCards list.");

            // Immediately update the list of card IDs and UI
            UpdateQuartetUI();
        }
        else
        {
            Debug.LogError("Attempted to add a null card to Quartets.");
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

    [ClientRpc]
    private void UpdateQuartetUIOnClients_ClientRpc(int[] cardIDs)
    {
        if (quartetUI != null)
        {
            quartetUI.UpdateQuartetUIWithIDs(new List<int>(cardIDs));
        }
    }
}