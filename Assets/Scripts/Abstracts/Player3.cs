using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections; // Required for FixedString32Bytes

public class Player3 : NetworkBehaviour
{
    
    [SerializeField] private TextMeshProUGUI playerNameText;
    private float xOffset = 2f;
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ulong clientId = GetComponent<NetworkObject>().NetworkObjectId;
            float xPosition = xOffset * (clientId - 1);
            transform.position = new Vector3(xPosition, 0, 0);
            Debug.Log("Player is Spawned");

            PlayerName.Value = new FixedString32Bytes("Player " + clientId.ToString()); // Assign the player name
        }

        // Subscribe to the PlayerName variable change event for all instances (server and client)
        PlayerName.OnValueChanged += OnPlayerNameChanged;
        OnPlayerNameChanged(default, PlayerName.Value); // Initial update for clients
    }

    private void OnDestroy()
    {
        if (PlayerName.OnValueChanged != null)
        {
            // Unsubscribe to avoid memory leaks
            PlayerName.OnValueChanged -= OnPlayerNameChanged;
        }
    }

    private void OnPlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        UpdatePlayerNameUI();
    }

    public void UpdatePlayerNameUI()
    {
        if (playerNameText != null)
        {
            playerNameText.text = PlayerName.Value.ToString(); // Convert FixedString32Bytes to string
        }
        else
        {
            Debug.LogError("TextMeshPro component not set on PlayerPositioner.");
        }
    }

    public void ParentCard(GameObject card)
    {
        if (card != null)
        {
            card.transform.SetParent(transform, false);
            card.transform.localPosition = Vector3.zero; // Reset local position
            Debug.Log($"Card {card.name} parented to player {PlayerName.Value}.");
        }
    }

}
