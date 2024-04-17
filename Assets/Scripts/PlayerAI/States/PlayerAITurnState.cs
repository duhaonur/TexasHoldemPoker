using UnityEngine;

public class PlayerAITurnState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAITurnState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        // Predict hand and evaluate game dynamics
        var predictedHand = PlayerAIMoveDecision.PredictHand(_stateMachine.HoleHand, _stateMachine.CommunityCards, 100);
        var handRank = PlayerAIMoveDecision.SetHighestHandRank(predictedHand);

        float futureHandWeight = PlayerAIMoveDecision.FutureHandRatio(_stateMachine.HoleHand, _stateMachine.CommunityCards, predictedHand, 100, _stateMachine.WeightSettings.TurnFutureHandWeight);
        float handRatio = PlayerAIMoveDecision.HoleHand(_stateMachine.HoleHand, _stateMachine.WeightSettings.TurnHandWeight, _stateMachine.gameObject.name);
        float fullHandRatio = PlayerAIMoveDecision.FullHand(_stateMachine.FullHand, _stateMachine.WeightSettings.TurnFullHandWeight, _stateMachine.gameObject.name);
        float potRatio = PlayerAIMoveDecision.PotWeight(_stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet, SharedData.Pot, _stateMachine.WeightSettings.TurnPotWeight, PlayerAIMoveDecision.GetHandStrength(_stateMachine.FullHand));

        // Calculate weighted sum
        float weightSum = handRatio + fullHandRatio + potRatio + futureHandWeight;

        // Dynamically adjust thresholds based on weighted sum and game dynamics
        if (weightSum >= _stateMachine.WeightSettings.TurnRaiseThreshold)
        {
            Raise(fullHandRatio, _stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet);
        }
        else if (weightSum >= _stateMachine.WeightSettings.TurnCallThreshold)
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

    protected override void OnUpdate()
    {
        // Nothing to do in update for this state
    }

    protected override void OnExit()
    {
        _stateMachine.IsMyTurn = false;
    }

    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }

    private void Raise(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        float raiseAmount = CalculateRaiseAmount(handStrength, totalMoney, currentBet, highestBet);
        _stateMachine.CurrentBet += (int)raiseAmount;
        _stateMachine.TotalMoney -= (int)raiseAmount;
        _stateMachine.SeatUI.ChangeInformationText("Raise");
        GameEvents.CallPlayerFinishedTurn((int)raiseAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
    }

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

    private void Fold()
    {
        _stateMachine.IsPlayerFolded = true;
        _stateMachine.SeatUI.ChangeInformationText("Fold");
        GameEvents.CallPlayerFold(_stateMachine.SeatId);
    }

    private float CalculateRaiseAmount(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        float raiseAmount = 0;

        if (highestBet == currentBet)
        {
            // If no one has bet before, calculate raise amount based on hand strength and pot size
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.TurnRaiseAggressiveness;

            // Ensure the raise amount is at least the minimum bet allowed
            raiseAmount = Mathf.Max(raiseAmount, SharedData.MinimumBet);
        }
        else
        {
            // If someone raised before, consider hand strength and adjust raise amount based on pot, highest bet, and total bet made so far
            float additionalBet = highestBet - currentBet;

            // Adjust the raise amount based on hand strength, pot size, and additional bet
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.TurnRaiseAggressiveness + additionalBet;

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
