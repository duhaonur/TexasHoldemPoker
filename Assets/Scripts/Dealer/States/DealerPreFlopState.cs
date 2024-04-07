using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerPreFlopState : State<DealerAI, DealerStateFactory>
{
    public DealerPreFlopState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log("Deler Pre-Flop Enter");
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.ReadyForNextStage = false;
        GameEvents.CallSetSmallBlind(_stateMachine.CurrentSmallBlind);
        GameEvents.CallSetBigBlind(_stateMachine.CurrentBigBlind);
        _stateMachine.DealTheCards(_stateMachine.MaxPocketCardsPlayersCanHave);
        CheckSwitchState();
    }
    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        Debug.Log("Deler Pre-Flop Exit");
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
}
