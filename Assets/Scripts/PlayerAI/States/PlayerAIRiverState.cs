using UnityEngine;
public class PlayerAIRiverState : State<PlayerAI, PlayerAIStateFactory>
{
    public PlayerAIRiverState(PlayerAI stateMachineController, PlayerAIStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }

    protected override void OnEnter()
    {
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Enter");
        float handRatio = PlayerAIMoveDecision.HandRatio(_stateMachine.HoleHand, _stateMachine.WeightSettings.RiverHandWeight, _stateMachine.gameObject.name);
        float communityCardRatio = PlayerAIMoveDecision.CommunityCardsRatio(_stateMachine.CommunityCards, _stateMachine.WeightSettings.RiverCommunityCardsWeight, _stateMachine.gameObject.name);
        float fullHandRatio = PlayerAIMoveDecision.FullHandRatio(_stateMachine.FullHand, _stateMachine.WeightSettings.RiverFullHandWeight, _stateMachine.gameObject.name);
        float potRatio = PlayerAIMoveDecision.PotWeight(_stateMachine.TotalMoney, _stateMachine.CurrentBet, SharedData.HighestBet, SharedData.Pot, _stateMachine.WeightSettings.RiverPotWeight);

        //float weightSum = Mathf.Clamp01((handRatio * _stateMachine.HandWeight) + (communityCardRatio * _stateMachine.CommunityCardsWeight) +
        //                               (fullHandRatio * _stateMachine.FullHandWeight) + (potRatio * _stateMachine.PotWeight));

        float weightSum = handRatio + communityCardRatio + fullHandRatio + potRatio;

        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Weight Sum: {weightSum}");
        if (weightSum >= _stateMachine.WeightSettings.RiverRaiseThreshold)
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Raise");
            float raiseMultiplier = Random.Range(1f, 2f);
            float raiseAmount = 0;
            if (SharedData.HighestBet == _stateMachine.CurrentBet)
            {
                raiseAmount = SharedData.Pot / 4;
            }
            else
            {
                raiseAmount = SharedData.HighestBet * raiseMultiplier;
            }
            _stateMachine.CurrentBet += (int)raiseAmount;
            _stateMachine.TotalMoney -= (int)raiseAmount;

            GameEvents.CallPlayerFinishedTurn((int)raiseAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
        }
        else if (weightSum >= _stateMachine.WeightSettings.RiverCallThreshold)
        {
            int callAmount = 0;

            if (SharedData.HighestBet == _stateMachine.CurrentBet)
            {
                Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Check");
            }
            else
            {
                Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Call");
                callAmount = (SharedData.HighestBet - _stateMachine.CurrentBet) == 0 ? SharedData.HighestBet : (SharedData.HighestBet - _stateMachine.CurrentBet);

            }
            _stateMachine.CurrentBet += callAmount;
            _stateMachine.TotalMoney -= callAmount;

            GameEvents.CallPlayerFinishedTurn(callAmount, _stateMachine.CurrentBet, _stateMachine.SeatId);
        }
        else
        {
            Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Fold");
            _stateMachine.IsPlayerFolded = true;
            GameEvents.CallPlayerFold(_stateMachine.SeatId);
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
        Debug.Log($"{_stateMachine.gameObject.name}-PLayerAI River Exit");
        _stateMachine.IsMyTurn = false;
    }
    protected override void CheckSwitchState()
    {
        SwitchState(_stateFactory.IdleState);
    }
}
