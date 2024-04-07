using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAISmallBlindState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAISmallBlindState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Small Blind Enter");
    }
    protected override void OnUpdate()
    {
        int betAmount = SharedData.MinimumBet / 2;
        _stateMachine.CurrentBet += betAmount;
        _stateMachine.TotalMoney -= betAmount;
        GameEvents.CallPlayerFinishedTurn(betAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
        _stateMachine.SeatUI.UpdateBetText(_stateMachine.CurrentBet);
        _stateMachine.SeatUI.UpdateTotalMoneyText(_stateMachine.TotalMoney);
        SwitchState(_stateFactory.IdleState);
    }
    protected override void OnExit()
    {
        _stateMachine.IsMyTurn = false;
        _stateMachine.IsSmallBlind = false;
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Small Blind Exit");
    }
    protected override void CheckSwitchState()
    {

    }
}
