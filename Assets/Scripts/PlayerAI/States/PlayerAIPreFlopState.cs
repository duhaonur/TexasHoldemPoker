using UnityEngine;

public class PlayerAIPreFlopState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIPreFlopState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    // Method called when entering the pre-flop state
    protected override void OnEnter()
    {
        // Predict hand and evaluate hand rank
        var predictedHand = PlayerAIMoveDecision.PredictHand(_stateMachine.HoleHand, _stateMachine.CommunityCards, 100);
        var handrank = PlayerAIMoveDecision.SetHighestHandRank(predictedHand);

        // Calculate future hand weight, hand ratio, and pot ratio
        float futureHandWeight = PlayerAIMoveDecision.FutureHandRatio(_stateMachine.HoleHand, _stateMachine.CommunityCards, predictedHand, 100, _stateMachine.WeightSettings.PreFlopFutureHandWeight);
        float handRatio = PlayerAIMoveDecision.HoleHand(_stateMachine.HoleHand, _stateMachine.WeightSettings.PreFlopHandWeight, _stateMachine.gameObject.name);
        float potRatio = PlayerAIMoveDecision.PotWeight(_stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet, SharedData.Pot, _stateMachine.WeightSettings.PreFlopPotWeight, PlayerAIMoveDecision.GetHandStrength(_stateMachine.FullHand));

        // Calculate weighted sum of factors
        float weightSum = handRatio + potRatio + futureHandWeight;

        // Determine action based on weighted sum
        if (weightSum >= _stateMachine.WeightSettings.PreFlopRaiseThreshold)
        {
            Raise(handRatio, _stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet);
        }
        else if (weightSum >= _stateMachine.WeightSettings.PreFlopCallThreshold)
        {
            CallOrCheck();
        }
        else
        {
            Fold();
        }

        // Update UI and transition to the next state
        _stateMachine.SeatUI.UpdateBetText(_stateMachine.CurrentBet, _stateMachine.IsAllIn);
        _stateMachine.SeatUI.UpdateTotalMoneyText(_stateMachine.TotalMoney);
        CheckSwitchState();
    }

    // Method called during state update
    protected override void OnUpdate()
    {
    }

    // Method called when exiting the pre-flop state
    protected override void OnExit()
    {
        _stateMachine.IsMyTurn = false;
    }

    // Check if state should transition to another state (always transitions to IdleState)
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }

    // Raise method for the pre-flop state
    private void Raise(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        float raiseAmount = CalculateRaiseAmount(handStrength, totalMoney, currentBet, highestBet);
        _stateMachine.CurrentBet += (int)raiseAmount;
        _stateMachine.TotalMoney -= (int)raiseAmount;
        GameEvents.CallPlayerFinishedTurn((int)raiseAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
        _stateMachine.SeatUI.ChangeInformationText("Raise");
    }

    // Call or Check method for the pre-flop state
    private void CallOrCheck()
    {
        int callAmount = 0;

        if (SharedData.HighestBet == _stateMachine.CurrentBet)
        {
            _stateMachine.SeatUI.ChangeInformationText("Check");
        }
        else
        {
            _stateMachine.SeatUI.ChangeInformationText("Call");
            callAmount = Mathf.Max(SharedData.HighestBet - _stateMachine.CurrentBet, 0);
        }

        if (callAmount >= _stateMachine.TotalMoney)
        {
            callAmount = _stateMachine.TotalMoney;
            _stateMachine.IsAllIn = true;
            _stateMachine.Seat.isAllIn = _stateMachine.IsAllIn;
        }

        _stateMachine.CurrentBet += callAmount;
        _stateMachine.TotalMoney -= callAmount;

        GameEvents.CallPlayerFinishedTurn(callAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
    }

    // Fold method for the pre-flop state
    private void Fold()
    {
        _stateMachine.IsPlayerFolded = true;
        _stateMachine.SeatUI.ChangeInformationText("Fold");
        GameEvents.CallPlayerFold(_stateMachine.SeatId);
    }

    // Calculate the raise amount based on hand strength and game dynamics
    private float CalculateRaiseAmount(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        float raiseAmount = 0;

        if (highestBet == currentBet)
        {
            // If no one has bet before, calculate raise amount based on hand strength and pot size
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.PreFlopRaiseAggressiveness;

            // Ensure the raise amount is at least the minimum bet allowed
            raiseAmount = Mathf.Max(raiseAmount, SharedData.MinimumBet);
        }
        else
        {
            // If someone raised before, consider hand strength and adjust raise amount based on pot, highest bet, and total bet made so far
            float additionalBet = highestBet - currentBet;

            // Adjust the raise amount based on hand strength, pot size, and additional bet
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.PreFlopRaiseAggressiveness + additionalBet;

            // Ensure the raise amount does not exceed the available funds
            raiseAmount = Mathf.Min(raiseAmount, totalMoney);
        }

        if (raiseAmount >= _stateMachine.TotalMoney)
        {
            raiseAmount = _stateMachine.TotalMoney;
            _stateMachine.IsAllIn = true;
            _stateMachine.Seat.isAllIn = _stateMachine.IsAllIn;
        }

        return raiseAmount;
    }
}
