using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIBigBlindState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIBigBlindState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Big Blind Enter");
        int betAmount = SharedData.MinimumBet;
        _stateMachine.CurrentBet += betAmount;
        _stateMachine.TotalMoney -= betAmount;
        GameEvents.CallPlayerFinishedTurn(betAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
        _stateMachine.SeatUI.UpdateBetText(_stateMachine.CurrentBet, _stateMachine.IsAllIn);
        _stateMachine.SeatUI.UpdateTotalMoneyText(_stateMachine.TotalMoney);
        SwitchState(_stateFactory.IdleState);
    }
    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Big Blind Exit");
        _stateMachine.IsMyTurn = false;
        _stateMachine.IsBigBlind = false;
    }
    protected override void CheckSwitchState()
    {

    }
}
