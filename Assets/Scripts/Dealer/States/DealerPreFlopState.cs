using UnityEngine;
public class DealerPreFlopState : State<DealerAI, DealerStateFactory>
{
    public DealerPreFlopState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        Debug.Log("Dealer Pre-Flop Enter");
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;

        // Increment the small blind position
        _stateMachine.CurrentSmallBlind = (_stateMachine.CurrentSmallBlind + 1) % _stateMachine.PlayerCount;
        // Increment the big blind position
        _stateMachine.CurrentBigBlind = (_stateMachine.CurrentBigBlind + 1) % _stateMachine.PlayerCount;

        _stateMachine.CurrentPlayersTurn = _stateMachine.CurrentSmallBlind;

        // Notify the game about small blind and big blind positions
        GameEvents.CallSetSmallBlind(_stateMachine.CurrentSmallBlind);
        GameEvents.CallSetBigBlind(_stateMachine.CurrentBigBlind);

        // Deal cards to players
        _stateMachine.DealTheCards(_stateMachine.MaxPocketCardsPlayersCanHave);

        CheckSwitchState();
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {
        Debug.Log("Dealer Pre-Flop Exit");
    }

    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
}
