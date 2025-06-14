using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInstance
{
    // NEW: Use PlayerState for data storage
    private PlayerState playerState;
    
    // Keep coordination components
    private Player playerComponent;
    private PlayerUI playerUI;

    // Keep coordination-only data (not player state)
    public List<PlayerInstance> PlayersToAsk { get; private set; } = new List<PlayerInstance>();

    public PlayerInstance(PlayerData data)
    {
        playerState = new PlayerState(data);
        PlayersToAsk = new List<PlayerInstance>();
    }

    // Access data through PlayerState (remove duplicates)
    public PlayerData Data => playerState.Data;
    public int Score => playerState.Score;
    public bool HasTurn => playerState.HasTurn;
    public bool IsWinner => playerState.IsWinner;
    public int Result => playerState.Result;
    public List<CardInstance> HandCards => playerState.HandCards;
    public List<CardInstance> CardsPlayerCanAsk => playerState.CardsPlayerCanAsk;
    public List<CardInstance> Quartets => playerState.Quartets;

    public void IncrementScore()
    {
        playerState.SetScore(playerState.Score + 1);
        
        if (playerComponent != null && playerComponent.IsServer)
        {
            playerComponent.Score.Value = playerState.Score;
        }
        
        Debug.Log($"PlayerInstance: Incremented Score to {playerState.Score}");
    }

    public bool IsHandEmpty()
    {
        return playerState.HandCards.Count == 0;
    }

    public void SetComponents(Player player, PlayerUI ui)
    {
        playerComponent = player;
        playerUI = ui;
    }

    public void AddCardToHand(CardInstance card) 
    {
        if (card != null)
        {
            playerState.AddCardToHand(card);
            
            if (playerComponent != null && !playerComponent.HandCards.Contains(card))
            {
                playerComponent.HandCards.Add(card);
            }
            
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

    public void RemoveCardFromHand(CardInstance card)
    {
        if (card != null)
        {
            playerState.RemoveCardFromHand(card);  // FIXED: use PlayerState method
            
            if (playerComponent != null)
            {
                playerComponent.HandCards.Remove(card);
            }
            
            UpdateCardsPlayerCanAsk();
            
            Debug.Log($"PlayerInstance: Removed card {card.cardName.Value} from hand");
        }
    }
    
    public void CheckForQuartets()
    {
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
            playerState.AddToQuartets(card);  // FIXED: add to PlayerState quartets
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

    public void UpdateCardsPlayerCanAsk()
    {
        playerState.ClearCardsPlayerCanAsk();  // FIXED: use PlayerState method
    
        var allCardComponents = CardManager.Instance.allSpawnedCards;

        foreach (var card in allCardComponents)
        {
            if (playerState.HandCards.Any(handCard => handCard.suit.Value == card.suit.Value) && !playerState.HandCards.Contains(card))
            {
                playerState.AddCardPlayerCanAsk(card);  // FIXED: use PlayerState method
            }
        }
        
        if (playerComponent != null)
        {
            playerComponent.CardsPlayerCanAsk.Clear();
            playerComponent.CardsPlayerCanAsk.AddRange(playerState.CardsPlayerCanAsk);
            
            if (playerComponent.IsServer && playerComponent.HasTurn.Value)
            {
                int[] cardIDs = playerState.CardsPlayerCanAsk.Select(card => card.cardId.Value).ToArray();
                playerComponent.UpdateCardDropdown_ClientRpc(cardIDs);
            }
        }
        
        Debug.Log($"PlayerInstance: Player can ask for {playerState.CardsPlayerCanAsk.Count} cards based on suits.");
    }

    public void UpdatePlayerToAskList(List<Player> allPlayers)
    {
        PlayersToAsk.Clear();
        
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

            if (playerComponent.IsServer && playerComponent.HasTurn.Value)
            {
                ulong[] playerIDs = playerComponent.PlayerToAsk.Select(player => player.OwnerClientId).ToArray();
                string playerNamesConcatenated = string.Join(",", playerComponent.PlayerToAsk.Select(player => player.playerName.Value.ToString()));
                playerComponent.TurnUIForPlayer_ClientRpc(playerIDs, playerNamesConcatenated);
            }
        }
    }
}