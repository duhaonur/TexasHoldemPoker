using UnityEngine;

public class PlayerAISmallBlindState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAISmallBlindState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
    }

    protected override void OnUpdate()
    {
        // Calculate and place small blind bet
        int betAmount = SharedData.MinimumBet / 2;
        _stateMachine.CurrentBet += betAmount;
        _stateMachine.TotalMoney -= betAmount;
        GameEvents.CallPlayerFinishedTurn(betAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);

        // Update UI and transition to the next state
        _stateMachine.SeatUI.UpdateBetText(_stateMachine.CurrentBet, _stateMachine.IsAllIn);
        _stateMachine.SeatUI.ChangeInformationText("SB");
        _stateMachine.SeatUI.UpdateTotalMoneyText(_stateMachine.TotalMoney);
        SwitchState(_stateFactory.IdleState);
    }

    protected override void OnExit()
    {
        // Clean up state variables
        _stateMachine.IsMyTurn = false;
        _stateMachine.IsSmallBlind = false;
    }

    protected override void CheckSwitchState()
    {
        // No need to check for state transition in this state
    }
}
