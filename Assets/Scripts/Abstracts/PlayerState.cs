using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    // Pure data storage - no logic
    public PlayerData Data { get; private set; }
    public List<CardInstance> HandCards { get; private set; }
    public int Score { get; private set; }
    public bool HasTurn { get; private set; }
    public List<CardInstance> Quartets { get; private set; }
    public List<CardInstance> CardsPlayerCanAsk { get; private set; }
    
    // ADD these missing fields
    public bool IsWinner { get; private set; }
    public int Result { get; private set; }
    
    // Constructor
    public PlayerState(PlayerData data) 
    {
        Data = data;
        HandCards = new List<CardInstance>();
        CardsPlayerCanAsk = new List<CardInstance>();
        Quartets = new List<CardInstance>();
        Score = 0;
        HasTurn = false;
        IsWinner = false;        // ADD
        Result = 0;              // ADD
    }
    
    // Simple setters - no complex logic
    public void SetScore(int newScore) { Score = newScore; }
    public void SetTurn(bool hasTurn) { HasTurn = hasTurn; }
    public void SetWinner(bool isWinner) { IsWinner = isWinner; }    // ADD
    public void SetResult(int result) { Result = result; }          // ADD
    
    // Simple operations
    public void AddCardToHand(CardInstance card)
    {
        if (card != null && !HandCards.Contains(card))
        {
            HandCards.Add(card);
        }
    }
    
    public void RemoveCardFromHand(CardInstance card)
    {
        if (card != null)
        {
            HandCards.Remove(card);
        }
    }
    
    // ADD these missing methods
    public void ClearCardsPlayerCanAsk()
    {
        CardsPlayerCanAsk.Clear();
    }
    
    public void AddCardPlayerCanAsk(CardInstance card)
    {
        if (card != null && !CardsPlayerCanAsk.Contains(card))
        {
            CardsPlayerCanAsk.Add(card);
        }
    }
    
    public void AddToQuartets(CardInstance card)
    {
        if (card != null && !Quartets.Contains(card))
        {
            Quartets.Add(card);
        }
    }
}