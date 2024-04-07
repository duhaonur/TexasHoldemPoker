using System.ComponentModel;
using UnityEngine;

public class PlayerAIPreFlopState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIPreFlopState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Enter");

        float handRatio = PlayerAIMoveDecision.HandRatio(_stateMachine.HoleHand, _stateMachine.WeightSettings.PreFlopHandWeight, _stateMachine.gameObject.name);
        float potRatio = PlayerAIMoveDecision.PotWeight(_stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet, SharedData.Pot, _stateMachine.WeightSettings.PreFlopPotWeight);

        //float weightSum = (handRatio * _stateMachine.HandWeight) + (potRatio * _stateMachine.PotWeight);
        float weightSum = handRatio + potRatio;
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

        _stateMachine.SeatUI.UpdateBetText(_stateMachine.CurrentBet);
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
        if (SharedData.HighestBet == _stateMachine.CurrentBet)
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Check");
        }
        else
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PlayerAI Pre-Flop Call");

            int callAmount = Mathf.Max(SharedData.HighestBet - _stateMachine.CurrentBet, 0);
            _stateMachine.CurrentBet += callAmount;
            _stateMachine.TotalMoney -= callAmount;
            GameEvents.CallPlayerFinishedTurn(callAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
        }
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

        if (SharedData.HighestBet == _stateMachine.CurrentBet)
        {
            // If no one has bet before, calculate raise amount based on hand strength and pot size
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.MinRaiseFractionOfTotalMoney;

            // Ensure the raise amount is at least the minimum bet allowed
            raiseAmount = Mathf.Max(raiseAmount, SharedData.MinimumBet);
        }
        else
        {
            // If someone raised before, consider hand strength and adjust raise amount based on pot, highest bet, and total bet made so far
            float additionalBet = highestBet - currentBet;

            // Adjust the raise amount based on hand strength, pot size, and additional bet
            raiseAmount = handStrength * SharedData.Pot * _stateMachine.MinRaiseFractionOfTotalMoney + additionalBet;

            // Ensure the raise amount does not exceed the available funds
            raiseAmount = Mathf.Min(raiseAmount, totalMoney);
        }

        return raiseAmount;
    }
}
