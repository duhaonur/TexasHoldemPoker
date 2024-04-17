using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using static CardSettings;

public class PlayerController : MonoBehaviour, IPlayer
{
    // References
    public PlayerUIManager UIManager;
    public List<Card> HoleHand;
    public List<Card> FullHand;
    public Seat Seat;
    public SeatUI SeatUI;
    public CinemachineVirtualCamera DefaultCamera;
    [SerializeField] private CinemachineFreeLook _freeLookCamera;

    // Card positions and rotation
    public Vector3 FirstCardPosition;
    public Vector3 SecondCardPosition;
    public Vector3 CardRotation;

    // Hand information
    public HandRank FullHandRank;
    public int FullHandSumOfRanks;
    public int HandHighCardRank;

    // Player data
    public int SeatId;
    public bool IsSmallBlind = false;
    public bool IsBigBlind = false;
    public bool IsAllIn = false;
    public bool IsMyTurn = false;

    private InputController _inputController;
    private int _cardCounter = 0;

    void Start()
    {
        // Initialize lists and setup initial total money
        HoleHand = new List<Card>();
        FullHand = new List<Card>();

        _inputController = CheckNull(_inputController);

        PlayerData.TotalMoney = 1000;
        PlayerEvents.CallDisplayTotalMoney(PlayerData.TotalMoney);
    }

    private void OnEnable()
    {
        // Subscribe to events
        SeatEvents.OnGetSeatId += GetSeatId;

        GameEvents.OnSetSmallBlind += SmallBlind;
        GameEvents.OnSetBigBlind += BigBlind;
        GameEvents.OnSendCardToHand += GetCard;
        GameEvents.OnCommunityCard += GetCommunityCard;
        GameEvents.OnGivePlayerTheTurn += GetTurn;
        GameEvents.OnWinner += Winner;
        GameEvents.OnResetGame += GameReset;

        PlayerEvents.OnPlayerRaise += PlayerRaise;
        PlayerEvents.OnPlayerCall += PlayerCall;
        PlayerEvents.OnPlayerCheck += PlayerCheck;
        PlayerEvents.OnPlayerFold += PlayerFold;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        SeatEvents.OnGetSeatId -= GetSeatId;

        GameEvents.OnSetSmallBlind -= SmallBlind;
        GameEvents.OnSetBigBlind -= BigBlind;
        GameEvents.OnSendCardToHand -= GetCard;
        GameEvents.OnCommunityCard -= GetCommunityCard;
        GameEvents.OnGivePlayerTheTurn -= GetTurn;
        GameEvents.OnWinner -= Winner;
        GameEvents.OnResetGame -= GameReset;

        PlayerEvents.OnPlayerRaise -= PlayerRaise;
        PlayerEvents.OnPlayerCall -= PlayerCall;
        PlayerEvents.OnPlayerCheck -= PlayerCheck;
        PlayerEvents.OnPlayerFold -= PlayerFold;
    }

    // Interface method to send hand information
    public (HandRank, int, int, int, int) SendHand()
    {
        return (FullHandRank, FullHandSumOfRanks, HandHighCardRank, SeatId, PlayerData.CurrentBet);
    }

    private void GameReset()
    {
        // Reset player data
        if (PlayerData.TotalMoney <= 200)
        {
            PlayerData.TotalMoney = 1000;
        }
        HoleHand = new List<Card>();
        FullHand = new List<Card>();
        PlayerData.CurrentBet = 0;
        _cardCounter = 0;
        IsSmallBlind = false;
        IsBigBlind = false;
        IsAllIn = false;
        IsMyTurn = false;
        PlayerEvents.CallDisplayTotalMoney(PlayerData.TotalMoney);
    }

    private void PlayerFold()
    {
        // Call fold event
        GameEvents.CallPlayerFold(SeatId);
    }

    private void PlayerCheck()
    {
        // Call check event
        GameEvents.CallPlayerFinishedTurn(0, PlayerData.CurrentBet, SeatId);
    }

    private void Winner(int seatId, int wonChips)
    {
        // Update total money if player wins
        if (seatId == SeatId)
        {
            PlayerData.TotalMoney += wonChips;
            PlayerEvents.CallDisplayTotalMoney(PlayerData.TotalMoney);
        }
    }

    private void PlayerCall(int callAmount)
    {
        // Process player call
        callAmount = Mathf.Min(callAmount, PlayerData.TotalMoney);
        IsAllIn = callAmount >= PlayerData.TotalMoney;
        Seat.isAllIn = IsAllIn;
        PlayerData.CurrentBet += callAmount;
        PlayerData.TotalMoney -= callAmount;
        SeatUI.UpdateBetText(PlayerData.CurrentBet, IsAllIn);
        GameEvents.CallPlayerFinishedTurn(callAmount, PlayerData.CurrentBet, SeatId);
        PlayerEvents.CallDisplayTotalMoney(PlayerData.TotalMoney);
    }

    private void PlayerRaise(int raiseAmount)
    {
        // Process player raise
        raiseAmount = Mathf.Min(raiseAmount, PlayerData.TotalMoney);
        IsAllIn = raiseAmount >= PlayerData.TotalMoney;
        Seat.isAllIn = IsAllIn;
        PlayerData.CurrentBet += raiseAmount;
        PlayerData.TotalMoney -= raiseAmount;
        SeatUI.UpdateBetText(PlayerData.CurrentBet, IsAllIn);
        GameEvents.CallPlayerFinishedTurn(raiseAmount, PlayerData.CurrentBet, SeatId);
        PlayerEvents.CallDisplayTotalMoney(PlayerData.TotalMoney);
    }

    private void GetTurn(int seatId, CurrentGameState gameState)
    {
        // Process player turn
        if (SeatId == seatId)
        {
            if (IsAllIn)
            {
                GameEvents.CallPlayerFinishedTurn(0, PlayerData.CurrentBet, SeatId);
                IsMyTurn = false;
                return;
            }
            if (IsSmallBlind || IsBigBlind)
            {
                int betAmount = IsSmallBlind ? SharedData.MinimumBet / 2 : SharedData.MinimumBet;
                string sbOrBb = IsSmallBlind ? "SB" : "BB";
                PlayerData.CurrentBet += betAmount;
                PlayerData.TotalMoney -= betAmount;
                SeatUI.ChangeInformationText(sbOrBb);
                SeatUI.UpdateBetText(PlayerData.CurrentBet, IsAllIn);
                PlayerEvents.CallDisplayTotalMoney(PlayerData.TotalMoney);
                GameEvents.CallPlayerFinishedTurn(betAmount, PlayerData.CurrentBet, SeatId);
                IsSmallBlind = false;
                IsBigBlind = false;
                return;
            }
            IsMyTurn = true;
            PlayerEvents.CallDisplayUI();
        }
    }

    private void GetSeatId(int id, Seat seat)
    {
        // Assign seat information
        SeatId = id;
        Seat = seat;
        SeatUI = seat.SeatUI;
        _freeLookCamera.m_Follow = seat.CameraLook;
        _freeLookCamera.m_LookAt = seat.CameraLook;
        DefaultCamera.Priority = 0;
    }

    private void GetCard(int id, Card card)
    {
        // Get player's card
        if (SeatId == id)
        {
            HoleHand.Add(card);
            FullHand.Add(card);
            card.transform.SetParent(Camera.main.transform, false);
            card.transform.localPosition = _cardCounter == 0 ? FirstCardPosition : SecondCardPosition;
            card.transform.localRotation = Quaternion.Euler(CardRotation);
            _cardCounter++;
            StartCoroutine(card.DisplayCard(1f));
            EvaluateHand();
        }
    }

    private void GetCommunityCard(Card card)
    {
        // Get community card
        FullHand.Add(card);
        EvaluateHand();
    }

    private void EvaluateHand()
    {
        // Evaluate player's hand
        (FullHandRank, FullHandSumOfRanks) = HandEvaluation.EvaluateHand(FullHand);
        var highCard = HandEvaluation.IsHighCard(HoleHand);
        HandHighCardRank = highCard.sumOfRanks;
        PlayerEvents.CallDisplayHand(FullHandRank);
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
        // Check if a component is null and add it if necessary
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
