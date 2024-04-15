using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealerAIGivePlayerTurnState : State<DealerAI, DealerStateFactory>
{
    private List<Seat> _playersThatMustPlayAgain;

    private int _lastRaisedPlayer = 0;

    public DealerAIGivePlayerTurnState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {

    }
    protected override void OnEnter()
    {
        if (_stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            var checkIfSomeoneRaised = CheckIfSomeoneRaised();

            if (checkIfSomeoneRaised.isPlayersReady)
            {
                GiveTurnToPlayer(checkIfSomeoneRaised.players);
            }
            else
            {
                _stateMachine.ReadyForNextStage = true;
                CheckSwitchState();
            }

        }
        else
        {
            GiveTurnToPlayer(_stateMachine.Players);
        }

        CheckSwitchState();
    }
    protected override void OnUpdate()
    {
        Debug.Log("UpdateDealerTurn");
    }
    protected override void OnExit()
    {
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
    private void GiveTurnToPlayer(Dictionary<int,Seat> playerList)
    {
        if(playerList.TryGetValue(_stateMachine.CurrentPlayersTurn, out Seat seat))
        {
            GameEvents.CallGivePlayerTheTurn(seat.SeatId, _stateMachine.GameState);
            _stateMachine.WaitForThePlayer = true;
            _stateMachine.GiveTurnToNextPlayer = false;
            _stateMachine.CurrentPlayersTurn++;
        }
    }
    private void GiveTurnToPlayer(List<Seat> playerList)
    {
        Debug.Log($"Raiste Turn Player {playerList.First().gameObject.name}");
        GameEvents.CallGivePlayerTheTurn(playerList.First().SeatId, _stateMachine.GameState);
        _stateMachine.WaitForThePlayer = true;
        _stateMachine.GiveTurnToNextPlayer = false;
    }

    private (bool isPlayersReady, List<Seat> players) CheckIfSomeoneRaised()
    {
        bool doWeHaveARaiser = false;

        _lastRaisedPlayer = _lastRaisedPlayer >= _stateMachine.PlayerCount ? 0 : _lastRaisedPlayer;

        _playersThatMustPlayAgain = new List<Seat>();

        for (int i = _lastRaisedPlayer; i < _stateMachine.PlayerCount; i++)
        {
            if (_stateMachine.Players.TryGetValue(i, out Seat seat))
            {
                if (_stateMachine.PlayersBetAmount.TryGetValue(seat.SeatId, out int betAmount))
                {
                    Debug.Log($"Seat: {seat.SeatId} Bet Amount: {betAmount}");
                    if(betAmount != SharedData.HighestBet)
                    {
                        Debug.Log($"Player Added to list: {seat.gameObject.name}");
                        _playersThatMustPlayAgain.Add(seat);
                        doWeHaveARaiser = true;
                    }
                }
            }
        }
        _lastRaisedPlayer = _playersThatMustPlayAgain.Count > 0 ? _playersThatMustPlayAgain.First().SeatId : 0;
        return (doWeHaveARaiser, _playersThatMustPlayAgain);
    }
}
