using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager1 : NetworkBehaviour
{
    public static TurnManager1 Instance;
    
    public delegate void EnableUIEvent(bool enableUI);
    public static event EnableUIEvent OnEnableUI;
    // Start is called before the first frame update
    private Card selectedCard;
    private Player selectedPlayer;
    private Player currentPlayer;
    private bool isPlayerUIEnabled = true;
    private bool isDrawingCard = false;
    private bool hasHandledCurrentPlayer = false;
    private bool isInitialized = false;
    private List<Player> players;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            players = PlayerManager.Instance.players;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartTurnManager()
    {
        Debug.Log("turnmanmager started");
        AssignTurnToPlayer();
        StartTurnLoop();
    }

    private void AssignTurnToPlayer()
    {
        bool foundPlayerWithTurn = false;
        foreach (Player player in players)
        {
            if (player.HasTurn.Value) // Correctly use the equality operator
            {
                foundPlayerWithTurn = true;
                break; // Exit the loop early as we've found a player with HasTurn = true
            }
        }

        if (!foundPlayerWithTurn) // If no player with HasTurn = true was found
        {
            AssignInitialTurn();
        }
        else
        {
            AssignNextTurn();
        }
    }

    private void AssignInitialTurn()
    {
        int randomIndex = Random.Range(0, players.Count);
        players[randomIndex].HasTurn.Value = true;
        // Let UpdatePlayerToAskList handle the RPC call with the correct data
        players[randomIndex].UpdatePlayerToAskList(players);
        players[randomIndex].UpdateCardsPlayerCanAsk();
        StartTurnLoop();

        Debug.Log($"Turn assigned to player: {players[randomIndex].playerName.Value}");
    }

    private void AssignNextTurn()
    {
        
        Debug.Log("next current player is called");
        if (players.Count == 0) return;

        
        int currentIndex = players.IndexOf(currentPlayer); // Find the index of the current player who has the turn.
        
        if (currentIndex == -1) return; // Ensure currentIndex is valid.

        currentPlayer.HasTurn.Value = false;// Turn off the current player's turn.

        int nextIndex = (currentIndex + 1) % players.Count;// Calculate the index of the next player. Wrap around if necessary.

        if (nextIndex < players.Count && nextIndex >= 0) // Ensure the next player exists.
        {
            // Set the next player as the current player and turn their turn on.
            currentPlayer = players[nextIndex];
            currentPlayer.HasTurn.Value = true;
            currentPlayer.UpdatePlayerToAskList(players);
            currentPlayer.UpdateCardsPlayerCanAsk();
            
            // Log or perform additional actions as necessary.
            Debug.Log($"Turn assigned to player: {currentPlayer.playerName.Value}");
            StartTurnLoop();
        }
        else
        {
            // Handle the unexpected case where nextIndex is out of bounds.
            Debug.LogError("Next player index is out of valid range.");
        }
    }

    private void StartTurnLoop() 
    {
        if (!isInitialized)
        {
            Debug.Log("Turn Manager Started");
            //AssignTurnToPlayer();
            isInitialized = true;
            currentPlayer = players.Find(player => player.HasTurn.Value);
            Debug.Log($"call TurnLoop: {currentPlayer.playerName.Value}");

            if (currentPlayer != null)
            {
                ActivateTurnUI(currentPlayer); // vv - a circle shape that shrink to display the turn and zoom out to display the turnui
                //and zoom out when the turn is checked. i.e. during askforcard
            }
            else
            {
                Debug.LogError("No initial player with hasTurn == true found.");
            }
        }
    }

    private void ActivateTurnUI(Player player)
    {
        DisplayMessage("Activate TurnUi");
    }

    private void DeactivateTurnUI()
    {
        DisplayMessage("Dectivated TurnUi");
    }
    
    public void OnEventGuessClick(ulong playerId, NetworkVariable<int> cardId)
    {
        //NetworkVariable<int> networkCardId =???(cardId); 
        Debug.Log($"The playerid value is: {playerId}, and cardid: {cardId}");
        Card selectedCard = CardManager.Instance.FetchCardById(cardId);
        Debug.Log($"oneventguessclick selected card: {selectedCard.cardName.Value}");
        Player selectedPlayer = PlayerManager.Instance.players.Find(player => player.OwnerClientId == playerId);
        Debug.Log($"oneventguessclick selected player: {selectedPlayer.playerName.Value}");
        this.selectedCard = selectedCard;
        this.selectedPlayer = selectedPlayer;
        if (currentPlayer != null && selectedPlayer != null && !isDrawingCard) //
        {
            GuessCheck(selectedCard, selectedPlayer);
            selectedCard = null;
            selectedPlayer = null;
            Debug.Log($"handle player method waits for selectedCard: {selectedCard} and/or selectedPlayer {selectedPlayer}");
        }
        else
        {
            Debug.LogWarning($"{Time.time}: Invalid player turn.");
        }
    }

    private void GuessCheck(Card selectedCard, Player selectedPlayer)
    {
        Debug.Log("GuessCheck is running");
        DeactivateTurnUI();
        PlayersTalkDelay(selectedPlayer, selectedPlayer);  //this method will inser a time gap for a talk
        TurnCheckHold(); 
        if (selectedPlayer.HandCards.Contains(selectedCard))
        {
            CorrectGuess();
        }
        else
        {
            WrongGuess();
        }
    }

    private void CorrectGuess()
    { 
        TransferCard(selectedCard, currentPlayer);
        CheckForQuartets(); 
        selectedCard = null;
        selectedPlayer = null;
        Debug.Log($"Selected Card: {selectedCard.cardName}");
        //ActivateTurnUI(currentPlayer);

        // If the guess is correct and the player's hand isn't empty, allow another guess.
        if (!IsPlayerHandEmpty(currentPlayer))
        {
            ActivateTurnUI(currentPlayer);
        }
        else if (DeckManager.Instance.CurrentDeck != null && DeckManager.Instance.CurrentDeck.DeckCards.Count > 0)
        {    
            DrawCardFromDeck();  // If the player's hand is empty but the deck isn't, draw a card from the deck.
            if (!IsPlayerHandEmpty(currentPlayer)) // After drawing a card, re-evaluate the hand.
            {
                // Allow the player to make another guess.
                selectedCard = null;
                selectedPlayer = null;
                ActivateTurnUI(currentPlayer);
            }
        }
    }

    private void WrongGuess()
    {
        Debug.Log("ask for card, player doesn't have card");
        DisplayMessage($"{selectedPlayer.playerName} does not have {selectedCard.cardName}.");
        DrawCardFromDeck();
        Debug.Log("ask for card, call end turn");
        EndTurn(); // This is the correct place to call EndTurn for an incorrect guess.
    }

    public void DisableTurnUi()
    {
        DisplayMessage("The turnui are disabled during the guesscheck");
    }
    
    public void PlayersTalkDelay(Player curPlayer, Player selPlayer) 
    {
        return; //players chat if the have a card
    }

    public void TurnCheckHold()
    {
        return;
    }

    private void TransferCard(Card selectedCard, Player curPlayer)
    {
        Debug.Log("TransferCard is correct");    
        selectedPlayer.RemoveCardFromHand(selectedCard);
        currentPlayer.AddCardToHand(selectedCard);
    }

    private bool IsPlayerHandEmpty(Player currentPlayer)
    {
        return currentPlayer.IsHandEmpty();
    }

    private void EndTurn()
    {
        AssignNextTurn();
        Debug.Log("end turn is running");
    }

    private void CheckForQuartets()
    {
        Debug.Log("check for quartets is called");
        currentPlayer.CheckForQuartets(); // Implement your quartet-checking logic here
        // Check if the player's hand is empty after quartets are checked.
        if (IsPlayerHandEmpty(currentPlayer) &&  DeckManager.Instance.CurrentDeck.DeckCards.Count == 0)
        {
            CheckGameEnd();
            EndTurn();
        }
    }

    private void DisplayMessage(string message)
    {
        // Your DisplayMessage logic here
        // ...
        Debug.Log("Display Message: " + message);
    }

    public void DrawCardFromDeck()
    {
        Debug.Log("draw card from deck is called");
        if (CardManager.Instance != null && currentPlayer != null)
        {
            CardManager.Instance.DrawCardFromDeck(currentPlayer);
        }
        else
        {
            Debug.LogError($"{Time.time}: CardManager reference or current player or selected card is not assigned.");
        }
    }

    private void CheckGameEnd()
    {
        bool allHandsEmpty = true;
        foreach (Player player in PlayerManager.Instance.players)
        {
            if (!player.IsHandEmpty())
            {
                allHandsEmpty = false;
                break;
            }
        }

        if (allHandsEmpty)
        {
            GameEnd();
        }
    }

    private void GameEnd()
    {
        Debug.Log($"{Time.time}: Game Ended");
        // Call the method to display end game results
        //gameFlowManager.DisplayEndGameResults();
    }
}
