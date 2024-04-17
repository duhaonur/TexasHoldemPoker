using UnityEngine;

public class DealerTurnState : State<DealerAI, DealerStateFactory>
{
    public DealerTurnState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        // Initialize state variables
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;
        _stateMachine.GameState = CurrentGameState.Turn;
        _stateMachine.DealCommunityCard(_stateMachine.TurnStateCommunityCardAmount);
        _stateMachine.CurrentPlayersTurn = 0;
        CheckSwitchState();
    }

    protected override void OnUpdate()
    {
        // No update logic needed for this state
    }

    protected override void OnExit()
    {
    }

    protected override void CheckSwitchState()
    {
        // Switch state to IdleState
        SwitchState(_stateFactory.IdleState);
    }
}
