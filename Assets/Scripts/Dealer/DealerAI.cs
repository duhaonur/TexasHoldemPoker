using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CardSettings;

public class DealerAI : MonoBehaviour, IControlStateMachine
{
    public Dictionary<int, int> PlayersBetAmount;
    public Stack<Card> Deck;
    public List<Card> CommunityCards;

    public List<HandRank> HandRank;
    public List<float> strength;

    public CurrentGameState GameState = CurrentGameState.PreFlop;
    [Header("Card Amounts To Deal")]
    public int MaxPocketCardsPlayersCanHave = 2;
    public int FlopStateCommunityCardAmount = 3;
    public int TurnStateCommunityCardAmount = 1;
    public int RiverStateCommunityCardAmount = 1;

    public int PlayerCount;
    public int CurrentSmallBlind;
    public int CurrentBigBlind;
    public int CurrentPlayersTurn;

    public bool WaitForThePlayer = false;
    public bool GiveTurnToNextPlayer = false;
    public bool ReadyForNextStage = false;

    private State<DealerAI, DealerStateFactory> _currentState;
    private DealerStateFactory _stateFactory;

    public Dictionary<int, Seat> Players;

    private bool _gameStarted = false;

    private WaitForSeconds _waitForSeconds = new WaitForSeconds(0.1f);

    public void SetNewState<T, U>(State<T, U> newState)
    where T : IControlStateMachine
    where U : StateFactory
    {
        _currentState = newState as State<DealerAI, DealerStateFactory>;
    }
    private void OnEnable()
    {
        GameEvents.OnSentDeck += GetDeck;
        GameEvents.OnSendPlayerCount += GetPlayerCount;
        GameEvents.OnStartGame += StartTheGame;
        GameEvents.OnCommunityCard += GetCommunityCard;
        GameEvents.OnPlayerFold += RemovePlayerFromTheGame;
        GameEvents.OnPlayerFinishedTurn += PlayerFinishedItsTurn;
//GameEvents.OnPlayerRaise += SomeoneRaised;
    }
    private void OnDisable()
    {
        GameEvents.OnSentDeck -= GetDeck;
        GameEvents.OnSendPlayerCount -= GetPlayerCount;
        GameEvents.OnStartGame -= StartTheGame;
        GameEvents.OnCommunityCard -= GetCommunityCard;
        GameEvents.OnPlayerFold -= RemovePlayerFromTheGame;
        GameEvents.OnPlayerFinishedTurn -= PlayerFinishedItsTurn;
        //GameEvents.OnPlayerRaise -= SomeoneRaised;
    }
    private void Awake()
    {
        Debug.Log($"Highest Bet {SharedData.HighestBet}");
        Deck = new Stack<Card>();
        CommunityCards = new List<Card>();
        PlayersBetAmount = new Dictionary<int, int>();
    }
    private void Start()
    {
        CurrentBigBlind = 1;
        CurrentSmallBlind = 0;
        CurrentPlayersTurn = 0;

        _stateFactory = new DealerStateFactory(this);
        SharedData.MinimumBet = 100;
    }
    private void Update()
    {
        if (_gameStarted)
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
    private IEnumerator DealCommunityCardEnumerator(int cardAmountToDeal)
    {
        for (int i = 0; i < cardAmountToDeal; i++)
        {
            Card card = Deck.Pop();
            yield return StartCoroutine(card.HideCard(0.2f));
            GameEvents.CallCommunityCard(Deck.Pop());
        }
        yield return new WaitForSeconds(2);
        GiveTurnToNextPlayer = true;
        var predicted = PlayerAIMoveDecision.PredictHand(CommunityCards, 100);
        var hand = PlayerAIMoveDecision.SetBestPossibleCommunityCard(predicted);
        Debug.Log($"Dealer AI Best Hand: {hand}");
        HandRank = new List<HandRank>(predicted.Keys);
        strength = new List<float>(predicted.Values);
    }
    private void GetCommunityCard(Card card)
    {
        CommunityCards.Add(card);
    }
    private IEnumerator DealTheCardsEnumerator(int cardAmountToDeal)
    {
        for (int i = 0; i < cardAmountToDeal; i++)
        {
            for (int j = 0; j < PlayerCount; j++)
            {
                Card card = Deck.Pop();
                yield return StartCoroutine(card.HideCard(0.2f));
                GameEvents.CallSendCardToHand(Players[j].SeatId, card);
            }
        }
        yield return new WaitForSeconds(2);
        GiveTurnToNextPlayer = true;
    }
    private void PlayerFinishedItsTurn(int betAmount, int currentBet, int id)
    {
        SharedData.Pot += betAmount;
        SharedData.HighestBet = currentBet > SharedData.HighestBet ? currentBet : SharedData.HighestBet;
        GameEvents.CallUpdatePotText();
        Debug.Log($"Highest Bet {SharedData.HighestBet}");
        if (PlayersBetAmount.ContainsKey(id))
        {
            PlayersBetAmount[id] = currentBet;
        }

        WaitForThePlayer = false;
        GiveTurnToNextPlayer = true;
    }
    private void RemovePlayerFromTheGame(int id)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].SeatId == id)
            {
                Players.Remove(i);
                break;
            }
        }
        Dictionary<int, Seat> reordered = new Dictionary<int, Seat>();
        int counter = 0;
        foreach (var value in Players.Values)
        {
            reordered.Add(counter, value);
            counter++;
        }
        Players = new Dictionary<int, Seat>(reordered);

        PlayersBetAmount.Remove(id);
        CurrentPlayersTurn--;
        PlayerCount = Players.Count;
        WaitForThePlayer = false;
        GiveTurnToNextPlayer = true;
    }

    private void StartTheGame()
    {
        StartCoroutine(DisplayDeckAndStartTheGame());

    }
    private IEnumerator DisplayDeckAndStartTheGame()
    {
        List<Card> cards = new List<Card>(Deck);

        foreach (Card card in cards)
        {
            StartCoroutine(card.DisplayCard(0f));
            yield return null;
        }
        Debug.Log("Works or not");
        _currentState = _stateFactory.PreFlopState;
        _currentState.EnterState();
        _gameStarted = true;
    }

    private void GetPlayerCount(Dictionary<int, Seat> seats)
    {
        Players = seats;

        foreach (var id in seats.Keys)
        {
            if (!PlayersBetAmount.ContainsKey(id))
            {
                Debug.Log($"Bet Amount Count {PlayersBetAmount.Count}");
                PlayersBetAmount.Add(id, 0);
            }
        }

        PlayerCount = seats.Count;
    }
    private void GetDeck(Stack<Card> deck)
    {
        Deck = deck;
    }
}
public enum CurrentGameState
{
    PreFlop,
    Flop,
    Turn,
    River,
    Showdown,
}