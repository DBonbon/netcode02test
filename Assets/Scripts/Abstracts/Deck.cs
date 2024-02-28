using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Deck : NetworkBehaviour
{
    [SerializeField] public Transform cardsContainer; // Assign in the inspector

    public List<GameObject> DeckCards = new List<GameObject>();

    private DeckUI deckUI;

    public override void OnNetworkSpawn()
    { 
        base.OnNetworkSpawn();
        deckUI = GetComponent<DeckUI>(); // Get the DeckUI component
    }

    public void AddCardToDeck(GameObject card)
    {
        if (IsServer)
        {
            DeckCards.Add(card);
            card.transform.SetParent(cardsContainer, false);

            // Optionally, send update to clients about the new card
            UpdateDeckUIOnAllClients();
        }
    }

    public void RemoveCardFromDeck(GameObject card)
    {
        if (IsServer && DeckCards.Contains(card))
        {
            DeckCards.Remove(card);

            // Optionally, send update to clients about the removed card
            UpdateDeckUIOnAllClients();
        }
    }

    private void UpdateDeckUIOnAllClients()
    {
        int[] cardIDs = DeckCards.Select(card => card.GetComponent<Card>().cardId.Value).ToArray();
        deckUI.UpdateDeckUIWithIDs(cardIDs.ToList()); // Continue to use List on the server side for convenience

        UpdateDeckUIOnClients_ClientRpc(cardIDs); // Convert to array for RPC call
    }

    [ClientRpc]
    private void UpdateDeckUIOnClients_ClientRpc(int[] cardIDs)
    {
        if (deckUI != null)
        {
            deckUI.UpdateDeckUIWithIDs(new List<int>(cardIDs)); // Convert back to List for the method call
        }
    }
}