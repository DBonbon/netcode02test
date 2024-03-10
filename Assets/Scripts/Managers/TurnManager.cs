using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    
    private Card selectedCard;
    private Player selectedPlayer;
    private Player currentPlayer;
    private bool isPlayerUIEnabled = true;
    private bool isDrawingCard = false;
    public delegate void EnableUIEvent(bool enableUI);
    public static event EnableUIEvent OnEnableUI;
    // Start is called before the first frame update
    
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

        Debug.Log($"Turn assigned to player: {players[randomIndex].playerName.Value}");
    }

    private void StartTurnLoop()
    {
        Debug.Log("Turn Manager Started");
        AssignTurnToPlayer();
        
        currentPlayer = PlayerManager.Instance.players.Find(player => player.HasTurn.Value);

        if (currentPlayer != null)
        {
            StartCoroutine(TurnLoop());
        }
        else
        {
            Debug.LogError("No initial player with hasTurn == true found.");
        }
    }

    private System.Collections.IEnumerator TurnLoop()
    {
        while (true)
        {
            if (currentPlayer.HasTurn.Value)
            {
                HandlePlayerTurn(currentPlayer);
                //hasHandledCurrentPlayer = true;
                
            }

            if (!currentPlayer.HasTurn.Value)
            {
                //hasHandledCurrentPlayer = false;
                NextCurrentPlayer();
            }

            yield return null;
        }
    }

    private void HandlePlayerTurn(Player currentPlayer)
    {
        //MakeGuess(currentPlayer);
        Debug.Log($"Selected Card: {selectedCard.cardName}");

        if (selectedCard != null && selectedPlayer != null)
        {
            AskForCard(selectedCard, selectedPlayer);

            // Reset selectedCard and selectedPlayer to null
            selectedCard = null;
            selectedPlayer = null;
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
