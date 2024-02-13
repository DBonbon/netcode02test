using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Bard : NetworkBehaviour
{
/*
public struct BiblingName : INetworkSerializable, IEquatable<BiblingName>
{
    private FixedString32Bytes name;

    public string Name => name.ToString();

    public BiblingName(string name)
    {
        this.name = name;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
    }

    // Implement the Equals method for IEquatable<BiblingName>
    public bool Equals(BiblingName other)
    {
        return name.Equals(other.name);
    }

    // It's also a good practice to override Equals(object obj) and GetHashCode()
    public override bool Equals(object obj)
    {
        return obj is BiblingName other && Equals(other);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }
}


public class Bard : NetworkBehaviour, IComparable<Bard>
{
    // NetworkVariable to hold the bard's name
    public NetworkVariable<FixedString32Bytes> BardName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Suit = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Hint = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString64Bytes> Level = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString128Bytes> BardImagePath = new NetworkVariable<FixedString128Bytes>();
    //public Sprite illustration;
    [SerializeField]
    private Transform gameObject;
    public Transform GameObject { get { return GameObject; } set { gameObject = value; } }
    // Add the biblingNames field
    public List<string> biblingNames;
    public NetworkList<BiblingName> BiblingNames = new NetworkList<BiblingName>();

    public List<Bard> HandBards { get; set; } = new List<Bard>();
    
    public void InitializeBard(string name, string suit, string hint, List<string> biblings)
    {
        if (IsServer)
        {
            BardName.Value = new FixedString32Bytes(name);
            Suit.Value = new FixedString32Bytes(suit);
            Hint.Value = new FixedString32Bytes(hint);

            // Populate the NetworkList with biblings
            BiblingNames.Clear();
            foreach (var bibling in biblings)
            {
                BiblingNames.Add(new BiblingName(bibling));
            }
        }
    }

    public int CompareTo(Bard other)
    {
        // Ensure you compare the FixedString32Bytes values correctly
        if (other == null) return 1; // Consider the current instance as greater if 'other' is null

        // Use the Value property of NetworkVariable to access the FixedString32Bytes and convert them to string for comparison
        var currentSuit = this.Suit.Value.ToString();
        var otherSuit = other.Suit.Value.ToString();

        return String.Compare(currentSuit, otherSuit, StringComparison.Ordinal);
    }*/

}