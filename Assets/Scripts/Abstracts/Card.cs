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

    // Implement the Equals method for IEquatable<SiblingName>
    public bool Equals(SiblingName other)
    {
        return name.Equals(other.name);
    }

    // It's also a good practice to override Equals(object obj) and GetHashCode()
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
    public NetworkVariable<FixedString32Bytes> CardName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Suit = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Hint = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString64Bytes> Level = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString128Bytes> CardImagePath = new NetworkVariable<FixedString128Bytes>();

    public NetworkList<SiblingName> SiblingNames = new NetworkList<SiblingName>();

    public void InitializeCard(string name, string suit, string hint, List<string> siblings)
    {
        if (IsServer)
        {
            CardName.Value = name;
            Suit.Value = suit;
            Hint.Value = hint;
            SiblingNames.Clear();
            foreach (var sibling in siblings)
            {
                SiblingNames.Add(new SiblingName(sibling));
            }
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
