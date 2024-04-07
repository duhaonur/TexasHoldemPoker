using UnityEngine;

public class PlayerAIIdleState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIIdleState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Idle Enter");
    }
    protected override void OnUpdate()
    {
        CheckSwitchState();
    }
    protected override void OnExit()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Idle Exit");
    }
    protected override void CheckSwitchState()
    {
        if (!_stateMachine.IsMyTurn)
            return;

        if (_stateMachine.CurrentGameState == CurrentGameState.PreFlop)
        {
            if (_stateMachine.IsSmallBlind)
            {
                SwitchState(_stateFactory.SmallBlindState);
            }
            else if (_stateMachine.IsBigBlind)
            {
                SwitchState(_stateFactory.BigBlindState);
            }
            else
            {
                SwitchState(_stateFactory.PreFlopState);
            }
        }
        else if (_stateMachine.CurrentGameState == CurrentGameState.Flop)
        {
            SwitchState(_stateFactory.FlopState);
        }
        else if (_stateMachine.CurrentGameState == CurrentGameState.Turn)
        {
            SwitchState(_stateFactory.TurnState);
        }
        else if (_stateMachine.CurrentGameState == CurrentGameState.River)
        {
            SwitchState(_stateFactory.RiverState);
        }
        else if (_stateMachine.CurrentGameState == CurrentGameState.Showdown)
        {
            SwitchState(_stateFactory.ShowdownState);
        }
    }
}
