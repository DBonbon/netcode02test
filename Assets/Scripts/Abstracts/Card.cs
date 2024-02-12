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
    // NetworkVariable to hold the card's name
    public NetworkVariable<FixedString32Bytes> CardName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Suit = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Hint = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString64Bytes> Level = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString128Bytes> CardImagePath = new NetworkVariable<FixedString128Bytes>();
    //public Sprite illustration;
    [SerializeField]
    private Transform gameObject;
    public Transform GameObject { get { return GameObject; } set { gameObject = value; } }
    // Add the siblingNames field
    public List<string> siblingNames;
    public NetworkList<SiblingName> SiblingNames = new NetworkList<SiblingName>();

    public List<Card> HandCards { get; set; } = new List<Card>();
    
    public void InitializeCard(string name, string suit, string hint, List<string> siblings)
    {
        if (IsServer)
        {
            CardName.Value = new FixedString32Bytes(name);
            Suit.Value = new FixedString32Bytes(suit);
            Hint.Value = new FixedString32Bytes(hint);

            // Populate the NetworkList with siblings
            SiblingNames.Clear();
            foreach (var sibling in siblings)
            {
                SiblingNames.Add(new SiblingName(sibling));
            }
        }
    }

    public int CompareTo(Card other)
    {
        // Ensure you compare the FixedString32Bytes values correctly
        if (other == null) return 1; // Consider the current instance as greater if 'other' is null

        // Use the Value property of NetworkVariable to access the FixedString32Bytes and convert them to string for comparison
        var currentSuit = this.Suit.Value.ToString();
        var otherSuit = other.Suit.Value.ToString();

        return String.Compare(currentSuit, otherSuit, StringComparison.Ordinal);
    }

}
