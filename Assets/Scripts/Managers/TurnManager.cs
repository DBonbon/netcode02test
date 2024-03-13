using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;
    
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
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        Debug.Log("AssignTurnToPlayer is called");
        var players = PlayerManager.Instance.players;
        if (players.Count == 0) return;

        foreach (var player in players)
        {
            player.HasTurn.Value = false;
        }

        int randomIndex = Random.Range(0, players.Count);
        players[randomIndex].HasTurn.Value = true;
        // Let UpdatePlayerToAskList handle the RPC call with the correct data
        players[randomIndex].UpdatePlayerToAskList(players);
        players[randomIndex].UpdateCardsPlayerCanAsk();

        Debug.Log($"Turn assigned to player: {players[randomIndex].playerName.Value}");
    }

    private void StartTurnLoop()
    {
        if (!isInitialized)
        {
            Debug.Log("Turn Manager Started");
            //AssignTurnToPlayer();
            isInitialized = true;
            currentPlayer = PlayerManager.Instance.players.Find(player => player.HasTurn.Value);
            Debug.Log($"call TurnLoop: {currentPlayer.playerName.Value}");

            if (currentPlayer != null)
            {
                StartCoroutine(TurnLoop());
                Debug.Log($"call coroutine TurnLoop: {currentPlayer.playerName.Value}");
            }
            else
            {
                Debug.LogError("No initial player with hasTurn == true found.");
            }
        }
    }

    private System.Collections.IEnumerator TurnLoop()
    {
        Debug.Log("Turn loop is rusnning");
        while (true)
        {
            Debug.Log($"Turn loop currewnt player: {currentPlayer.playerName.Value} with turn status: {currentPlayer.HasTurn.Value}");
            if (currentPlayer.HasTurn.Value)
            {
                if (!hasHandledCurrentPlayer)
                {
                    Debug.Log($"Turn loop hasHandledCurrentPlayer current player: {currentPlayer.playerName.Value} whith turn status: {currentPlayer.HasTurn.Value} and hashandlecurretplay flag is: {hasHandledCurrentPlayer}");
                    HandlePlayerTurn(currentPlayer);
                    Debug.Log($"Turn loop hasHandledCurrentPlayer1 current player: {currentPlayer.playerName.Value} whith turn status: {currentPlayer.HasTurn.Value}");
                    Debug.Log($" hashandlecurretplay flag is: {hasHandledCurrentPlayer}");
                    hasHandledCurrentPlayer = true;
                    Debug.Log($" hashandlecurretplay1 flag is: {hasHandledCurrentPlayer}");
                    Debug.Log($"Turn loop hasHandledCurrentPlayer1 current player: {currentPlayer.playerName.Value} ad hashandlecurretplay flag is: {hasHandledCurrentPlayer}");
                }
            }

            if (!currentPlayer.HasTurn.Value)
            {
                Debug.Log($"Turn loop hasHandledCurrentPlayer current player: {currentPlayer.playerName.Value} has Turn: {currentPlayer.HasTurn.Value}");
                hasHandledCurrentPlayer = false;
                NextCurrentPlayer();
            }
            // Debug log to check if the loop is still running
            Debug.Log("Turn loop is still running");
            yield return null;
        }
        // Debug log to check if the loop terminates
        Debug.Log("Turn loop terminated");
    }

    public void ServerRpcTest(int id)
    {
        Debug.Log($"The id value is: {id}");
    }

    public void OnEventGuessClick(ulong playerId, NetworkVariable<int> cardId)
    {
        //NetworkVariable<int> networkCardId =???(cardId); 
        Debug.Log($"The playerid value is: {playerId}, and cardid: {cardId}");
        Card selectedCard = CardManager.Instance.FetchCardById(cardId);
        Debug.Log($"oneventguessclick selected card: {selectedCard.cardName.Value}");
        Player selectedPlayer = PlayerManager.Instance.players.Find(player => player.OwnerClientId == playerId);
        Debug.Log($"oneventguessclick selected player: {selectedPlayer.playerName.Value}");
        if (currentPlayer != null && !isDrawingCard) //
        {
            HandlePlayerTurn(currentPlayer);
            Debug.Log($"HandlePlayerTurn currentPlayer is: {currentPlayer}");
        }
        else
        {
            Debug.LogWarning($"{Time.time}: Invalid player turn.");
        }
    }

    public void OnEventGuessClick1(ulong playerId, NetworkVariable<int> cardId)
    {
        Debug.Log($"oneventguessclick turnmanager1: is running with playerid: {playerId} and cardid: {cardId}");
        // Fetch the selected card by its ID
        selectedCard = CardManager.Instance.FetchCardById(cardId);
        Debug.Log($"oneventguessclick selected card: {selectedCard.cardName.Value}");
        // Fetch the selected player from PlayerManager
        selectedPlayer = PlayerManager.Instance.players.Find(player => player.OwnerClientId == playerId);
        Debug.Log($"oneventguessclick selected player: {selectedPlayer.playerName.Value}");
        // Assign the selected card and player to the TurnManager's fields
        this.selectedCard = selectedCard;
        this.selectedPlayer = selectedPlayer;
        Debug.Log($"oneventguessclick turnmanager: {selectedCard.cardName.Value} and {selectedPlayer.playerName.Value}");
        // Check if it's a valid player turn and handle the player's turn
        if (currentPlayer != null && !isDrawingCard) //
        {
            HandlePlayerTurn(currentPlayer);
            Debug.Log($"HandlePlayerTurn currentPlayer is: {currentPlayer}");
        }
        else
        {
            Debug.LogWarning($"{Time.time}: Invalid player turn.");
        }
    }

    public void OnEventGuessClick2(ulong playerId, int cardId)
    {
        Debug.Log("oneventguessclick turnmanager: is running");
        if (selectedCard != null)
        {
            Debug.LogWarning("Method already called with int card ID. Skipping recursion.");
            return;
        }
        NetworkVariable<int> networkCardId = new NetworkVariable<int>(cardId);
        Debug.Log($"oneventguessclicko card and player are {networkCardId}, {playerId} ");
        TurnManager.Instance.OnEventGuessClick(playerId, networkCardId);
    }
    

    public void OnEventGuessClick3(ulong playerId, NetworkVariable<int> cardId)
    {
        Debug.Log($"oneventguessclick turnmanager1: is running with playerid: {playerId} and cardid: {cardId}");
        // Fetch the selected card by its ID
        Card selectedCard = CardManager.Instance.FetchCardById(cardId);
        Debug.Log($"oneventguessclick selected card: {selectedCard}");
        // Fetch the selected player from PlayerManager
        Player selectedPlayer = PlayerManager.Instance.players.Find(player => player.OwnerClientId == playerId);
        Debug.Log($"oneventguessclick selected player: {selectedPlayer}");
        // Assign the selected card and player to the TurnManager's fields
        this.selectedCard = selectedCard;
        this.selectedPlayer = selectedPlayer;
        Debug.Log($"oneventguessclick turnmanager: {selectedCard} and {selectedPlayer}");
        // Check if it's a valid player turn and handle the player's turn
        if (currentPlayer != null && !isDrawingCard) //
        {
            HandlePlayerTurn(currentPlayer);
            Debug.Log($"HandlePlayerTurn currentPlayer is: {currentPlayer}");
        }
        else
        {
            Debug.LogWarning($"{Time.time}: Invalid player turn.");
        }
    }


    private void HandlePlayerTurn(Player currentPlayer)
    {
        //MakeGuess(currentPlayer);
        Debug.Log("HandlePlayerTurn is called");
        Debug.Log($"currentPlazer is: {currentPlayer}");
        Debug.Log($"Selected Card: {selectedCard.cardName}");

        if (selectedCard != null && selectedPlayer != null)
        {
            AskForCard(selectedCard, selectedPlayer);

            // Reset selectedCard and selectedPlayer to null
            selectedCard = null;
            selectedPlayer = null;
        }
        else
        {
            Debug.Log($"handle player method waits for selectedCard: {selectedCard} and/or selectedPlayer {selectedPlayer}");
        }
    }


    private void AskForCard(Card selectedCard, Player selectedPlayer)
    {
        if (selectedPlayer.HandCards.Contains(selectedCard))
        {
            Debug.Log("AskForCar guess is correct");
            TransferCard(selectedCard, currentPlayer);
            CheckForQuartets(); 
            selectedCard = null;
            selectedPlayer = null;
            Debug.Log($"Selected Card: {selectedCard.cardName}");  
            //HandlePlayerTurn(currentPlayer);*/
            
            // If the guess is correct and the player's hand isn't empty, allow another guess.
            if (!IsPlayerHandEmpty(currentPlayer))
            {
                // Allow the player to make another guess without drawing a card or ending the turn.
                //selectedCard = null;
                //selectedPlayer = null;
                HandlePlayerTurn(currentPlayer);
            }
            else if (DeckManager.Instance.CurrentDeck != null && DeckManager.Instance.CurrentDeck.DeckCards.Count > 0)
            {
                // If the player's hand is empty but the deck isn't, draw a card from the deck.
                DrawCardFromDeck();
                // After drawing a card, re-evaluate the hand.
                if (!IsPlayerHandEmpty(currentPlayer))
                {
                    // Allow the player to make another guess.
                    selectedCard = null;
                    selectedPlayer = null;
                    HandlePlayerTurn(currentPlayer);
                }
            }
            // No need to check for the deck being empty here, as we've already handled that case.
        }
        else
        {
            // If the guess is wrong, draw a card from the deck and end the turn.
            DisplayMessage($"{selectedPlayer.playerName} does not have {selectedCard.cardName}.");
            DrawCardFromDeck();
            EndTurn(); // This is the correct place to call EndTurn for an incorrect guess.
        }
    }

    private void TransferCard(Card selectedCard, Player curPlayer)
    {
        selectedPlayer.RemoveCardFromHand(selectedCard);
        currentPlayer.AddCardToHand(selectedCard);
    }

    private bool IsPlayerHandEmpty(Player currentPlayer)
    {
        return currentPlayer.IsHandEmpty();
    }

    private void EndTurn()
    {
        NextCurrentPlayer();
    }

    private void NextCurrentPlayer()
    {
        int currentIndex = PlayerManager.Instance.players.IndexOf(currentPlayer);
        int nextIndex = (currentIndex + 1) % PlayerManager.Instance.players.Count;

        int skippedPlayers = 0;

        while (skippedPlayers < PlayerManager.Instance.players.Count)
        {
            Player nextPlayer = PlayerManager.Instance.players[nextIndex];

            if (!nextPlayer.IsHandEmpty())
            {
                currentPlayer.HasTurn.Value = false;
                nextPlayer.HasTurn.Value = true;
                currentPlayer = nextPlayer;
                return; // Found a non-empty-handed player, exit the loop
            }

            // If the next player's hand is empty, skip them and move to the next one.
            nextIndex = (nextIndex + 1) % PlayerManager.Instance.players.Count;
            skippedPlayers++;
        }

        // If all players have empty hands, you can handle this case or end the game.
        // For example, you can call a function to end the game.
        // HandleAllEmptyHands();
    }


    private void CheckForQuartets()
    {
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
        /*bool allHandsEmpty = true;
        foreach (Player player in PlayerManager.Players)
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
        }*/
    }

    private void GameEnd()
    {
        Debug.Log($"{Time.time}: Game Ended");
        // Call the method to display end game results
        //gameFlowManager.DisplayEndGameResults();
    }
}
