public class PlayerAIBigBlindState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIBigBlindState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    // Method called when entering the big blind state
    protected override void OnEnter()
    {
        // Set the bet amount to the minimum bet and deduct it from total money
        int betAmount = SharedData.MinimumBet;
        _stateMachine.CurrentBet += betAmount;
        _stateMachine.TotalMoney -= betAmount;

        // Notify game events and update UI
        GameEvents.CallPlayerFinishedTurn(betAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
        _stateMachine.SeatUI.UpdateBetText(_stateMachine.CurrentBet, _stateMachine.IsAllIn);
        _stateMachine.SeatUI.ChangeInformationText("BB");
        _stateMachine.SeatUI.UpdateTotalMoneyText(_stateMachine.TotalMoney);

        // Transition to the idle state
        SwitchState(_stateFactory.IdleState);
    }

    // Method called during state update (not used in this case)
    protected override void OnUpdate()
    {
    }

    // Method called when exiting the big blind state
    protected override void OnExit()
    {
         // Reset flags
        _stateMachine.IsMyTurn = false;
        _stateMachine.IsBigBlind = false;
    }

    // Check if state should transition to another state (not used in this case)
    protected override void CheckSwitchState()
    {
    }
}
