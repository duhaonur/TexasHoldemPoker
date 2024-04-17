using UnityEngine;

public class PlayerAIRiverState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIRiverState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    // Method called when entering the river state
    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Enter");

        // Evaluate hand strength and game dynamics
        var predictedHand = PlayerAIMoveDecision.PredictHand(_stateMachine.HoleHand, _stateMachine.CommunityCards, 100);
        var handrank = PlayerAIMoveDecision.SetHighestHandRank(predictedHand);
        Debug.Log($"{_stateMachine.gameObject.name}HandRank:{handrank}");

        float handRatio = PlayerAIMoveDecision.HoleHand(_stateMachine.HoleHand, _stateMachine.WeightSettings.RiverHandWeight, _stateMachine.gameObject.name);
        float fullHandRatio = PlayerAIMoveDecision.FullHand(_stateMachine.FullHand, _stateMachine.WeightSettings.RiverFullHandWeight, _stateMachine.gameObject.name);
        float potRatio = PlayerAIMoveDecision.PotWeight(_stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet, SharedData.Pot, _stateMachine.WeightSettings.RiverPotWeight, PlayerAIMoveDecision.GetHandStrength(_stateMachine.FullHand));

        // Calculate the weighted sum of different factors
        float weightSum = handRatio + fullHandRatio + potRatio;

        Debug.Log($"{_stateMachine.gameObject.name}-Flop HandRatio:{handRatio} FullHandRatio:{fullHandRatio} PotRatio:{potRatio} WholeWeight:{weightSum}");

        // Dynamically adjust thresholds based on weighted sum and game dynamics
        if (weightSum >= _stateMachine.WeightSettings.RiverRaiseThreshold)
        {
            Raise(fullHandRatio, _stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet);
        }
        else if (weightSum >= _stateMachine.WeightSettings.RiverCallThreshold)
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

    // Method called during state update (not used in this case)
    protected override void OnUpdate()
    {
    }

    // Method called when exiting the river state
    protected override void OnExit()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Exit");
        _stateMachine.IsMyTurn = false;
    }

    // Check if state should transition to another state (always transitions to IdleState)
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }

    // Raise method for the river state
    private void Raise(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI River Raise");

        float raiseAmount = CalculateRaiseAmount(handStrength, totalMoney, currentBet, highestBet);
        _stateMachine.CurrentBet += (int)raiseAmount;
        _stateMachine.TotalMoney -= (int)raiseAmount;
        _stateMachine.SeatUI.ChangeInformationText("Raise");
        GameEvents.CallPlayerFinishedTurn((int)raiseAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
    }

    // Call or Check method for the river state
    private void CallOrCheck()
    {
        int callAmount = 0;

        if (SharedData.HighestBet == _stateMachine.CurrentBet)
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI River Check");
            _stateMachine.SeatUI.ChangeInformationText("Check");
        }
        else
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI River Call");
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

    // Fold method for the river state
    private void Fold()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI River Fold");
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
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.RiverRaiseAggressiveness;

            // Ensure the raise amount is at least the minimum bet allowed
            raiseAmount = Mathf.Max(raiseAmount, SharedData.MinimumBet);
        }
        else
        {
            // If someone raised before, consider hand strength and adjust raise amount based on pot, highest bet, and total bet made so far
            float additionalBet = highestBet - currentBet;

            // Adjust the raise amount based on hand strength, pot size, and additional bet
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.RiverRaiseAggressiveness + additionalBet;

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
