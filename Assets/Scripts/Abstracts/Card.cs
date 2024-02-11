using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class Card : NetworkBehaviour
{
    // NetworkVariable to hold the card's name
    public NetworkVariable<FixedString32Bytes> CardName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Suit = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> Hint = new NetworkVariable<FixedString32Bytes>();
    //public NetworkVariable<FixedString64Bytes> Level = new NetworkVariable<FixedString64Bytes>();
    //public NetworkVariable<FixedString128Bytes> CardImagePath = new NetworkVariable<FixedString128Bytes>();
    //public Sprite illustration;
    [SerializeField]
    private Transform gameObject;
    public Transform GameObject { get { return GameObject; } set { gameObject = value; } }
    // Add the siblingNames field
    public List<string> siblingNames;
    public List<Card> HandCards { get; set; } = new List<Card>();

    // Additional attributes like suit can be added similarly
    /*
    / Add the siblingNames field
    public List<string> siblingNames;
    // Whether this card is the main card (colored differently)
    public bool isMainCard;
    [SerializeField] // Add this attribute
    public Color cardNameColor = Color.red; // Default color
    // Property to get/set the cardNameColor
    private Color CardNameColor
    {
        get { return cardNameColor; }
        set { cardNameColor = value; }
    }

    public int CompareTo(Card other)
    {
        // Sort by suit alphabetically
        return String.Compare(suit, other.suit, StringComparison.Ordinal);
    }
    */
    // Method to initialize card properties
    public void InitializeCard(string name, string suit, string hint)
    {
        if (IsServer)
        {
            // Ensure name and suit are not null. Assign default values if they are.
            var safeName = string.IsNullOrEmpty(name) ? "DefaultName" : name;
            var safeSuit = string.IsNullOrEmpty(suit) ? "DefaultSuit" : suit;
            var safeHint = string.IsNullOrEmpty(hint) ? "DefaultHint" : hint; // Hint gets a default value if null or empty

            CardName.Value = new FixedString32Bytes(safeName);
            Suit.Value = new FixedString32Bytes(safeSuit);
            Hint.Value = new FixedString32Bytes(safeHint);
        }
    }


    // Method to handle changes in the card's attributes, if necessary
    private void OnCardNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        // Handle any updates required when the card's name changes
    }

    private void Start()
    {
        if (IsServer)
        {
            // Initialize card properties
        }

        // Subscribe to the network variable's change event
        CardName.OnValueChanged += OnCardNameChanged;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (CardName.OnValueChanged != null)
        {
            CardName.OnValueChanged -= OnCardNameChanged;
        }
    }


}
