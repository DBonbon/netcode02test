using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // The brain that coordinates everything
    private PlayerState playerState;
    
    // Components this controller manages
    private Player playerComponent;
    private PlayerUI playerUI;

    // Coordination-only data (not player state)
    public List<PlayerController> PlayersToAsk { get; private set; } = new List<PlayerController>();

    // Initialize with player data
    public void Initialize(PlayerData data)
    {
        playerState = new PlayerState(data);
        playerComponent = GetComponent<Player>();
        playerUI = GetComponent<PlayerUI>();
        PlayersToAsk = new List<PlayerController>();
        
        Debug.Log($"PlayerController initialized for {data.playerName}");
    }

    // Access to data for other systems
    public PlayerState State => playerState;
    
    // Expose common properties for easy access
    public PlayerData Data => playerState.Data;
    public int Score => playerState.Score;
    public bool HasTurn => playerState.HasTurn;
    public bool IsWinner => playerState.IsWinner;
    public int Result => playerState.Result;
    public List<CardInstance> HandCards => playerState.HandCards;
    public List<CardInstance> CardsPlayerCanAsk => playerState.CardsPlayerCanAsk;
    public List<CardInstance> Quartets => playerState.Quartets;

    // COORDINATION METHODS - The core controller logic
    public void IncrementScore()
    {
        // 1. Update data
        playerState.SetScore(playerState.Score + 1);
        
        // 2. Sync to network
        if (playerComponent != null && playerComponent.IsServer)
        {
            playerComponent.Score.Value = playerState.Score;
        }
        
        Debug.Log($"PlayerController: Incremented Score to {playerState.Score}");
    }

    public bool IsHandEmpty()
    {
        return playerState.HandCards.Count == 0;
    }

    public void AddCardToHand(CardInstance card) 
    {
        if (card == null) return;
        
        // 1. Update data
        playerState.AddCardToHand(card);
        
        // 2. Update network component (during transition)
        if (playerComponent != null && !playerComponent.HandCards.Contains(card))
        {
            playerComponent.HandCards.Add(card);
        }
        
        // 3. Update UI
        CardUI cardUI = CardManager.Instance.FetchCardUIById(card.cardId.Value);
        if (cardUI != null)
        {
            cardUI.gameObject.SetActive(true);
            cardUI.SetFaceUp(playerComponent != null && playerComponent.IsOwner);
        }
        
        // 4. Check game consequences
        CheckForQuartets();
        UpdateCardsPlayerCanAsk();
        
        Debug.Log($"PlayerController: Added card {card.cardName.Value} to hand");
    }

    public void RemoveCardFromHand(CardInstance card)
    {
        if (card == null) return;
        
        // 1. Update data
        playerState.RemoveCardFromHand(card);
        
        // 2. Update network component (during transition)
        if (playerComponent != null)
        {
            playerComponent.HandCards.Remove(card);
        }
        
        // 3. Update game state
        UpdateCardsPlayerCanAsk();
        
        Debug.Log($"PlayerController: Removed card {card.cardName.Value} from hand");
    }
    
    public void CheckForQuartets()
    {
        // Game logic coordination
        var groupedBySuit = playerState.HandCards.GroupBy(card => card.suit.Value.ToString());

        foreach (var suitGroup in groupedBySuit)
        {
            if (suitGroup.Count() == 4)
            {
                MoveCardsToQuartetsArea(suitGroup.ToList());
            }
        }
    }

    public void MoveCardsToQuartetsArea(List<CardInstance> quartets)
    {
        // Coordinate the quartet move
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
            playerState.AddToQuartets(card);
            Debug.Log($"PlayerController: Moved card {card.cardName.Value} to Quartets.");
        }
        IncrementScore();
    }

    public void UpdatePlayersToAsk(List<PlayerController> allPlayers)
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

    public void UpdateCardsPlayerCanAsk()
    {
        // 1. Clear data
        playerState.ClearCardsPlayerCanAsk();
    
        // 2. Calculate what cards can be asked for
        var allCardComponents = CardManager.Instance.allSpawnedCards;
        foreach (var card in allCardComponents)
        {
            if (playerState.HandCards.Any(handCard => handCard.suit.Value == card.suit.Value) 
                && !playerState.HandCards.Contains(card))
            {
                playerState.AddCardPlayerCanAsk(card);
            }
        }
        
        // 3. Update network component (during transition)
        if (playerComponent != null)
        {
            playerComponent.CardsPlayerCanAsk.Clear();
            playerComponent.CardsPlayerCanAsk.AddRange(playerState.CardsPlayerCanAsk);
            
            // 4. Update UI if needed
            if (playerComponent.IsServer && playerComponent.HasTurn.Value)
            {
                int[] cardIDs = playerState.CardsPlayerCanAsk.Select(card => card.cardId.Value).ToArray();
                playerComponent.UpdateCardDropdown_ClientRpc(cardIDs);
            }
        }
        
        Debug.Log($"PlayerController: Player can ask for {playerState.CardsPlayerCanAsk.Count} cards based on suits.");
    }

    public void UpdatePlayerToAskList(List<Player> allPlayers)
    {
        // Coordination logic for UI updates
        if (playerComponent != null)
        {
            playerComponent.PlayerToAsk.Clear();
            
            foreach (var potentialPlayer in allPlayers)
            {
                if (potentialPlayer != playerComponent)
                {
                    playerComponent.PlayerToAsk.Add(potentialPlayer);
                    Debug.Log($"PlayerController: Added {potentialPlayer.playerName.Value} to PlayerToAsk.");
                }
            }

            // Update UI if it's this player's turn
            if (playerComponent.IsServer && playerComponent.HasTurn.Value)
            {
                ulong[] playerIDs = playerComponent.PlayerToAsk.Select(player => player.OwnerClientId).ToArray();
                string playerNamesConcatenated = string.Join(",", playerComponent.PlayerToAsk.Select(player => player.playerName.Value.ToString()));
                playerComponent.TurnUIForPlayer_ClientRpc(playerIDs, playerNamesConcatenated);
            }
        }
    }
}