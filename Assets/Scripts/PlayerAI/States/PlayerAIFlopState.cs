using System.Collections.Generic;
using UnityEngine;

public class PlayerAIFlopState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIFlopState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Flop Enter");
        var predictedHand = PlayerAIMoveDecision.PredictHand(_stateMachine.HoleHand, _stateMachine.CommunityCards, 100);
        var handrank = PlayerAIMoveDecision.SetHighestHandRank(predictedHand);
        Debug.Log($"{_stateMachine.gameObject.name}HandRank:{handrank}");
        float futureHandWeight = PlayerAIMoveDecision.FutureHandRatio(_stateMachine.HoleHand, _stateMachine.CommunityCards, predictedHand, 100, _stateMachine.WeightSettings.FlopFutureHandWeight);
        // Evaluate hand strength and game dynamics
        float handRatio = PlayerAIMoveDecision.HoleHand(_stateMachine.HoleHand, _stateMachine.WeightSettings.FlopHandWeight, _stateMachine.gameObject.name);
        //float communityCardRatio = PlayerAIMoveDecision.CommunityCardsRatio(_stateMachine.CommunityCards, _stateMachine.WeightSettings.FlopCommunityCardsWeight, _stateMachine.gameObject.name);
        float fullHandRatio = PlayerAIMoveDecision.FullHand(_stateMachine.FullHand, _stateMachine.WeightSettings.FlopFullHandWeight, _stateMachine.gameObject.name);
        float potRatio = PlayerAIMoveDecision.PotWeight(_stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet, SharedData.Pot, _stateMachine.WeightSettings.FlopPotWeight, PlayerAIMoveDecision.GetHandStrength(_stateMachine.FullHand));
        _stateMachine.Ranks = new(predictedHand.Keys);
        _stateMachine.Something = new(predictedHand.Values);
        //float weightSum = Mathf.Clamp01((handRatio * _stateMachine.HandWeight) + (communityCardRatio * _stateMachine.CommunityCardsWeight) +
        //                               (fullHandRatio * _stateMachine.FullHandWeight) + (potRatio * _stateMachine.PotWeight));

        float weightSum = handRatio + fullHandRatio + potRatio + futureHandWeight;

        Debug.Log($"{_stateMachine.gameObject.name}-Flop HandRatio:{handRatio}FullHandRatio:{fullHandRatio} PotRatio:{potRatio} FutureHandRatio:{futureHandWeight} WholeWeight:{weightSum}");

        // Dynamically adjust thresholds based on weighted sum and game dynamics
        if (weightSum >= _stateMachine.WeightSettings.FlopRaiseThreshold)
        {
            Raise(fullHandRatio, _stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet);
        }
        else if (weightSum >= _stateMachine.WeightSettings.FlopCallThreshold)
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

    }
    protected override void OnExit()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Flop Exit");
        _stateMachine.IsMyTurn = false;
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
    private void Raise(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Flop Raise");

        float raiseAmount = CalculateRaiseAmount(handStrength, totalMoney, currentBet, highestBet);
        _stateMachine.CurrentBet += (int)raiseAmount;
        _stateMachine.TotalMoney -= (int)raiseAmount;
        GameEvents.CallPlayerFinishedTurn((int)raiseAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
    }

    private void CallOrCheck()
    {
        int callAmount = 0;

        if (SharedData.HighestBet == _stateMachine.CurrentBet)
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Flop Check");
        }
        else
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Flop Call");

            callAmount = Mathf.Max(SharedData.HighestBet - _stateMachine.CurrentBet, 0);
        }

        _stateMachine.CurrentBet += callAmount;
        _stateMachine.TotalMoney -= callAmount;

        GameEvents.CallPlayerFinishedTurn(callAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
    }

    private void Fold()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Flop Fold");
        _stateMachine.IsPlayerFolded = true;
        GameEvents.CallPlayerFold(_stateMachine.SeatId);
    }

    private float CalculateRaiseAmount(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        float raiseAmount = 0;

        if (highestBet == currentBet)
        {
            // If no one has bet before, calculate raise amount based on hand strength and pot size
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.FlopRaiseAggressiveness;

            // Ensure the raise amount is at least the minimum bet allowed
            raiseAmount = Mathf.Max(raiseAmount, SharedData.MinimumBet);
        }
        else
        {
            // If someone raised before, consider hand strength and adjust raise amount based on pot, highest bet, and total bet made so far
            float additionalBet = highestBet - currentBet;

            // Adjust the raise amount based on hand strength, pot size, and additional bet
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.WeightSettings.FlopRaiseAggressiveness + additionalBet;

            // Ensure the raise amount does not exceed the available funds
            raiseAmount = Mathf.Min(raiseAmount, totalMoney);
        }

        if (raiseAmount >= _stateMachine.TotalMoney)
        {
            raiseAmount = _stateMachine.TotalMoney;
            _stateMachine.IsAllIn = true;
        }

        return raiseAmount;
    }
}
