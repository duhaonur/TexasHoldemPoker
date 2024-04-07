using UnityEngine;

public class DealerTurnState : State<DealerAI, DealerStateFactory>
{
    public DealerTurnState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log("Deler Turn Enter");
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;
        _stateMachine.GameState = CurrentGameState.Turn;
        _stateMachine.DealCommunityCard(_stateMachine.TurnStateCommunityCardAmount);
        _stateMachine.CurrentPlayersTurn = 0;
        CheckSwitchState();
    }
    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        Debug.Log("Deler Turn Exit");
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
}
