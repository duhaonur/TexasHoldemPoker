using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using static CardSettings;
public class DealerShowdownState : State<DealerAI, DealerStateFactory>
{
    public DealerShowdownState(DealerAI stateMachineController, DealerStateFactory stateFactory) : base(stateMachineController, stateFactory)
    {
    }
    protected override void OnEnter()
    {
        Debug.Log("Deler Showdown Enter");
        _stateMachine.GiveTurnToNextPlayer = false;
        _stateMachine.WaitForThePlayer = false;

        List<PlayerHand> playerHands = new List<PlayerHand>();
        List<PlayerHand> winners = new List<PlayerHand>();

        for (int i = 0; i < _stateMachine.Players.Count; i++)
        {
            var hand = _stateMachine.Players[i].seatedObj.GetComponent<IPlayer>().SendHand();
            PlayerHand playerHand = new PlayerHand();
            playerHand.FullHandRank = hand.fullHandRank;
            playerHand.FullHandSumOfRanks = hand.fullHandSumOfRanks;
            playerHand.HighCardRank = hand.highCardRank;
            playerHand.SeatId = hand.seatId;
            playerHands.Add(playerHand);
        }
        winners = DetermineWinner(playerHands);

        if(winners.Count > 1)
        {
            Debug.Log("Pot Split");
            for (int i = 0; i < winners.Count; i++)
            {
                Debug.Log($"Winner:{winners[i].SeatId}");
            }
        }
        else
        {
            Debug.Log($"Winner:{winners[0].SeatId}");
        }
    }
    protected override void OnUpdate()
    {
        CheckSwitchState();
    }
    protected override void OnExit()
    {
        Debug.Log("Deler Showdown Exit");
    }
    protected override void CheckSwitchState()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchState(_stateFactory.ResetState);
        }
    }
    // Compare function to compare two PlayerHand objects
    public int ComparePlayerHands(PlayerHand hand1, PlayerHand hand2)
    {
        // First, compare by full hand rank
        if (hand1.FullHandRank != hand2.FullHandRank)
        {
            return hand1.FullHandRank.CompareTo(hand2.FullHandRank);
        }

        // If the full hand ranks are the same, compare by the sum of ranks
        if (hand1.FullHandSumOfRanks != hand2.FullHandSumOfRanks)
        {
            return hand1.FullHandSumOfRanks.CompareTo(hand2.FullHandSumOfRanks);
        }

        // If the sum of ranks are the same, compare by high card rank
        return hand1.HighCardRank.CompareTo(hand2.HighCardRank);
    }
    // Main method to determine the winner and handle ties
    public List<PlayerHand> DetermineWinner(List<PlayerHand> playerHands)
    {
        // Sort the player hands based on the comparison function
        playerHands.Sort(ComparePlayerHands);

        // Determine the best hand
        PlayerHand bestHand = playerHands[playerHands.Count - 1];

        // Check for ties
        List<PlayerHand> winners = new List<PlayerHand>();
        foreach (PlayerHand hand in playerHands)
        {
            if (ComparePlayerHands(hand, bestHand) == 0)
            {
                winners.Add(hand);
            }
        }

        return winners;
    }
}
public class PlayerHand
{
    public HandRank FullHandRank { get; set; }
    public int FullHandSumOfRanks { get; set; }
    public int HighCardRank { get; set; }
    public int SeatId { get; set; }
}
