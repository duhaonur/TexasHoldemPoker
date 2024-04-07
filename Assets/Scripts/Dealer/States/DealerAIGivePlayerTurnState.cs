using System.Collections;
using UnityEngine;

public class DealerAIGivePlayerTurnState : State<DealerAI, DealerStateFactory>
{
    private int _timesWeWereHere = 0;
    private int _readyPlayers = 0;
    private int _lastRaisedPlayer = -1;
    public DealerAIGivePlayerTurnState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {

    }
    protected override void OnEnter()
    {
        Debug.Log("EnterDealerTurn");
        if (_stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            Debug.Log("TurnEnterFirstIf");
            GiveTurnForRaiseOrCall();
        }
        else
        {
            GiveTurnToPlayer();
        }
    }
    protected override void OnUpdate()
    {
        Debug.Log("UpdateDealerTurn");
    }
    protected override void OnExit()
    {
        _timesWeWereHere++;
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
    private void GiveTurnForRaiseOrCall()
    {
        for (int i = 0; i < _stateMachine.PlayerCount; i++)
        {
            Debug.Log($"For Loop{i}");
            if (_stateMachine.Players.TryGetValue(i, out Seat seat))
            {
                if (_stateMachine.PlayersBetAmount.TryGetValue(seat.SeatId, out int betAmount))
                {
                    if (_lastRaisedPlayer == seat.SeatId)
                        continue;

                    Debug.Log($"Seat ID: {seat.SeatId} Bet Amount: {betAmount}");
                    if (betAmount < SharedData.HighestBet)
                    {
                        Debug.Log($"Inside Current Player {seat.SeatId}");
                        GameEvents.CallGivePlayerTheTurn(seat.SeatId, _stateMachine.GameState);
                        _lastRaisedPlayer = seat.SeatId;
                        _stateMachine.WaitForThePlayer = true;
                        _stateMachine.GiveTurnToNextPlayer = false;
                        break;
                    }
                    else
                    {
                        _readyPlayers++;
                        Debug.Log($"Ready Player{_readyPlayers}");
                    }
                }
            }
        }
        if (_readyPlayers >= _stateMachine.PlayerCount - 1)
        {
            _stateMachine.ReadyForNextStage = true;
            _readyPlayers = 0;
        }

        _lastRaisedPlayer = -1;
        CheckSwitchState();
    }
    private void GiveTurnToPlayer()
    {
        Debug.Log("TurnEnterSecondIf");
        Debug.Log(_stateMachine.CurrentPlayersTurn);
        int seatID = _stateMachine.Players[_stateMachine.CurrentPlayersTurn].SeatId;
        Debug.Log($"Current Seat Id {seatID} Times We Were Here {_timesWeWereHere}");
        GameEvents.CallGivePlayerTheTurn(seatID, _stateMachine.GameState);
        _stateMachine.WaitForThePlayer = true;
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.CurrentPlayersTurn++;

        CheckSwitchState();
    }
}
