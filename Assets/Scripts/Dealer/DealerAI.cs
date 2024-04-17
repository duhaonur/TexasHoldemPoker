using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardSettings;

// Enumeration for different game states
public enum CurrentGameState
{
    PreFlop,
    Flop,
    Turn,
    River,
    Showdown,
}

// DealerAI class responsible for managing the game flow
public class DealerAI : MonoBehaviour, IControlStateMachine
{
    // Dictionary to store players' bet amounts
    public Dictionary<int, int> PlayersBetAmount;

    // Stack to represent the deck of cards
    public Stack<Card> Deck;

    // List to hold community cards
    public List<Card> CommunityCards;

    // Lists to store hand ranks and strengths for AI decision making
    public List<HandRank> HandRank;
    public List<float> strength;

    // Current game state
    public CurrentGameState GameState = CurrentGameState.PreFlop;

    // Parameters for card dealing
    [Header("Card Amounts To Deal")]
    public int MaxPocketCardsPlayersCanHave = 2;
    public int FlopStateCommunityCardAmount = 3;
    public int TurnStateCommunityCardAmount = 1;
    public int RiverStateCommunityCardAmount = 1;

    // Player and game-related variables
    public int PlayerCount;
    public int CurrentSmallBlind = -1;
    public int CurrentBigBlind = 0;
    public int CurrentPlayersTurn;

    // Flags to control game flow
    public bool WaitForThePlayer = false;
    public bool GiveTurnToNextPlayer = false;
    public bool ReadyForNextStage = false;
    public bool GameEnded = false;
    public bool GameStarted = false;

    // Current state and state factory
    private State<DealerAI, DealerStateFactory> _currentState;
    private DealerStateFactory _stateFactory;

    // Dictionary to store players and their seats
    public Dictionary<int, Seat> Players;

    // Coroutine WaitForSeconds object
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(0.1f);

    // Set new state method
    public void SetNewState<T, U>(State<T, U> newState)
        where T : IControlStateMachine
        where U : StateFactory
    {
        _currentState = newState as State<DealerAI, DealerStateFactory>;
    }

    // Subscribe to events when the object is enabled
    private void OnEnable()
    {
        // Subscribe to game events
        GameEvents.OnSentDeck += GetDeck;
        GameEvents.OnSendPlayerCount += GetPlayerCount;
        GameEvents.OnStartGame += StartTheGame;
        GameEvents.OnCommunityCard += GetCommunityCard;
        GameEvents.OnPlayerFold += RemovePlayerFromTheGame;
        GameEvents.OnPlayerFinishedTurn += PlayerFinishedItsTurn;
        //GameEvents.OnPlayerRaise += SomeoneRaised;
    }

    // Unsubscribe from events when the object is disabled
    private void OnDisable()
    {
        // Unsubscribe from game events
        GameEvents.OnSentDeck -= GetDeck;
        GameEvents.OnSendPlayerCount -= GetPlayerCount;
        GameEvents.OnStartGame -= StartTheGame;
        GameEvents.OnCommunityCard -= GetCommunityCard;
        GameEvents.OnPlayerFold -= RemovePlayerFromTheGame;
        GameEvents.OnPlayerFinishedTurn -= PlayerFinishedItsTurn;
        //GameEvents.OnPlayerRaise -= SomeoneRaised;
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Initialize deck, community cards, and player bet amounts
        Deck = new Stack<Card>();
        CommunityCards = new List<Card>();
        PlayersBetAmount = new Dictionary<int, int>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize game variables
        CurrentBigBlind = 0;
        CurrentSmallBlind = -1;
        CurrentPlayersTurn = 0;

        // Initialize state factory
        _stateFactory = new DealerStateFactory(this);

        // Set minimum bet
        SharedData.MinimumBet = 100;
    }

    // Update is called once per frame
    private void Update()
    {
        // Update the current state if the game has started
        if (GameStarted)
            _currentState.UpdateState();
    }
    public void DealTheCards(int cardAmountToDeal)
    {
        StartCoroutine(DealTheCardsEnumerator(cardAmountToDeal));
    }
    public void DealCommunityCard(int cardAmountToDeal)
    {
        StartCoroutine(DealCommunityCardEnumerator(cardAmountToDeal));
    }
    public void ResetGame()
    {
        StartCoroutine(StartResetGame());
    }
    // Coroutine to deal the specified number of cards to players
    private IEnumerator DealTheCardsEnumerator(int cardAmountToDeal)
    {
        // Deal cards to each player
        for (int i = 0; i < cardAmountToDeal; i++)
        {
            for (int j = 0; j < PlayerCount; j++)
            {
                Card card = Deck.Pop();
                yield return StartCoroutine(card.HideCard(0.2f, false));
                GameEvents.CallSendCardToHand(Players[j].SeatId, card);
            }
        }

        // Wait for a short duration
        yield return new WaitForSeconds(2);

        // Set flag to give turn to next player
        GiveTurnToNextPlayer = true;
    }

    // Coroutine to deal the specified number of community cards
    private IEnumerator DealCommunityCardEnumerator(int cardAmountToDeal)
    {
        // Deal each community card
        for (int i = 0; i < cardAmountToDeal; i++)
        {
            Card card = Deck.Pop();
            yield return StartCoroutine(card.HideCard(0.2f, false));
            GameEvents.CallCommunityCard(Deck.Pop());
        }

        // Wait for a short duration
        yield return new WaitForSeconds(2);

        // Set flag to give turn to next player
        GiveTurnToNextPlayer = true;

        // Predict AI's best hand based on current community cards
        var predicted = PlayerAIMoveDecision.PredictHand(CommunityCards, 100);
        var hand = PlayerAIMoveDecision.SetBestPossibleCommunityCard(predicted);
        Debug.Log($"Dealer AI Best Hand: {hand}");

        // Store predicted hand ranks and strengths
        HandRank = new List<HandRank>(predicted.Keys);
        strength = new List<float>(predicted.Values);
    }
    // Coroutine to reset the game
    private IEnumerator StartResetGame()
    {
        // Reset game-related data
        PlayersBetAmount = new Dictionary<int, int>();
        CommunityCards = new List<Card>();
        Deck = new Stack<Card>();
        Players = new Dictionary<int, Seat>();
        GameState = CurrentGameState.PreFlop;

        // Call reset game event
        GameEvents.CallResetGame();

        // Wait for a short duration
        yield return new WaitForSeconds(5);

        // Call start game event
        GameEvents.CallStartGame();
    }

    // Event handler to receive community cards
    private void GetCommunityCard(Card card)
    {
        CommunityCards.Add(card);
    }
    // Event handler to handle when a player finishes their turn
    private void PlayerFinishedItsTurn(int betAmount, int currentBet, int id)
    {
        // Update pot and highest bet
        SharedData.Pot += betAmount;
        SharedData.HighestBet = currentBet > SharedData.HighestBet ? currentBet : SharedData.HighestBet;
        GameEvents.CallUpdatePotText();

        // Update player's bet amount
        if (PlayersBetAmount.ContainsKey(id))
        {
            PlayersBetAmount[id] = currentBet;
        }

        // Set flags
        WaitForThePlayer = false;
        GiveTurnToNextPlayer = true;
    }

    // Event handler to remove a player from the game
    private void RemovePlayerFromTheGame(int id)
    {
        // Remove player from the dictionary
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].SeatId == id)
            {
                Players.Remove(i);
                break;
            }
        }

        // Reorder the dictionary
        Dictionary<int, Seat> reordered = new Dictionary<int, Seat>();
        int counter = 0;
        foreach (var value in Players.Values)
        {
            reordered.Add(counter, value);
            counter++;
        }
        Players = new Dictionary<int, Seat>(reordered);

        // Remove player's bet amount
        PlayersBetAmount.Remove(id);

        // Update player count and flags
        CurrentPlayersTurn--;
        PlayerCount = Players.Count;
        WaitForThePlayer = false;
        GiveTurnToNextPlayer = true;
    }

    // Event handler to start the game
    private void StartTheGame()
    {
        StartCoroutine(DisplayDeckAndStartTheGame());
    }

    // Coroutine to display the deck and start the game
    private IEnumerator DisplayDeckAndStartTheGame()
    {
        // Display each card in the deck
        List<Card> cards = new List<Card>(Deck);
        foreach (Card card in cards)
        {
            StartCoroutine(card.DisplayCard(0f));
            yield return null;
        }

        // Set initial game state and start the game
        _currentState = _stateFactory.PreFlopState;
        _currentState.EnterState();
        GameStarted = true;
    }

    // Event handler to receive player count and their seats
    private void GetPlayerCount(Dictionary<int, Seat> seats)
    {
        // Initialize players and their bet amounts
        Players = new Dictionary<int, Seat>(seats);
        foreach (var id in seats.Keys)
        {
            if (!PlayersBetAmount.ContainsKey(id))
            {
                PlayersBetAmount.Add(id, 0);
            }
        }
        PlayerCount = seats.Count;
    }

    // Event handler to receive the deck of cards
    private void GetDeck(Stack<Card> deck)
    {
        Deck = deck;
    }
}
