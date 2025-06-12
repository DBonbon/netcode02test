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

    public void SetComponents(Player player, PlayerUI ui)
    {
        playerComponent = player;
        playerUI = ui;
    }

    public void AddCardToHand(CardInstance card)
    {
        if (card != null && !HandCards.Contains(card))
        {
            HandCards.Add(card);
            CheckForQuartets();
            UpdateCardsPlayerCanAsk();
        }
    }

    public void RemoveCardFromHand(CardInstance card)
    {
        if (card != null && HandCards.Remove(card))
        {
            UpdateCardsPlayerCanAsk();
        }
    }

    public void UpdateCardsPlayerCanAsk()
    {
        CardsPlayerCanAsk.Clear();
        
        var allCardComponents = CardManager.Instance.allSpawnedCards
            .Select(go => go.GetComponent<CardInstance>())
            .Where(c => c != null);

        foreach (var card in allCardComponents)
        {
            if (HandCards.Any(handCard => handCard.suit.Value.ToString() == card.suit.Value.ToString()) 
                && !HandCards.Contains(card))
            {
                CardsPlayerCanAsk.Add(card);
            }
        }
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

    public void CheckForQuartets()
    {
        var groupedBySuit = HandCards.GroupBy(card => card.suit.Value.ToString());

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
        if (quartetZone != null)
        {
            foreach (var card in quartets)
            {
                RemoveCardFromHand(card);
                quartetZone.AddCardToQuartet(card);
            }
            IncrementScore();
        }
    }

    public void IncrementScore()
    {
        Score += 1;
        if (playerComponent != null && playerComponent.IsServer)
        {
            playerComponent.Score.Value = Score;
        }
    }

    public bool IsHandEmpty()
    {
        return HandCards.Count == 0;
    }
}