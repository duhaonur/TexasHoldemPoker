using System.Collections.Generic;
using UnityEngine;
using static CardSettings;

public class PlayerAI : MonoBehaviour, IControlStateMachine, IPlayer
{
    // Lists to store player's hand, community cards, full hand, and deck
    [HideInInspector] public List<Card> HoleHand;
    [HideInInspector] public List<Card> CommunityCards;
    [HideInInspector] public List<Card> FullHand;
    [HideInInspector] public List<Card> Deck;

    // References to UI elements
    public Seat Seat;
    public SeatUI SeatUI;

    // ScriptableObject to store AI decision weights
    public AIDecisionWeightsSO WeightSettings;

    // Variables to store hand rank, sum of ranks, high card rank, and other game-related information
    public HandRank FullHandRank;
    public int FullHandSumOfRanks;
    public int HandHighCardRank;

    public CurrentGameState CurrentGameState;

    public int CurrentBet;
    public int TotalMoney;
    public int SeatId;

    public bool IsSmallBlind;
    public bool IsBigBlind;
    public bool IsPlayerFolded;
    public bool IsMyTurn;
    public bool IsAllIn;

    // Current state and state factory for state machine implementation
    private State<PlayerAI, PlayerAIStateFactory> _currentState;
    private PlayerAIStateFactory _stateFactory;

    // Current position in the card list
    private int _currentCardPosition = 0;

    // Method to set a new state in the state machine
    public void SetNewState<T, U>(State<T, U> newState)
        where T : IControlStateMachine
        where U : StateFactory
    {
        _currentState = newState as State<PlayerAI, PlayerAIStateFactory>;
    }

    // Awake method to initialize lists
    private void Awake()
    {
        HoleHand = new List<Card>();
        CommunityCards = new List<Card>();
        FullHand = new List<Card>();
        Deck = new List<Card>();
    }

    // Start method to initialize the state machine and other variables
    void Start()
    {
        _stateFactory = new PlayerAIStateFactory(this);
        _currentState = _stateFactory.IdleState;
        _currentState.EnterState();

        // Initialize total money and display player's name
        float multiplier = Random.Range(1f, 2f);
        TotalMoney = (int)(PlayerData.TotalMoney * multiplier);
        string randomName = Seat.NameList.Names[Random.Range(0, Seat.NameList.Names.Length)];
        SeatUI.DisplayTexts(randomName);
        SeatUI.UpdateTotalMoneyText(TotalMoney);
    }

    // Subscribe to events when enabled
    private void OnEnable()
    {
        GameEvents.OnSetSmallBlind += SmallBlind;
        GameEvents.OnSetBigBlind += BigBlind;
        GameEvents.OnSendCardToHand += GetCard;
        GameEvents.OnCommunityCard += GetCommunityCard;
        GameEvents.OnGivePlayerTheTurn += GetTurn;
        GameEvents.OnWinner += Winner;
        GameEvents.OnResetGame += GameReset;
    }

    // Unsubscribe from events when disabled
    private void OnDisable()
    {
        GameEvents.OnSetSmallBlind -= SmallBlind;
        GameEvents.OnSetBigBlind -= BigBlind;
        GameEvents.OnSendCardToHand -= GetCard;
        GameEvents.OnCommunityCard -= GetCommunityCard;
        GameEvents.OnGivePlayerTheTurn -= GetTurn;
        GameEvents.OnWinner -= Winner;
        GameEvents.OnResetGame -= GameReset;
    }

    // Update method to handle state machine updates
    private void Update()
    {
        _currentState.UpdateState();
    }

    // Method to send the player's hand information
    public (HandRank, int, int, int, int) SendHand()
    {
        return (FullHandRank, FullHandSumOfRanks, HandHighCardRank, SeatId, CurrentBet);
    }

    // Method to reset the game state
    private void GameReset()
    {
        if (TotalMoney <= 300)
        {
            TotalMoney = Random.Range(1000, 3000);
            SeatUI.UpdateTotalMoneyText(TotalMoney);
        }

        // Reset all lists and variables
        HoleHand = new List<Card>();
        CommunityCards = new List<Card>();
        FullHand = new List<Card>();
        Deck = new List<Card>();
        CurrentBet = 0;
        _currentCardPosition = 0;
        IsBigBlind = false;
        IsSmallBlind = false;
        IsPlayerFolded = false;
        IsMyTurn = false;
        IsAllIn = false;
    }

    // Method to handle winning chips
    private void Winner(int seatId, int wonChips)
    {
        if (seatId == SeatId)
        {
            TotalMoney += wonChips;
            SeatUI.UpdateTotalMoneyText(TotalMoney);
        }
    }

    // Method to handle receiving a card
    private void GetCard(int id, Card card)
    {
        if (SeatId == id)
        {
            HoleHand.Add(card);
            FullHand.Add(card);
            card.transform.SetParent(Seat.CardPositions[_currentCardPosition], false);
            card.transform.localPosition = Vector3.zero;
            _currentCardPosition++;
            StartCoroutine(card.DisplayCard(1f));
            EvaluateHand();
        }
    }

    // Method to handle receiving a community card
    private void GetCommunityCard(Card card)
    {
        CommunityCards.Add(card);
        FullHand.Add(card);
        EvaluateHand();
    }

    // Method to evaluate the player's hand
    private void EvaluateHand()
    {
        (FullHandRank, FullHandSumOfRanks) = HandEvaluation.EvaluateHand(FullHand);
        var highCard = HandEvaluation.IsHighCard(HoleHand);
        HandHighCardRank = highCard.sumOfRanks;
    }

    // Method to handle the player's turn
    private void GetTurn(int id, CurrentGameState state)
    {
        if (id == SeatId)
        {
            if (IsAllIn)
            {
                GameEvents.CallPlayerFinishedTurn(0, CurrentBet, SeatId);
                return;
            }

            CurrentGameState = state;
            IsMyTurn = true;
        }
    }

    // Method to handle the small blind action
    private void SmallBlind(int id)
    {
        if (SeatId == id)
            IsSmallBlind = true;
    }

    // Method to handle the big blind action
    private void BigBlind(int id)
    {
        if (SeatId == id)
            IsBigBlind = true;
    }

    // Method to check if a component is null and create it if needed
    private T CheckNull<T>(T component) where T : Component
    {
        if (component == null)
        {
            if (TryGetComponent(out T comp))
            {
                return comp;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }
        else
        {
            return component;
        }
    }
}
