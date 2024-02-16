using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;

public struct SiblingName : INetworkSerializable, IEquatable<SiblingName>
{
    private FixedString32Bytes name;
    public string Name => name.ToString();

    public SiblingName(string name)
    {
        this.name = name;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
    }

    public bool Equals(SiblingName other)
    {
        return name.Equals(other.name);
    }

    public override bool Equals(object obj)
    {
        return obj is SiblingName other && Equals(other);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }
}

public class Card : NetworkBehaviour, IComparable<Card>
{
    public NetworkVariable<int> cardId = new NetworkVariable<int>();
    public NetworkVariable<FixedString32Bytes> CardName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Suit = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Hint = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString64Bytes> Level = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString128Bytes> CardImagePath = new NetworkVariable<FixedString128Bytes>();

    public NetworkList<SiblingName> SiblingNames = new NetworkList<SiblingName>();

    public void InitializeCard(int id, string name, string suit, string hint, List<string> siblings)
    {
        Debug.Log("the intialized card was called");
        Debug.Log($"checking is server on: {IsServer}");
        if (IsServer)
        {
            Debug.Log($"checking is server on: {IsServer}");
            cardId.Value = id;
            CardName.Value = name;
            Suit.Value = suit;
            Hint.Value = hint;
            SiblingNames.Clear();
            foreach (var sibling in siblings)
            {
                SiblingNames.Add(new SiblingName(sibling));
            }
            Debug.Log($"the cardId is: {cardId.Value}");
            Debug.Log($"the cardname is: {CardName.Value}");
        }
    }

    public int CompareTo(Card other)
    {
        if (other == null) return 1;
        var currentSuit = Suit.Value.ToString();
        var otherSuit = other.Suit.Value.ToString();
        return String.Compare(currentSuit, otherSuit, StringComparison.Ordinal);
    }
}
