using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Card3 : NetworkBehaviour
{
    // NetworkVariable to hold the card's name
    public NetworkVariable<FixedString32Bytes> CardName = new NetworkVariable<FixedString32Bytes>();

    // Additional attributes like suit can be added similarly

    // Method to initialize card properties
    public void InitializeCard(string name)
    {
        if (IsServer)
        {
            CardName.Value = new FixedString32Bytes(name);
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
