public class DealerIdleState : State<DealerAI, DealerStateFactory>
{
    public DealerIdleState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    // Called when entering the state
    protected override void OnEnter()
    {
    }

    // Called every frame while in the state
    protected override void OnUpdate()
    {
        // Check for state transition conditions
        CheckSwitchState();
    }

    // Called when exiting the state
    protected override void OnExit()
    {
    }

    // Check conditions for transitioning to another state
    protected override void CheckSwitchState()
    {
        // Transition conditions

        // Someone raised and it's time to give turn to next player
        if (!_stateMachine.WaitForThePlayer && _stateMachine.GiveTurnToNextPlayer && !_stateMachine.ReadyForNextStage && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.GivePlayerTurnState);
        }
        // Normal play without raise
        else if (!_stateMachine.WaitForThePlayer && _stateMachine.GiveTurnToNextPlayer && _stateMachine.CurrentPlayersTurn < _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.GivePlayerTurnState);
        }
        // Transition to FlopState after PreFlop stage
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.PreFlop && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.FlopState);
        }
        // Transition to TurnState after Flop stage
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.Flop && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.TurnState);
        }
        // Transition to RiverState after Turn stage
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.Turn && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.RiverState);
        }
        // Transition to ShowdownState after River stage
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.River && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.ShowdownState);
        }
    }
}
