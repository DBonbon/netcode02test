using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> playerTest = new NetworkVariable<int>();
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("DefaultName");
    public NetworkVariable<int> playerScore = new NetworkVariable<int>();
    public GameObject playerUIPrefab;
    private PlayerUI playerUIInstance;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerManager.Instance.AddPlayer(this);
        }

        // This will now execute for both the server and all clients
        SetupPlayerUI();

        if (IsLocalPlayer)
        {
            playerTest.OnValueChanged += OnPlayerTestValueChanged;
        }
    }

    private void Update()
    {
        if (IsLocalPlayer && Input.GetKeyDown(KeyCode.T))
        {
            SubmitPlayerTestServerRpc(playerTest.Value + 1);
        }

        // Update UI with the latest value for all instances
        if (playerUIInstance != null)
        {
            playerUIInstance.UpdatePlayerTest(playerTest.Value);
        }
    }

    [ServerRpc]
    void SubmitPlayerTestServerRpc(int newValue)
    {
        playerTest.Value = newValue;
    }

    private void SetupPlayerUI()
    {
        Canvas playerUICanvas = FindObjectOfType<Canvas>(); // Find the Canvas in the scene
        if (playerUIPrefab != null && playerUICanvas != null)
        {
            GameObject uiObject = Instantiate(playerUIPrefab, playerUICanvas.transform);
            playerUIInstance = uiObject.GetComponent<PlayerUI>();

            // Correct call to SetupPlayer with all required parameters
            playerUIInstance.SetupPlayer(GetComponent<NetworkObject>().NetworkObjectId, playerName.Value.ToString(), playerScore.Value, IsLocalPlayer, PlayerManager.Instance.TotalPlayerCount(), IsServer);
        }
    }


    private void OnPlayerTestValueChanged(int previousValue, int newValue)
    {
        if (playerUIInstance != null)
        {
                playerUIInstance.UpdatePlayerTest(newValue);
        }
    }
}
