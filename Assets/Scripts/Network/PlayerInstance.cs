using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInstance
{
    public PlayerData Data { get; private set; }
    public int Score { get; private set; }
    public bool HasTurn { get; private set; }
    public bool IsWinner { get; private set; }
    public int Result { get; private set; }
    
    public List<CardInstance> HandCards { get; set; } = new List<CardInstance>();
    public List<PlayerInstance> PlayersToAsk { get; private set; } = new List<PlayerInstance>();
    public List<CardInstance> CardsPlayerCanAsk { get; private set; } = new List<CardInstance>();
    public List<CardInstance> Quartets { get; private set; } = new List<CardInstance>();

    private Player playerComponent;
    private PlayerUI playerUI;

    public PlayerInstance(PlayerData data)
    {
        Data = data;
        Score = 0;
        HasTurn = false;
    }

    // In PlayerInstance.cs - ADD this new method
    public void IncrementScore()
    {
        Score += 1; // Update local Score
        
        // Update the network Score through Player component
        if (playerComponent != null && playerComponent.IsServer)
        {
            playerComponent.Score.Value = Score;
        }
        
        Debug.Log($"PlayerInstance: Incremented Score to {Score}");
    }

    // In PlayerInstance.cs - ADD this new method
    public bool IsHandEmpty()
    {
        return HandCards.Count == 0;
    }

    public void SetComponents(Player player, PlayerUI ui)
    {
        playerComponent = player;
        playerUI = ui;
    }

    // In PlayerInstance.cs - ADD this new method
// In PlayerInstance.cs - REPLACE the AddCardToHand_New method
    public void AddCardToHand(CardInstance card) 
    {
        if (card != null)
        {
            // Update PlayerInstance HandCards
            if (!HandCards.Contains(card))
            {
                HandCards.Add(card);
            }
            
            // Also update Player component HandCards (for now, during transition)
            if (playerComponent != null && !playerComponent.HandCards.Contains(card))
            {
                playerComponent.HandCards.Add(card);
            }
            
            // Handle CardUI
            CardUI cardUI = CardManager.Instance.FetchCardUIById(card.cardId.Value);
            if (cardUI != null)
            {
                cardUI.gameObject.SetActive(true);
                cardUI.SetFaceUp(playerComponent != null && playerComponent.IsOwner);
            }
            
            CheckForQuartets();
            UpdateCardsPlayerCanAsk();
            
            Debug.Log($"PlayerInstance: Added card {card.cardName.Value} to hand");
        }
    }

    // In PlayerInstance.cs - ADD this new method
    public void RemoveCardFromHand(CardInstance card)
    {
        if (card != null)
        {
            // Remove from PlayerInstance HandCards
            HandCards.Remove(card);
            
            // Also remove from Player component HandCards (during transition)
            if (playerComponent != null)
            {
                playerComponent.HandCards.Remove(card);
            }
            
            UpdateCardsPlayerCanAsk();
            
            Debug.Log($"PlayerInstance: Removed card {card.cardName.Value} from hand");
        }
    }
    
    // In PlayerInstance.cs - ADD this new method
    public void CheckForQuartets()
    {
        // Group cards by their Suit value
        var groupedBySuit = HandCards.GroupBy(card => card.suit.Value.ToString());

        foreach (var suitGroup in groupedBySuit)
        {
            if (suitGroup.Count() == 4) // Exactly 4 cards of the same suit
            {
                MoveCardsToQuartetsArea(suitGroup.ToList());
            }
        }
    }

    // We'll also need to add this helper method
    public void MoveCardsToQuartetsArea(List<CardInstance> quartets)
    {
        Quartets quartetZone = QuartetManager.Instance.QuartetInstance.GetComponent<Quartets>();
        if (quartetZone == null)
        {
            Debug.LogError("Quartets zone not found.");
            return;
        }

        foreach (var card in quartets)
        {
            RemoveCardFromHand(card);
            quartetZone.AddCardToQuartet(card);
            Debug.Log($"PlayerInstance: Moved card {card.cardName.Value} to Quartets.");
        }
        IncrementScore();
    }

    public void UpdatePlayersToAsk(List<PlayerInstance> allPlayers)
    {
        PlayersToAsk.Clear();
        
        foreach (var player in allPlayers)
        {
            if (player != this)
            {
                PlayersToAsk.Add(player);
            }
        }
    }

    // In PlayerInstance.cs - ADD this new method
    public void UpdateCardsPlayerCanAsk()
    {
        // Ensure CardsPlayerCanAsk is initialized
        if (CardsPlayerCanAsk == null)
        {
            CardsPlayerCanAsk = new List<CardInstance>();
        }
        else
        {
            CardsPlayerCanAsk.Clear();
        }
    
        var allCardComponents = CardManager.Instance.allSpawnedCards;

        foreach (var card in allCardComponents)
        {
            if (HandCards.Any(handCard => handCard.suit.Value == card.suit.Value) && !HandCards.Contains(card))
            {
                CardsPlayerCanAsk.Add(card);
            }
        }
        
        // Update the Player component's list (during transition)
        if (playerComponent != null)
        {
            playerComponent.CardsPlayerCanAsk.Clear();
            playerComponent.CardsPlayerCanAsk.AddRange(CardsPlayerCanAsk);
            
            // If this player has turn, update the dropdown
            if (playerComponent.IsServer && playerComponent.HasTurn.Value)
            {
                int[] cardIDs = CardsPlayerCanAsk.Select(card => card.cardId.Value).ToArray();
                playerComponent.UpdateCardDropdown_ClientRpc(cardIDs);
            }
        }
        
        Debug.Log($"PlayerInstance: Player can ask for {CardsPlayerCanAsk.Count} cards based on suits.");
    }

    // In PlayerInstance.cs - ADD this new method
    public void UpdatePlayerToAskList(List<Player> allPlayers)
    {
        // Clear the PlayerInstance list
        PlayersToAsk.Clear();
        
        // Also update the Player component list (during transition)
        if (playerComponent != null)
        {
            playerComponent.PlayerToAsk.Clear();
            
            foreach (var potentialPlayer in allPlayers)
            {
                if (potentialPlayer != playerComponent)
                {
                    playerComponent.PlayerToAsk.Add(potentialPlayer);
                    Debug.Log($"PlayerInstance: Added {potentialPlayer.playerName.Value} to PlayerToAsk.");
                }
            }

            // If this player has turn, update the dropdown
            if (playerComponent.IsServer && playerComponent.HasTurn.Value)
            {
                ulong[] playerIDs = playerComponent.PlayerToAsk.Select(player => player.OwnerClientId).ToArray();
                string playerNamesConcatenated = string.Join(",", playerComponent.PlayerToAsk.Select(player => player.playerName.Value.ToString()));
                playerComponent.TurnUIForPlayer_ClientRpc(playerIDs, playerNamesConcatenated);
            }
        }
    }

}