using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> playerTest = new NetworkVariable<int>();
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("DefaultName");
    public NetworkVariable<int> playerScore = new NetworkVariable<int>();
    public NetworkVariable<int> Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> Result = new NetworkVariable<int>(0);
    public NetworkVariable<bool> IsWinner = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> HasTurn = new NetworkVariable<bool>(false);

    public GameObject playerUIPrefab;
    private PlayerUI playerUIInstance;

    private List<Card> hand = new List<Card>();

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

    public void ReceiveCard(Card card)
    {
        hand.Add(card);
        var cardUI = card.GetComponent<CardUI>();
        Debug.Log($"Player {playerName.Value} received card: {card.CardName.Value}, CardUI present: {cardUI != null}");
        UpdatePlayerUIHand();
    }

    public List<ulong> GetHandData()
    {
        List<ulong> handData = new List<ulong>();
        foreach (var card in hand)
        {
            handData.Add(card.GetComponent<NetworkObject>().NetworkObjectId);
        }
        return handData;
    }

    private void UpdatePlayerUIHand()
    {
        Debug.Log($"RecievCard called updateplayeruihand.");
        if (playerUIInstance != null)
        {
            var handCardUIs = new List<CardUI>();
            foreach (var card in hand)
            {
                var cardUI = card.GetComponent<CardUI>();
                if (cardUI != null)
                {
                    handCardUIs.Add(cardUI);
                }
            }
            Debug.Log($"Updating player UI hand with {handCardUIs.Count} cards.");
            playerUIInstance.UpdateHandUI(handCardUIs);
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

    private void OnPlayerTestValueChanged(int previousValue, int newValue)
    {
        if (playerUIInstance != null)
        {
                playerUIInstance.UpdatePlayerTest(newValue);
        }
    }
}