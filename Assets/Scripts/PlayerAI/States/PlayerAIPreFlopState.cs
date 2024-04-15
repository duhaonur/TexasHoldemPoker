using System.ComponentModel;
using UnityEngine;
using System.Collections.Generic;
public class PlayerAIPreFlopState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIPreFlopState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Enter");
        var predictedHand = PlayerAIMoveDecision.PredictHand(_stateMachine.HoleHand, _stateMachine.CommunityCards, 100);
        var handrank = PlayerAIMoveDecision.SetHighestHandRank(predictedHand);
        Debug.Log($"{_stateMachine.gameObject.name}HandRank:{handrank}");
        float futureHandWeight = PlayerAIMoveDecision.FutureHandRatio(_stateMachine.HoleHand, _stateMachine.CommunityCards, predictedHand, 100, _stateMachine.WeightSettings.PreFlopFutureHandWeight);
        float handRatio = PlayerAIMoveDecision.HoleHand(_stateMachine.HoleHand, _stateMachine.WeightSettings.PreFlopHandWeight, _stateMachine.gameObject.name);
        float potRatio = PlayerAIMoveDecision.PotWeight(_stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet, SharedData.Pot, _stateMachine.WeightSettings.PreFlopPotWeight, PlayerAIMoveDecision.GetHandStrength(_stateMachine.FullHand));

        _stateMachine.Ranks = new(predictedHand.Keys);
        _stateMachine.Something = new(predictedHand.Values);

        float weightSum = handRatio + potRatio + futureHandWeight;
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Weight Sum: {weightSum}");

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

        _stateMachine.SeatUI.UpdateBetText(_stateMachine.CurrentBet, _stateMachine.IsAllIn);
        _stateMachine.SeatUI.UpdateTotalMoneyText(_stateMachine.TotalMoney);
        CheckSwitchState();
    }
    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI Pre-Flop Exit");
        _stateMachine.IsMyTurn = false;
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
    private void Raise(float handStrength, float totalMoney, int currentBet, int highestBet)
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Raise");

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
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Check");
        }
        else
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Call");

            callAmount = Mathf.Max(SharedData.HighestBet - _stateMachine.CurrentBet, 0);
        }

        _stateMachine.CurrentBet += callAmount;
        _stateMachine.TotalMoney -= callAmount;

        GameEvents.CallPlayerFinishedTurn(callAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
    }

    private void Fold()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Fold");
        _stateMachine.IsPlayerFolded = true;
        GameEvents.CallPlayerFold(_stateMachine.SeatId);
    }

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
        }

        return raiseAmount;
    }
}
