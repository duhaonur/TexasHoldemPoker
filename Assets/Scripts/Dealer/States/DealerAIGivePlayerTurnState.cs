using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealerAIGivePlayerTurnState : State<DealerAI, DealerStateFactory>
{
    private List<Seat> _playersThatMustPlayAgain = new List<Seat>();
    private bool _someoneRaised = false;

    public DealerAIGivePlayerTurnState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {

    }

    // Called when entering the state
    protected override void OnEnter()
    {

    }

    // Called every frame while in the state
    protected override void OnUpdate()
    {
        // Check conditions and progress the state accordingly
        CheckIfWeReadyForNextStage();
    }

    // Called when exiting the state
    protected override void OnExit()
    {
        Debug.Log("Dealer Give Turn Exit");
    }

    // Check conditions for transitioning to another state
    protected override void CheckSwitchState()
    {
        // Transition to IdleState if ready for the next stage
        if (_stateMachine.ReadyForNextStage)
            SwitchState(_stateFactory.IdleState);
    }

    // Check if we are ready to move to the next stage
    private void CheckIfWeReadyForNextStage()
    {
        // If we are waiting for a player or not giving turn to the next player, return
        if (_stateMachine.WaitForThePlayer || !_stateMachine.GiveTurnToNextPlayer)
            return;

        // If there is only one player left, move to the ShowdownState
        if (_stateMachine.PlayerCount == 1)
        {
            SwitchState(_stateFactory.ShowdownState);
            return;
        }

        // If we have given turn to all players
        if (_stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            // Check if someone raised
            if (_playersThatMustPlayAgain.Count == 0)
            {
                _someoneRaised = CheckIfSomeoneRaised();
            }

            // If someone raised, give turn to those players
            if (_someoneRaised)
            {
                GiveTurnToPlayer(_playersThatMustPlayAgain);
            }
            // Otherwise, mark ready for the next stage and check for state transition
            else
            {
                _stateMachine.ReadyForNextStage = true;
                CheckSwitchState();
            }

        }
        // If there are still players left to give turn, continue giving turn to them
        else
        {
            GiveTurnToPlayer(_stateMachine.Players);
        }
    }

    // Give turn to a list of players
    private void GiveTurnToPlayer(Dictionary<int, Seat> playerList)
    {
        if (playerList.TryGetValue(_stateMachine.CurrentPlayersTurn, out Seat seat))
        {
            _stateMachine.WaitForThePlayer = true;
            _stateMachine.GiveTurnToNextPlayer = false;
            GameEvents.CallGivePlayerTheTurn(seat.SeatId, _stateMachine.GameState);
            _stateMachine.CurrentPlayersTurn++;
        }
    }

    // Give turn to a list of players who must play again due to raise
    private void GiveTurnToPlayer(List<Seat> playerList)
    {
        Debug.Log($"Raise Turn Player {playerList.First().gameObject.name}");
        _stateMachine.WaitForThePlayer = true;
        _stateMachine.GiveTurnToNextPlayer = false;
        GameEvents.CallGivePlayerTheTurn(_playersThatMustPlayAgain.First().SeatId, _stateMachine.GameState);
        _playersThatMustPlayAgain.Remove(_playersThatMustPlayAgain.First());
    }

    // Check if any player raised the bet
    private bool CheckIfSomeoneRaised()
    {
        bool doWeHaveARaiser = false;

        _playersThatMustPlayAgain = new List<Seat>();

        for (int i = 0; i < _stateMachine.PlayerCount; i++)
        {
            if (_stateMachine.Players.TryGetValue(i, out Seat seat))
            {
                if (_stateMachine.PlayersBetAmount.TryGetValue(seat.SeatId, out int betAmount))
                {
                    Debug.Log($"Seat: {seat.SeatId} Bet Amount: {betAmount}");
                    if (betAmount != SharedData.HighestBet && !seat.isAllIn)
                    {
                        Debug.Log($"Player Added to list: {seat.gameObject.name}");
                        _playersThatMustPlayAgain.Add(seat);
                        doWeHaveARaiser = true;
                    }
                }
            }
        }

        return doWeHaveARaiser;
    }
}
