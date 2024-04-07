using UnityEngine;
public class DealerRiverState : State<DealerAI, DealerStateFactory>
{
    public DealerRiverState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log("Deler River Enter");
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;
        _stateMachine.GameState = CurrentGameState.River;
        _stateMachine.DealCommunityCard(_stateMachine.RiverStateCommunityCardAmount);
        _stateMachine.CurrentPlayersTurn = 0;
        CheckSwitchState();
    }
    protected override void OnUpdate()
    {
        
    }
    protected override void OnExit()
    {
        Debug.Log("Deler River Exit");
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
}
