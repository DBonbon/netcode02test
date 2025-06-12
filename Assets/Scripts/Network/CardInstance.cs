using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Pure network representation of a card instance in the game
/// This is the actual card object that moves between players
/// No UI logic - only network state and data
/// </summary>
public class CardInstance : NetworkBehaviour, IComparable<CardInstance>
{
    [Header("Network Card Data")]
    public NetworkVariable<int> cardId = new NetworkVariable<int>();
    public NetworkVariable<FixedString128Bytes> cardName = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> suit = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> hint = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> level = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> cardImagePath = new NetworkVariable<FixedString128Bytes>();
    
    public NetworkList<SiblingName> siblingNames = new NetworkList<SiblingName>();

    /// <summary>
    /// Initialize this card instance with data from CardData
    /// Should only be called on server
    /// </summary>
    public void InitializeCard(int id, string name, string cardSuit, string cardHint, List<string> siblings)
    {
        if (!IsServer)
        {
            Debug.LogWarning("InitializeCard should only be called on server");
            return;
        }

        Debug.Log($"Initializing CardInstance: {name}");
        
        cardId.Value = id;
        cardName.Value = name;
        suit.Value = cardSuit;
        hint.Value = cardHint;
        
        siblingNames.Clear();
        foreach (var sibling in siblings)
        {
            siblingNames.Add(new SiblingName(sibling));
        }
        
        Debug.Log($"CardInstance initialized - ID: {cardId.Value}, Name: {cardName.Value}");
    }

    /// <summary>
    /// Get the static card data this instance represents
    /// </summary>
    public CardData GetCardData()
    {
        // Find the matching CardData from DataManager
        var allCardData = DataManager.LoadedCardData;
        if (allCardData != null)
        {
            return allCardData.Find(data => data.cardId == cardId.Value);
        }
        
        Debug.LogWarning($"Could not find CardData for cardId: {cardId.Value}");
        return null;
    }

    /// <summary>
    /// Compare cards by suit for sorting
    /// </summary>
    public int CompareTo(CardInstance other)
    {
        if (other == null) return 1;
        return String.Compare(suit.Value.ToString(), other.suit.Value.ToString(), StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return $"CardInstance({cardId.Value}: {cardName.Value})";
    }
}