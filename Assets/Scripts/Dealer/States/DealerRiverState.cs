using UnityEngine;

public class DealerRiverState : State<DealerAI, DealerStateFactory>
{
    public DealerRiverState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        Debug.Log("Dealer River Enter");

        // Reset flags
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;
        _stateMachine.GameState = CurrentGameState.River;

        // Deal community card
        _stateMachine.DealCommunityCard(_stateMachine.RiverStateCommunityCardAmount);

        // Reset player turn counter
        _stateMachine.CurrentPlayersTurn = 0;

        // Check for state transition
        CheckSwitchState();
    }

    protected override void OnUpdate()
    {
        // No update logic needed for this state
    }

    protected override void OnExit()
    {
        Debug.Log("Dealer River Exit");
    }

    protected override void CheckSwitchState()
    {
        // Transition to IdleState
        SwitchState(_stateFactory.IdleState);
    }
}
