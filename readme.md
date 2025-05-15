
🧠 Game Flow Recap
✅ 1. Network Initialization

    NetworkManagerUI.cs: Handles UI controls for starting the game as Host/Client.

    Launches NetworkManager.StartHost() or .StartClient().

🧩 2. Data Loading

    DataManager.cs:

        Loads players.json and cards.json (or animals.json) from StreamingAssets.

        Fires OnCardDataLoaded / OnPlayerDataLoaded events.

🎴 3. Card Setup

    CardManager.cs:

        Listens to OnCardDataLoaded.

        Stores all CardData in allCardsList.

        Spawns CardUI objects for each card (inactive by default).

        On server start, also spawns network Card objects (gameplay logic) and adds them to the deck.

📦 4. Deck Handling

    DeckManager.cs:

        Stores a singleton reference to the deck GameObject (DeckInstance).

        Provides methods to AddCardToDeck() or RemoveCardFromDeck().

👥 5. Player Management

    PlayerManager.cs:

        Handles player connections and stores references.

        Initializes player instances and links with data (names, IDs).

        Coordinates with CardManager to distribute cards.

🔁 6. Turn Logic

    TurnManager.cs:

        Controls the current player's turn.

        Likely includes logic for card selection, guessing, and drawing cards.

        May invoke DrawCardFromDeck() and AddCardToHand() on players.

🧠 7. Game Progress & Victory

    GameFlowManager.cs:

        Orchestrates general phases like game start, midgame, and end conditions.

        Could trigger QuartetsManager logic on successful guesses.

🃏 8. Quartets Completion

    QuartetsManager.cs:

        Manages completed sets.

        Moves cards from hand to display zone (e.g. QuartetsZone).

        Updates score and player state
