using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections; // Required for FixedString32Bytes

public class Player3 : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    private float yOffset = 2f;
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ulong clientId = GetComponent<NetworkObject>().NetworkObjectId;
            float yPosition = yOffset * (clientId - 1);
            transform.position = new Vector3(0, yPosition, 0);
            Debug.Log("Player is Spawned");

            PlayerName.Value = new FixedString32Bytes("Player " + clientId.ToString()); // Assign the player name
        }

        // Subscribe to the PlayerName variable change event for all instances (server and client)
        
    }

}
