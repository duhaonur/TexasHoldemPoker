using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardSettings;

public static class PlayerAIMoveDecision
{
    private static int _lowestHandRank = (int)HandRank.HighCard;
    private static float _highestHandRank = (float)HandRank.RoyalFlush;
    private static float _highestCommunityRank = (float)HandRank.HighCard;
    private static System.Random random = new System.Random();

    // Method to calculate the maximum possible sum of ranks based on the hand rank
    private static float MaxPossibleSumOfRanks(HandRank handRank)
    {
        // Mapping hand ranks to their maximum sum of ranks
        switch (handRank)
        {
            case HandRank.RoyalFlush:
                return (float)MaxSumOfRanks.RoyalFlush;
            case HandRank.StraightFlush:
                return (float)MaxSumOfRanks.StraightFlush;
            case HandRank.FourOfAKind:
                return (float)MaxSumOfRanks.FourOfAKind;
            case HandRank.FullHouse:
                return (float)MaxSumOfRanks.FullHouse;
            case HandRank.Flush:
                return (float)MaxSumOfRanks.Flush;
            case HandRank.Straight:
                return (float)MaxSumOfRanks.Straight;
            case HandRank.ThreeOfAKind:
                return (float)MaxSumOfRanks.ThreeOfAKind;
            case HandRank.TwoPair:
                return (float)MaxSumOfRanks.TwoPair;
            case HandRank.Pair:
                return (float)MaxSumOfRanks.Pair;
            default:
                return (float)MaxSumOfRanks.HighCard;
        }
    }

    // Method to evaluate the strength of the hole hand
    public static float HoleHand(List<Card> hand, float handRankWeight, string name)
    {
        var evaluationResult = HandEvaluation.EvaluateHand(hand);
        var handRank = evaluationResult.handRank;
        var sumOfRanks = evaluationResult.sumOfRanks;

        // Calculate the hand rank ratio based on the predefined weights
        float handRankRatio = (float)handRank / (float)HandRank.Pair;

        // Calculate the weighted sum
        float weight = handRankRatio * handRankWeight;

        return weight;
    }

    // Method to evaluate the strength of the full hand (hole hand + community cards)
    public static float FullHand(List<Card> hand, float handRankWeight, string name)
    {
        var evaluationResult = HandEvaluation.EvaluateHand(hand);
        var handRank = evaluationResult.handRank;
        var sumOfRanks = evaluationResult.sumOfRanks;

        // Calculate the hand rank ratio based on the predefined weights
        float handRankRatio = (float)handRank / _highestCommunityRank;

        // Calculate the weighted sum
        float weight = handRankRatio * handRankWeight;

        return weight;
    }

    // Method to calculate the weight based on pot size and other factors
    public static float PotWeight(float totalMoney, float currentBet, float highestBet, float potSize, float potWeight, float handStrength)
    {
        float chipsToCall = Mathf.Abs(currentBet - highestBet);
        float callRatio = 1 - Mathf.Clamp01(chipsToCall / highestBet);
        float totalMoneyRatio = 1 - Mathf.Clamp01(currentBet / totalMoney);
        float ratioSum = callRatio + totalMoneyRatio + handStrength;
        float potWeightedSum = ratioSum / 3 * potWeight;

        Debug.Log($"CallRatio: {callRatio} TotalMoneyRatio: {totalMoneyRatio} HandStrength: {handStrength} Pot Weight: {potWeightedSum}");

        return potWeightedSum;
    }

    // Method to get the hand strength based on the hand rank
    public static float GetHandStrength(List<Card> hand)
    {
        var evaluationResult = HandEvaluation.EvaluateHand(hand);
        var handRank = evaluationResult.handRank;

        return (float)handRank / _highestHandRank;
    }

    // Method to predict the future hand strength
    public static float FutureHandRatio(List<Card> aiHand, List<Card> communityCards, Dictionary<HandRank, float> predictedHand, int numSimulations, float futureHandWeight)
    {
        float highestRatio = 0;
        HandRank handRank = HandRank.HighCard;

        foreach (var kvp in predictedHand)
        {
            if (kvp.Value > highestRatio)
            {
                highestRatio = kvp.Value;
                handRank = kvp.Key;
            }
        }

        float handRankRatio = (float)handRank / (float)HandRank.RoyalFlush;

        return handRankRatio * futureHandWeight;
    }

    // Method to set the highest hand rank based on the predicted hand
    public static HandRank SetHighestHandRank(Dictionary<HandRank, float> predictedHand)
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

    // Method to set the highest community rank based on the predicted hand
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

    // Method to predict the hand based on the community cards and player's hand
    public static Dictionary<HandRank, float> PredictHand(List<Card> aiHand, List<Card> communityCards, int numSimulations)
    {
        Dictionary<HandRank, float> handStrengths = new Dictionary<HandRank, float>();
        int realSimulatedNumber = numSimulations;

        for (int i = 0; i < numSimulations; i++)
        {
            List<Card> simulatedCommunityCards = SimulateCommunityCards(communityCards, aiHand);
            List<Card> allCards = aiHand.Concat(simulatedCommunityCards).ToList();
            List<Card> aiFullHand = aiHand.Concat(communityCards).ToList();
            HandRank handRank = HandEvaluation.EvaluateHand(allCards).handRank;
            HandRank playerHandRank = HandEvaluation.EvaluateHand(aiFullHand).handRank;

            if (playerHandRank >= handRank)
            {
                realSimulatedNumber--;
                continue;
            }

            if (handStrengths.ContainsKey(handRank))
            {
                handStrengths[handRank]++;
            }
            else
            {
                handStrengths.Add(handRank, 1);
            }
        }

        foreach (var key in handStrengths.Keys.ToList())
        {
            handStrengths[key] /= realSimulatedNumber;
        }

        return handStrengths;
    }

    // Method to predict the hand based on the community cards
    public static Dictionary<HandRank, float> PredictHand(List<Card> communityCards, int numSimulations)
    {
        Dictionary<HandRank, float> handStrengths = new Dictionary<HandRank, float>();
        int realSimulatedNumber = numSimulations;

        for (int i = 0; i < numSimulations; i++)
        {
            List<Card> simulatedCommunityCards = SimulateCommunityCards(communityCards);
            HandRank handRank = HandEvaluation.EvaluateHand(simulatedCommunityCards).handRank;

            if (handStrengths.ContainsKey(handRank))
            {
                handStrengths[handRank]++;
            }
            else
            {
                handStrengths.Add(handRank, 1);
            }
        }

        foreach (var key in handStrengths.Keys.ToList())
        {
            handStrengths[key] /= realSimulatedNumber;
        }

        return handStrengths;
    }

    // Method to simulate community cards based on the current community cards and player's hand
    private static List<Card> SimulateCommunityCards(List<Card> currentCommunityCards, List<Card> playerHand)
    {
        List<Card> remainingCards = SharedData.Deck.Except(currentCommunityCards).Except(playerHand).ToList();
        int cardsNeeded = 5 - currentCommunityCards.Count;

        List<Card> simulatedCommunityCards = new List<Card>(currentCommunityCards);
        for (int i = 0; i < cardsNeeded; i++)
        {
            int index = random.Next(0, remainingCards.Count);
            simulatedCommunityCards.Add(remainingCards[index]);
            remainingCards.RemoveAt(index);
        }

        return simulatedCommunityCards;
    }

    // Method to simulate community cards based on the current community cards
    private static List<Card> SimulateCommunityCards(List<Card> currentCommunityCards)
    {
        List<Card> remainingCards = SharedData.Deck.Except(currentCommunityCards).ToList();
        int cardsNeeded = 5 - currentCommunityCards.Count;

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
