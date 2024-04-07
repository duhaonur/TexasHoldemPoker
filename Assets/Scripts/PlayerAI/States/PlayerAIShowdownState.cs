using UnityEngine;
public class PlayerAIShowdownState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIShowdownState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Showdown Enter");
    }
    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Showdown Exit");
    }
    protected override void CheckSwitchState()
    {

    }
}
