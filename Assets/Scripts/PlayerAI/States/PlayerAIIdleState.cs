using UnityEngine;

public class PlayerAIIdleState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIIdleState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    // Method called when entering the idle state
    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Idle Enter");
    }

    // Method called during state update
    protected override void OnUpdate()
    {
        CheckSwitchState(); // Check if state should transition to another state
    }

    // Method called when exiting the idle state
    protected override void OnExit()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Idle Exit");
    }

    // Check if state should transition to another state based on game state and player's turn
    protected override void CheckSwitchState()
    {
        // If it's not player's turn, do nothing
        if (!_stateMachine.IsMyTurn)
            return;

        // Determine which state to switch to based on the current game state
        switch (_stateMachine.CurrentGameState)
        {
            case CurrentGameState.PreFlop:
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
                break;
            case CurrentGameState.Flop:
                SwitchState(_stateFactory.FlopState);
                break;
            case CurrentGameState.Turn:
                SwitchState(_stateFactory.TurnState);
                break;
            case CurrentGameState.River:
                SwitchState(_stateFactory.RiverState);
                break;
            default:
                // Handle any other game states if necessary
                break;
        }
    }
}
