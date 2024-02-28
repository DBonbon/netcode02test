using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections; // Required for FixedString32Bytes
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

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

    public List<Card> HandCards { get; set; } = new List<Card>();
    public List<Player> PlayerToAsk { get; private set; } = new List<Player>();
    public List<Card> CardsPlayerCanAsk { get; private set; } = new List<Card>();
    public List<Card> Quartets { get; private set; } = new List<Card>();

    private PlayerUI playerUI;

    public override void OnNetworkSpawn()
    {
        playerUI = GetComponent<PlayerUI>();

        if (IsServer)
        {
            Score.Value = 0; // Initialize or re-assign to ensure it's set on all clients.
        }

        // Subscribe to Score value changes to update UI accordingly.
        Score.OnValueChanged += OnScoreChanged;
        OnScoreChanged(0, Score.Value); // Manually trigger the update to set initial UI state.
    }

    private void OnScoreChanged(int oldValue, int newValue)
    {
        // This method is called whenever Score changes
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
        if (IsServer)
        {
            PlayerName.Value = name;
            PlayerDbId.Value = dbId;
            PlayerImagePath.Value = imagePath;

            // Update server UI directly here
            UpdateServerUI(name, imagePath);
            // setup and we want to immediately propagate these values to all clients.
            BroadcastPlayerDbAttributes();
            // Spawn and parent the player hand
            SpawnAndParentPlayerHand();
        }
        
    }

    private void UpdateServerUI(string playerName, string playerImagePath)
    {
        if (playerUI != null)
        {
            playerUI.InitializePlayerUI(playerName, playerImagePath);
            playerUI.UpdateHasTurnUI(HasTurn.Value);
        }
    }

    public void BroadcastPlayerDbAttributes()
    {
        if (IsServer)
        {
            UpdatePlayerDbAttributes_ClientRpc(PlayerName.Value.ToString(), PlayerImagePath.Value.ToString());
        }
    }

    [ClientRpc]
    private void UpdatePlayerDbAttributes_ClientRpc(string playerName, string playerImagePath)
    {
        if (playerUI != null)
        {
            playerUI.InitializePlayerUI(playerName, playerImagePath);
        }
    }

    public void AddCardToHand(GameObject cardGameObject)
    {
        if (cardGameObject != null)
        {
            var cardComponent = cardGameObject.GetComponent<Card>();
            if (cardComponent != null)
            {
                HandCards.Add(cardComponent);
                Debug.Log($"Card {cardComponent.name} added to player {PlayerName.Value}'s HandCards list.");

                // Prepare a list of card IDs to send to the UI
                List<int> cardIDs = new List<int>();
                foreach (var card in HandCards)
                {
                    cardIDs.Add(card.cardId.Value);
                }

                
                // Update the UI with the list of card IDs
                if (playerUI != null)
                {
                    //SendCardIDsToClient();
                    playerUI.UpdatePlayerHandUIWithIDs(cardIDs);
                }
            }
            else
            {
                Debug.LogError($"The GameObject {cardGameObject.name} does not have a Card component.");
            }

            UpdateCardsPlayerCanAsk();
            CheckForQuartets();
        }
        else
        {
            Debug.LogError("cardGameObject is null.");
        }
    }


    // This method is called on the server to send the card IDs to the client
    public void SendCardIDsToClient()
    {
        if (IsServer)
        {
            int[] cardIDs = HandCards.Select(card => card.cardId.Value).ToArray();
            UpdatePlayerHandUI_ClientRpc(cardIDs, OwnerClientId);
        }
    }

    // ClientRpc to update the player's hand UI with the given card IDs
    [ClientRpc]
    private void UpdatePlayerHandUI_ClientRpc(int[] cardIDs, ulong targetClient)
    {
        // Ensure that this RPC is executed only by the target client
        if (IsOwner)
        {
            playerUI?.UpdatePlayerHandUIWithIDs(cardIDs.ToList());
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

    public void UpdateCardsPlayerCanAsk()
    {
        // Ensure CardsPlayerCanAsk is initialized
        if (CardsPlayerCanAsk == null)
        {
            CardsPlayerCanAsk = new List<Card>();
        }
        else
        {
            CardsPlayerCanAsk.Clear();
        }

        // You should ensure CardManager.Instance provides access to all cards in the game correctly
        // Assuming CardManager.Instance.allSpawnedCards is a List<Card> and not List<GameObject>
        //var allCards = CardManager.Instance.allSpawnedCards; // Make sure this is a List<Card>
        var allCardComponents = CardManager.Instance.allSpawnedCards.Select(go => go.GetComponent<Card>()).Where(c => c != null);

        // Filter out cards that have the same suit as at least one card in HandCards
        // and are not already in HandCards
        foreach (var card in allCardComponents)
        {
            if (HandCards.Any(handCard => handCard.Suit.Value == card.Suit.Value) && !HandCards.Contains(card))
            {
                CardsPlayerCanAsk.Add(card);
            }
        }
        Debug.Log($"Player {PlayerName.Value} can ask for {CardsPlayerCanAsk.Count} cards based on suits.");
    }

    public void CheckForQuartets()
    {
        // Group cards by their Suit value
        var groupedBySuit = HandCards.GroupBy(card => card.Suit.Value.ToString());

        foreach (var suitGroup in groupedBySuit)
        {
            if (suitGroup.Count() == 4) // Exactly 4 cards of the same suit
            {
                MoveCardsToQuartetsArea(suitGroup.ToList());
            }
        }
    }

    private void MoveCardsToQuartetsArea(List<Card> quartet)
    {
        foreach (var card in quartet)
        {
            HandCards.Remove(card);
            Quartets.Add(card);
        }

        // Optionally, trigger some UI update or gameplay effect here
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

    // Test method to increment score
    void Update()
    {
        // Simple test: Increment score on mouse click
        if (IsServer && Input.GetMouseButtonDown(0)) // Check for left mouse click
        {
            IncrementScoreTest();
        }
    }

    //to remove when isn't needed anymore:
    public void IncrementScoreTest()
    {
        Score.Value += 1; // Increment score by 1 for test
        Debug.Log($"Test: Incremented Score to {Score.Value}");
    }

    private void SpawnAndParentPlayerHand()
    {/*
        if (IsServer)
        {
            GameObject handObject = Instantiate(playerHandPrefab);
            NetworkObject handNetworkObject = handObject.GetComponent<NetworkObject>();
            if (handNetworkObject != null)
            {
                //handNetworkObject.Spawn();
                //playerHandTransform = handObject.transform;
                //playerHandTransform.SetParent(this.transform, false);
                //playerHandTransform.localPosition = Vector3.zero;

                // Check if the playerHandPrefab has a Canvas, and if so, add the Horizontal Layout Group to it.
                var canvas = handObject.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    var layoutGroup = canvas.gameObject.AddComponent<HorizontalLayoutGroup>();
                    layoutGroup.spacing = 10; // Adjust spacing as needed
                    layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    layoutGroup.childForceExpandWidth = false;
                    layoutGroup.childForceExpandHeight = false;
                }
                else
                {
                    Debug.LogWarning("Player hand prefab does not contain a Canvas component for the layout group.");
                }
            }
            else
            {
                Debug.LogError("Spawned PlayerHand does not have a NetworkObject component.");
            }
        }*/
    }

    // Ensure OnDestroy is correctly implemented to handle any cleanup
    public override void OnDestroy()
    {
        base.OnDestroy();
        // Your cleanup logic here
    }

}