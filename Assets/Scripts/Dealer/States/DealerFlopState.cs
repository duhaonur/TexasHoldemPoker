using UnityEngine;
public class DealerFlopState : State<DealerAI, DealerStateFactory>
{
    public DealerFlopState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log("Deler Flop Enter");
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;
        _stateMachine.GameState = CurrentGameState.Flop;
        _stateMachine.DealCommunityCard(_stateMachine.FlopStateCommunityCardAmount);
        _stateMachine.CurrentPlayersTurn = 0;

        CheckSwitchState();
    }
    protected override void OnUpdate()
    {
        
    }
    protected override void OnExit()
    {
        Debug.Log("Deler Flop Exit");
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
}
