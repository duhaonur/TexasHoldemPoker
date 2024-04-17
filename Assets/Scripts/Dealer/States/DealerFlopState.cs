public class DealerFlopState : State<DealerAI, DealerStateFactory>
{
    public DealerFlopState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    // Called when entering the state
    protected override void OnEnter()
    {
        // Reset flags and set game state
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;
        _stateMachine.GameState = CurrentGameState.Flop;

        // Deal community cards and set current player's turn
        _stateMachine.DealCommunityCard(_stateMachine.FlopStateCommunityCardAmount);
        _stateMachine.CurrentPlayersTurn = 0;

        // Check if state transition is needed
        CheckSwitchState();
    }

    // Called every frame while in the state
    protected override void OnUpdate()
    {
        // No action needed
    }

    // Called when exiting the state
    protected override void OnExit()
    {
    }

    // Check conditions for transitioning to another state
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState); // Always transition to IdleState after dealing the flop
    }
}
