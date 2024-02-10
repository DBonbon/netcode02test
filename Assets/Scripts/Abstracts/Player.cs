using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections; // Required for FixedString32Bytes
using System.Collections.Generic;

public class Player : NetworkBehaviour
{   
    //[SerializeField] private TextMeshProUGUI playerNameText;
    //private float xOffset = 2f;
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> PlayerDbId = new NetworkVariable<int>();
    public NetworkVariable<FixedString128Bytes> PlayerImagePath = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<int> Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> Result = new NetworkVariable<int>(0);
    public NetworkVariable<bool> IsWinner = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> HasTurn = new NetworkVariable<bool>(false);
    public NetworkVariable<int> Numi = new NetworkVariable<int>(3);

    public List<Card> HandCards { get; set; } = new List<Card>();
    public List<Card> CardsPlayerCanAsk { get; private set; }
    public List<Player> PlayerToAsk { get; private set; } = new List<Player>();

    [SerializeField]
    private Transform playerHand;
    public Transform PlayerHand { get { return playerHand; } set { playerHand = value; } }

    [SerializeField]
    private Transform playerQuartets;
    public Transform PlayerQuartets { get { return playerQuartets; } set { playerQuartets = value; } }

    private PlayerUI playerUI;

    public override void OnNetworkSpawn()
    {
        playerUI = GetComponent<PlayerUI>();
        playerHand = transform.Find("PlayerHandTransfrom");
        if (IsServer)
        {
            Score.Value = 0; // Initialize or re-assign to ensure it's set on all clients
        }

        // Subscribe to Numi value changes to update UI accordingly
        Score.OnValueChanged += OnScoreChanged;
        OnScoreChanged(0, Score.Value); // Manually trigger the update to set initial UI state
    }

    private void OnScoreChanged(int oldValue, int newValue)
    {
        // This method is called whenever Numi changes
        UpdateScoreUI(newValue);
    }

    private void UpdateScoreUI(int score)
    {
        if (playerUI != null)
        {
            playerUI.UpdateScoreUI(score);
        }
    }

    public void InitializePlayer(string name, int dbId, string imagePath)
    {
        PlayerName.Value = name;
        PlayerDbId.Value = dbId;
        PlayerImagePath.Value = imagePath;
      
        if (playerUI != null)
        {
            playerUI.InitializePlayerUI(PlayerName.Value.ToString(), PlayerImagePath.Value.ToString());
            // Initialize UI for Score and HasTurn
            playerUI.UpdateScoreUI(Score.Value);
            playerUI.UpdateHasTurnUI(HasTurn.Value);
        }
    }
    
    public void AddCardToHand(GameObject cardGameObject)
    {
        if (cardGameObject != null)
        {
            // Set the parent to this GameObject's transform (keeping it under the Player GameObject)
            cardGameObject.transform.SetParent(transform, false);
            cardGameObject.transform.localPosition = Vector3.zero; // Optionally reset local position
            Debug.Log($"Card {cardGameObject.name} parented to player {PlayerName.Value}.");

            // Attempt to retrieve the Card component from the card GameObject
            var cardComponent = cardGameObject.GetComponent<Card>();
            if (cardComponent != null)
            {
                // Add the Card component to the HandCards list
                HandCards.Add(cardComponent);
                Debug.Log($"Card {cardComponent.name} added to player {PlayerName.Value}'s HandCards list.");
            }
            else
            {
                Debug.LogError($"The GameObject {cardGameObject.name} does not have a Card component.");
            }
        }
        else
        {
            Debug.LogError("Attempted to add a null card to the hand.");
        }
    }


    public void UpdateTurnStatus(bool hasTurn)
    {
        HasTurn.Value = hasTurn;
        if (playerUI != null)
        {
            playerUI.UpdateHasTurnUI(hasTurn);
        }
    }

    public void UpdatePlayerToAskList(List<Player> allPlayers)
    {
        PlayerToAsk.Clear(); // Clear the list to ensure it's up-to-date

        foreach (var potentialPlayer in allPlayers)
        {
            if (potentialPlayer != this) // Exclude the current player
            {
                PlayerToAsk.Add(potentialPlayer);
            }
        }

        // Optional: Log the count for verification
        Debug.Log($"Player {PlayerName.Value} has {PlayerToAsk.Count} players to ask.");
    }

    // Test method to increment Numi
    void Update()
    {
        // Simple test: Increment Numi on mouse click
        if (IsServer && Input.GetMouseButtonDown(0)) // Check for left mouse click
        {
            IncrementScoreTest();
        }
    }

    public void IncrementScoreTest()
    {
        Score.Value += 1; // Increment Numi by 1 for test
        Debug.Log($"Test: Incremented Score to {Score.Value}");
    }


}