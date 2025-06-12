using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Pure game rules for Quartets - no Unity dependencies, no networking, no UI
/// Contains only the logic of how the Quartets card game works
/// </summary>
public static class QuartetsGameRules
{
    /// <summary>
    /// Check if a guess is valid according to Quartets rules
    /// </summary>
    public static bool IsValidGuess(Player guessingPlayer, Player targetPlayer, Card requestedCard)
    {
        // Rule: Can't guess if you already have the card
        bool alreadyHasCard = guessingPlayer.HandCards.Any(c => c.cardId.Value == requestedCard.cardId.Value);
        
        // Rule: Target must actually have the card
        bool targetHasCard = targetPlayer.HandCards.Contains(requestedCard);
        
        // Rule: Can only ask for cards in suits you already have
        bool hasSameSuit = guessingPlayer.HandCards.Any(c => c.Suit.Value.ToString() == requestedCard.Suit.Value.ToString());
        
        return !alreadyHasCard && targetHasCard && hasSameSuit;
    }
    
    /// <summary>
    /// Process the result of a guess - returns true if guess was correct
    /// </summary>
    public static bool ProcessGuess(Player guessingPlayer, Player targetPlayer, Card requestedCard)
    {
        if (!IsValidGuess(guessingPlayer, targetPlayer, requestedCard))
        {
            Debug.LogWarning("Invalid guess attempted");
            return false;
        }
        
        // Target has the card - guess is correct
        return targetPlayer.HandCards.Contains(requestedCard);
    }
    
    /// <summary>
    /// Check if player has completed any quartets (4 cards of same suit)
    /// Returns list of completed quartet suits
    /// </summary>
    public static List<string> GetCompletedQuartets(Player player)
    {
        var completedSuits = new List<string>();
        
        // Group cards by suit
        var groupedBySuit = player.HandCards.GroupBy(card => card.Suit.Value.ToString());
        
        foreach (var suitGroup in groupedBySuit)
        {
            if (suitGroup.Count() >= 4) // Complete quartet
            {
                completedSuits.Add(suitGroup.Key);
            }
        }
        
        return completedSuits;
    }
    
    /// <summary>
    /// Check if the game should end
    /// Game ends when all cards are in quartets (no cards left in hands or deck)
    /// </summary>
    public static bool IsGameOver(List<Player> players, int cardsLeftInDeck)
    {
        // Game ends when no cards left in any hands AND no cards left in deck
        bool allHandsEmpty = players.All(p => p.HandCards.Count == 0);
        bool deckEmpty = cardsLeftInDeck == 0;
        
        return allHandsEmpty && deckEmpty;
    }
    
    /// <summary>
    /// Determine winner(s) based on quartets completed
    /// </summary>
    public static List<Player> GetWinners(List<Player> players)
    {
        if (players.Count == 0) return new List<Player>();
        
        int maxScore = players.Max(p => p.Score.Value);
        return players.Where(p => p.Score.Value == maxScore).ToList();
    }
    
    /// <summary>
    /// Check if a player's hand is empty
    /// </summary>
    public static bool IsHandEmpty(Player player)
    {
        return player.HandCards.Count == 0;
    }
}