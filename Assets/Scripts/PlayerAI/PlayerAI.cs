using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static CardSettings;

public class PlayerAI : MonoBehaviour, IControlStateMachine, IPlayer
{
    public List<Card> HoleHand;
    public List<Card> CommunityCards;
    public List<Card> FullHand;

    public Seat Seat;
    public SeatUI SeatUI;

    public AIDecisionWeightsSO WeightSettings;

    public HandRank FullHandRank;

    public int FullHandSumOfRanks;
    public int HandHighCardRank;

    public CurrentGameState CurrentGameState;

    public float RaiseMultiplierMin = 1.0f;
    public float RaiseMultiplierMax = 2.0f;
    public float MinRaiseFractionOfTotalMoney = 0.25f;
    public float MaxRaiseFractionOfTotalMoney = 1.0f;

    //public float HandWeight = 0.2f;
    //public float PotWeight = 0.15f;
    //public float CommunityCardsWeight = 0.3f;
    //public float FullHandWeight = 0.35f;

    //public float PreFlopRaiseThreshold = 0.7f;
    //public float PreFlopCallThreshold = 0.3f;

    //public float FlopRaiseThreshold = 0.7f;
    //public float FlopCallThreshold = 0.5f;

    //public float TurnRaiseThreshold = 1f;
    //public float TurnCallThreshold = 0.50f;

    //public float RiverRaiseThreshold = 1f;
    //public float RiverCallThreshold = 0.50f;

    public int CurrentBet;
    public int TotalMoney;

    public int SeatId;

    public bool IsSmallBlind;
    public bool IsBigBlind;
    public bool IsPlayerFolded;
    public bool IsMyTurn;

    private State<PlayerAI, PlayerAIStateFactory> _currentState;
    private PlayerAIStateFactory _stateFactory;

    private int _currentCardPosition = 0;

    public void SetNewState<T, U>(State<T, U> newState)
        where T : IControlStateMachine
        where U : StateFactory
    {
        _currentState = newState as State<PlayerAI, PlayerAIStateFactory>;
    }
    private void Awake()
    {
        HoleHand = new List<Card>();
        CommunityCards = new List<Card>();
        FullHand = new List<Card>();
    }
    void Start()
    {
        _stateFactory = new PlayerAIStateFactory(this);
        _currentState = _stateFactory.IdleState;
        _currentState.EnterState();

        float multiplier = Random.Range(1f, 2f);
        TotalMoney = (int)(PlayerData.TotalMoney * multiplier);
        string randomName = Seat.NameList.Names[Random.Range(0, Seat.NameList.Names.Length)];
        SeatUI.DisplayTexts(randomName);
        SeatUI.UpdateTotalMoneyText(TotalMoney);
    }
    private void OnEnable()
    {
        GameEvents.OnSetSmallBlind += SmallBlind;
        GameEvents.OnSetBigBlind += BigBlind;
        GameEvents.OnSendCardToHand += GetCard;
        GameEvents.OnCommunityCard += GetCommunityCard;
        GameEvents.OnGivePlayerTheTurn += GetTurn;
    }
    private void OnDisable()
    {
        GameEvents.OnSetSmallBlind -= SmallBlind;
        GameEvents.OnSetBigBlind -= BigBlind;
        GameEvents.OnSendCardToHand -= GetCard;
        GameEvents.OnCommunityCard -= GetCommunityCard;
        GameEvents.OnGivePlayerTheTurn -= GetTurn;
    }
    private void Update()
    {
        _currentState.UpdateState();
    }
    public (HandRank, int, int, int) SendHand()
    {
        return (FullHandRank, FullHandSumOfRanks, HandHighCardRank, SeatId);
    }
    public void SetLookAtConstraintSource(Transform player)
    {
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = player;
        source.weight = 1;
        SeatUI.NameAndMoneyConstraint.AddSource(source);
        SeatUI.BetConstraint.AddSource(source);
    }
    private void GetCard(int id, Card card)
    {
        if (SeatId == id)
        {
            HoleHand.Add(card);
            FullHand.Add(card);
            card.transform.SetParent(Seat.CardPositions[_currentCardPosition], false);
            _currentCardPosition++;
            StartCoroutine(card.DisplayCard(1f));
            EvaluateHand();
        }
    }
    private void GetCommunityCard(Card card)
    {
        CommunityCards.Add(card);
        FullHand.Add(card);
        EvaluateHand();
    }
    private void EvaluateHand()
    {
        (FullHandRank, FullHandSumOfRanks) = HandEvaluation.EvaluateHand(FullHand);
        var highCard = HandEvaluation.IsHighCard(HoleHand);
        HandHighCardRank = highCard.sumOfRanks;
    }
    private void GetTurn(int id, CurrentGameState state)
    {
        if(id == SeatId)
        {
            CurrentGameState = state;
            IsMyTurn = true;
        }
    }
    private void SmallBlind(int id)
    {
        if (SeatId == id)
            IsSmallBlind = true;
    }
    private void BigBlind(int id)
    {
        if (SeatId == id)
            IsBigBlind = true;
    }
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
