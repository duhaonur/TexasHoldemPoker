using UnityEngine;
public class DealerIdleState : State<DealerAI, DealerStateFactory>
{
    public DealerIdleState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log("Deler Idle Enter");
    }
    protected override void OnUpdate()
    {
        CheckSwitchState();
    }
    protected override void OnExit()
    {
        Debug.Log("Deler Idle Exit");
    }
    protected override void CheckSwitchState()
    {
        if (!_stateMachine.WaitForThePlayer && _stateMachine.GiveTurnToNextPlayer && !_stateMachine.ReadyForNextStage && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            Debug.Log("Someone Raised Look for call fold");
            SwitchState(_stateFactory.GivePlayerTurnState);
        }
        else if(!_stateMachine.WaitForThePlayer && _stateMachine.GiveTurnToNextPlayer && _stateMachine.CurrentPlayersTurn < _stateMachine.PlayerCount)
        {
            Debug.Log("Normal play");
            SwitchState(_stateFactory.GivePlayerTurnState);
        }
        else if(_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.PreFlop && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.FlopState);
        }
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.Flop && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.TurnState);
        }
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.Turn && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.RiverState);
        }
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.River && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.ShowdownState);
        }
        else if (_stateMachine.ReadyForNextStage && _stateMachine.GameState == CurrentGameState.Showdown && !_stateMachine.WaitForThePlayer && _stateMachine.CurrentPlayersTurn >= _stateMachine.PlayerCount)
        {
            SwitchState(_stateFactory.ResetState);
        }
    }
}
