using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CardSettings;

public class PlayerController : MonoBehaviour, IPlayer
{
    public PlayerUIManager UIManager;
    public List<Card> HoleHand;
    public List<Card> FullHand;

    public Seat Seat;
    public SeatUI SeatUI;

    public CinemachineVirtualCamera DefaultCamera;
    [SerializeField] private CinemachineFreeLook _freeLookCamera;
    public Vector3 FirstCardPosition;
    public Vector3 SecondCardPosition;
    public Vector3 CardRotation;

    public HandRank FullHandRank;

    public int FullHandSumOfRanks;
    public int HandHighCardRank;

    public int SeatId;

    public bool IsSmallBlind = false;
    public bool IsBigBlind = false;

    private InputController _inputController;

    private int _cardCounter = 0;

    void Start()
    {
        HoleHand = new List<Card>();
        FullHand = new List<Card>();

        _inputController = CheckNull(_inputController);

        PlayerData.TotalMoney = 1000;
    }
    private void OnEnable()
    {
        SeatEvents.OnGetSeatId += GetSeatId;
        GameEvents.OnSetSmallBlind += SmallBlind;
        GameEvents.OnSetBigBlind += BigBlind;
        GameEvents.OnSendCardToHand += GetCard;
        GameEvents.OnCommunityCard += GetCommunityCard;
        PlayerEvents.OnPlayerRaise += PlayerRaise;
        PlayerEvents.OnPlayerCall += PlayerCall;
        PlayerEvents.OnPlayerCheck += PlayerCheck;
        PlayerEvents.OnPlayerFold += PlayerFold;
        PlayerEvents.OnMoveCamera += MoveCamera;
    }
    private void OnDisable()
    {
        SeatEvents.OnGetSeatId -= GetSeatId;
        GameEvents.OnSetSmallBlind -= SmallBlind;
        GameEvents.OnSetBigBlind -= BigBlind;
        GameEvents.OnSendCardToHand -= GetCard;
        GameEvents.OnCommunityCard -= GetCommunityCard;
        PlayerEvents.OnPlayerRaise -= PlayerRaise;
        PlayerEvents.OnPlayerCall -= PlayerCall;
        PlayerEvents.OnPlayerCheck -= PlayerCheck;
        PlayerEvents.OnPlayerFold -= PlayerFold;
        PlayerEvents.OnMoveCamera -= MoveCamera;
    }
    public (HandRank, int, int, int) SendHand()
    {
        return (FullHandRank, FullHandSumOfRanks, HandHighCardRank, SeatId);
    }
    private void PlayerFold()
    {
        GameEvents.CallPlayerFold(SeatId);
    }

    private void PlayerCheck()
    {
        PlayerData.CurrentBet = SharedData.HighestBet;
        GameEvents.CallPlayerFinishedTurn(PlayerData.CurrentBet, PlayerData.CurrentBet, SeatId);
    }

    private void PlayerCall(int callAmount)
    {
        throw new NotImplementedException();
    }

    private void PlayerRaise(int raiseAmount)
    {
        PlayerData.CurrentBet += raiseAmount;
        Debug.Log($"Player Current Bet {PlayerData.CurrentBet}");
        SeatUI.UpdateBetText(PlayerData.CurrentBet);
        GameEvents.CallPlayerFinishedTurn(raiseAmount, PlayerData.CurrentBet, SeatId);
        GameEvents.CallPlayerRaise(true);
    }

    private void GetSeatId(int id, Seat seat)
    {
        SeatId = id;
        Seat = seat;
        SeatUI = seat.SeatUI;
        _freeLookCamera.m_Follow = seat.CameraLook;
        _freeLookCamera.m_LookAt = seat.CameraLook;
        DefaultCamera.Priority = 0;
    }
    private void GetCard(int id, Card card)
    {
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
        FullHand.Add(card);
        EvaluateHand();
    }
    private void EvaluateHand()
    {
        (FullHandRank, FullHandSumOfRanks) = HandEvaluation.EvaluateHand(FullHand);
        var highCard = HandEvaluation.IsHighCard(HoleHand);
        HandHighCardRank = highCard.sumOfRanks;
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
    private void MoveCamera(float x, float y)
    {
        if (x > 0)
        {
            _freeLookCamera.m_XAxis.m_InputAxisValue = 1;
        }
        else if (x < 0)
        {
            _freeLookCamera.m_XAxis.m_InputAxisValue = -1;
        }
        else
        {
            _freeLookCamera.m_XAxis.m_InputAxisValue = 0;
        }

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
