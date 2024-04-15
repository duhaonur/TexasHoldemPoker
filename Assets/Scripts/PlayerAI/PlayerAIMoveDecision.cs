using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using static CardSettings;

public static class PlayerAIMoveDecision
{
    private static int _lowestHandRank = (int)HandRank.HighCard;
    private static float _highestHandRank = (float)HandRank.RoyalFlush;
    private static float _highestCommunityRank = (float)HandRank.HighCard;
    private static System.Random random = new System.Random();
    private static float MaxPossibleSumOfRanks(HandRank handRank)
    {
        if (handRank == HandRank.RoyalFlush) return (float)MaxSumOfRanks.RoyalFlush;
        if (handRank == HandRank.StraightFlush) return (float)MaxSumOfRanks.StraightFlush;
        if (handRank == HandRank.FourOfAKind) return (float)MaxSumOfRanks.FourOfAKind;
        if (handRank == HandRank.FullHouse) return (float)MaxSumOfRanks.FullHouse;
        if (handRank == HandRank.Flush) return (float)MaxSumOfRanks.Flush;
        if (handRank == HandRank.Straight) return (float)MaxSumOfRanks.Straight;
        if (handRank == HandRank.ThreeOfAKind) return (float)MaxSumOfRanks.ThreeOfAKind;
        if (handRank == HandRank.TwoPair) return (float)MaxSumOfRanks.TwoPair;
        if (handRank == HandRank.Pair) return (float)MaxSumOfRanks.Pair;

        return (float)MaxSumOfRanks.HighCard;
    }
    public static float HoleHand(List<Card> hand, float handRankWeight, string name)
    {
        // Evaluate the hand and get the hand rank and sum of ranks
        var evaluationResult = HandEvaluation.EvaluateHand(hand);
        var handRank = evaluationResult.handRank;
        var sumOfRanks = evaluationResult.sumOfRanks;
      //  Debug.Log($"Player{name}-Hand Rank: {handRank} Sum Of Ranks: {sumOfRanks} Max Possible Sum Of Ranks: {MaxPossibleSumOfRanks(handRank)} HandRatio");
        // Calculate ratios
        float handRankRatio = (float)handRank / (float)HandRank.Pair;
        //float sumOfRanksRatio = Mathf.Clamp01(sumOfRanks / MaxPossibleSumOfRanks(handRank));

        // Calculate weighted sum
        //float ratioSum = handRankRatio + sumOfRanksRatio;

        float weight = handRankRatio * handRankWeight;
       // Debug.Log($"Player{name}-Hand Rank Ratio {handRankRatio} Sum Of Ranks Ratio {sumOfRanksRatio} RatioSum:{ratioSum} Weight {weight} CommunityCardRatio");
        return weight;
    }
    public static float FullHand(List<Card> hand, float handRankWeight, string name)
    {
        // Evaluate the hand and get the hand rank and sum of ranks
        var evaluationResult = HandEvaluation.EvaluateHand(hand);
        var handRank = evaluationResult.handRank;
        var sumOfRanks = evaluationResult.sumOfRanks;
       // Debug.Log($"Player{name}-Hand Rank: {handRank} Sum Of Ranks: {sumOfRanks} Max Possible Sum Of Ranks: {MaxPossibleSumOfRanks(handRank)} HandRatio");
        // Calculate ratios
        float handRankRatio = (float)handRank / _highestCommunityRank;
        //float sumOfRanksRatio = Mathf.Clamp01(sumOfRanks / MaxPossibleSumOfRanks(handRank));

        // Calculate weighted sum
        //float ratioSum = handRankRatio + sumOfRanksRatio;

        float weight = handRankRatio * handRankWeight;
       // Debug.Log($"Player{name}-Hand Rank Ratio {handRankRatio} Sum Of Ranks Ratio {sumOfRanksRatio} RatioSum:{ratioSum} Weight {weight} CommunityCardRatio");
        return weight;
    }
    public static float PotWeight(float totalMoney, float currentBet, float highestBet, float potSize, float potWeight, float handStrength)
    {
        float chipsToCall = Mathf.Abs(currentBet - highestBet);

        float callRatio = 1 - Mathf.Clamp01(chipsToCall / highestBet);

        //callRatio = Mathf.Clamp01(callRatio);

        float totalMoneyRatio = 1 - Mathf.Clamp01(currentBet / totalMoney);

        //totalMoneyRatio = Mathf.Clamp01(totalMoneyRatio);

        float ratioSum = callRatio + totalMoneyRatio + handStrength;

        float potWeightedSum = ratioSum / 3 * potWeight;

        Debug.Log($"CallRatio: {callRatio} TotalMoneyRatio: {totalMoneyRatio} HandStrength: {handStrength} Pot Weight: {potWeightedSum}");

        return potWeightedSum;
    }
    public static float GetHandStrength(List<Card> hand)
    {
        // Evaluate the hand and get the hand rank and sum of ranks
        var evaluationResult = HandEvaluation.EvaluateHand(hand);
        var handRank = evaluationResult.handRank;

        return (float)handRank / _highestHandRank;
    }
    public static float FutureHandRatio(List<Card> aiHand, List<Card> communityCards, Dictionary<HandRank, float> predictedHand, int numSimulations, float futureHandWeight)
    {
        // Predict future hand strengths
        //Dictionary<HandRank, float> predictedFutureHandList = PredictFutureHandStrength(aiHand, communityCards, numSimulations);

        // Initialize variables to track the highest ratio and its associated hand rank
        float highestRatio = 0;
        HandRank handRank = HandRank.HighCard; // Initialize to a default value

        // Iterate over each key-value pair in the dictionary
        foreach (var kvp in predictedHand)
        {
            // Check if the current ratio is higher than the highest ratio found so far
            if (kvp.Value > highestRatio)
            {
                // Update the highest ratio and its associated hand rank
                highestRatio = kvp.Value;
                handRank = kvp.Key;
            }
        }

        float handRankRatio = (float)handRank / (float)HandRank.RoyalFlush;

        return handRankRatio * futureHandWeight;
    }
    public static HandRank SetHighestHandRank(Dictionary<HandRank,float> predictedHand)
    {
        HandRank handRank = HandRank.HighCard;

        foreach (var kvp in predictedHand)
        {
            if (kvp.Key > handRank)
            {
                handRank = kvp.Key;
            }
        }

        _highestHandRank = (float)handRank;
        return handRank;
    }
    public static HandRank SetBestPossibleCommunityCard(Dictionary<HandRank, float> predictedHand)
    {
        HandRank handRank = HandRank.HighCard;

        foreach (var kvp in predictedHand)
        {
            if (kvp.Key > handRank)
            {
                handRank = kvp.Key;
            }
        }

        _highestCommunityRank = (float)handRank;
        return handRank;
    }
    public static Dictionary<HandRank, float> PredictHand(List<Card> aiHand, List<Card> communityCards, int numSimulations)
    {
        Dictionary<HandRank, float> handStrengths = new Dictionary<HandRank, float>();
        int realSimulatedNumber = numSimulations;
        // Simulate future scenarios
        for (int i = 0; i < numSimulations; i++)
        {
            List<Card> simulatedCommunityCards = SimulateCommunityCards(communityCards, aiHand);
            List<Card> allCards = aiHand.Concat(simulatedCommunityCards).ToList();
            List<Card> aiFullHand = aiHand.Concat(communityCards).ToList();
            // Evaluate hand strength for the simulated scenario
            HandRank handRank = HandEvaluation.EvaluateHand(allCards).handRank;
            HandRank playerHandRank = HandEvaluation.EvaluateHand(aiFullHand).handRank;

            // If the player's hand already has this rank, skip this simulation
            if (playerHandRank >= handRank)
            {
                realSimulatedNumber--;
                continue;
            }

            // Update hand strength frequency
            if (handStrengths.ContainsKey(handRank))
            {
                handStrengths[handRank]++;
            }
            else
            {
                handStrengths.Add(handRank, 1);
            }
        }

        // Normalize frequencies to get probabilities
        foreach (var key in handStrengths.Keys.ToList())
        {
            handStrengths[key] /= realSimulatedNumber;
        }

        return handStrengths;
    }
    public static Dictionary<HandRank, float> PredictHand(List<Card> communityCards, int numSimulations)
    {
        Dictionary<HandRank, float> handStrengths = new Dictionary<HandRank, float>();
        int realSimulatedNumber = numSimulations;
        // Simulate future scenarios
        for (int i = 0; i < numSimulations; i++)
        {
            List<Card> simulatedCommunityCards = SimulateCommunityCards(communityCards);
            // Evaluate hand strength for the simulated scenario
            HandRank handRank = HandEvaluation.EvaluateHand(simulatedCommunityCards).handRank;

            // Update hand strength frequency
            if (handStrengths.ContainsKey(handRank))
            {
                handStrengths[handRank]++;
            }
            else
            {
                handStrengths.Add(handRank, 1);
            }
        }

        // Normalize frequencies to get probabilities
        foreach (var key in handStrengths.Keys.ToList())
        {
            handStrengths[key] /= realSimulatedNumber;
        }

        return handStrengths;
    }
    private static List<Card> SimulateCommunityCards(List<Card> currentCommunityCards, List<Card> playerHand)
    {
        List<Card> remainingCards = SharedData.Deck.Except(currentCommunityCards).Except(playerHand).ToList();
        int cardsNeeded = 5 - currentCommunityCards.Count;

        // Simulate remaining community cards
        List<Card> simulatedCommunityCards = new List<Card>(currentCommunityCards);
        for (int i = 0; i < cardsNeeded; i++)
        {
            int index = random.Next(0, remainingCards.Count);
            simulatedCommunityCards.Add(remainingCards[index]);
            remainingCards.RemoveAt(index);
        }

        return simulatedCommunityCards;
    }
    private static List<Card> SimulateCommunityCards(List<Card> currentCommunityCards)
    {
        List<Card> remainingCards = SharedData.Deck.Except(currentCommunityCards).ToList();
        int cardsNeeded = 5 - currentCommunityCards.Count;

        // Simulate remaining community cards
        List<Card> simulatedCommunityCards = new List<Card>(currentCommunityCards);
        for (int i = 0; i < cardsNeeded; i++)
        {
            int index = random.Next(0, remainingCards.Count);
            simulatedCommunityCards.Add(remainingCards[index]);
            remainingCards.RemoveAt(index);
        }

        return simulatedCommunityCards;
    }
}
