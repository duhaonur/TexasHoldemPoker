using System.Collections.Generic;
using UnityEngine;
using static CardSettings;

public static class PlayerAIMoveDecision
{
    private static int _lowestHandRank = (int)HandRank.HighCard;
    private static int _highestHandRank = (int)HandRank.RoyalFlush;

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
    public static float HandRatio(List<Card> hand, float handRankWeight, string name)
    {
        // Evaluate the hand and get the hand rank and sum of ranks
        var evaluationResult = HandEvaluation.EvaluateHand(hand);
        var handRank = evaluationResult.handRank;
        var sumOfRanks = evaluationResult.sumOfRanks;
        Debug.Log($"Player{name}-Hand Rank: {handRank} Sum Of Ranks: {sumOfRanks} Max Possible Sum Of Ranks: {MaxPossibleSumOfRanks(handRank)} HandRatio");
        // Calculate ratios
        float handRankRatio = (int)handRank / _highestHandRank;
        float sumOfRanksRatio = Mathf.Clamp01(sumOfRanks / MaxPossibleSumOfRanks(handRank));

        // Calculate weighted sum
        float ratioSum = handRankRatio + sumOfRanksRatio;

        float weight = ratioSum / 2 * handRankWeight;
        Debug.Log($"Player{name}-Hand Rank Ratio {handRankRatio} Sum Of Ranks Ratio {sumOfRanksRatio} Ratio Sum {ratioSum} Weight {weight} HandRatio");
        return weight;
    }
    public static float CommunityCardsRatio(List<Card> communityCards, float communityCardWeight, string name)
    {
        var evaluationResult = HandEvaluation.EvaluateHand(communityCards);
        var handRank = evaluationResult.handRank;
        var sumOfRanks = evaluationResult.sumOfRanks;
        Debug.Log($"Player{name}-Hand Rank: {handRank} Sum Of Ranks: {sumOfRanks} Max Possible Sum Of Ranks: {MaxPossibleSumOfRanks(handRank)} CommunityCardRatio");
        // Calculate ratios
        float handRankRatio = (int)handRank / _highestHandRank;
        float sumOfRanksRatio = Mathf.Clamp01(sumOfRanks / MaxPossibleSumOfRanks(handRank));

        // Calculate weighted sum
        float ratioSum = handRankRatio + sumOfRanksRatio;

        float weight = ratioSum / 2 * communityCardWeight;
        Debug.Log($"Player{name}-Hand Rank Ratio {handRankRatio} Sum Of Ranks Ratio {sumOfRanksRatio} Ratio Sum {ratioSum} Weight {weight} CommunityCardRatio");
        return weight;
    }
    public static float FullHandRatio(List<Card> fullHand, float fullhandWeight, string name)
    {
        var evaluationResult = HandEvaluation.EvaluateHand(fullHand);
        var handRank = evaluationResult.handRank;
        var sumOfRanks = evaluationResult.sumOfRanks;
        Debug.Log($"Player{name}-Hand Rank: {handRank} Sum Of Ranks: {sumOfRanks} Max Possible Sum Of Ranks: {MaxPossibleSumOfRanks(handRank)}FullHandRatio");
        // Calculate ratios
        float handRankRatio = (int)handRank / _highestHandRank;
        float sumOfRanksRatio = Mathf.Clamp01(sumOfRanks / MaxPossibleSumOfRanks(handRank));

        // Calculate weighted sum
        float ratioSum = handRankRatio + sumOfRanksRatio;
        float weight = ratioSum / 2 * fullhandWeight;
        Debug.Log($"Player{name}-Hand Rank Ratio {handRankRatio} Sum Of Ranks Ratio {sumOfRanksRatio} Ratio Sum {ratioSum} Weight {weight} FullHandRatio");

        return weight;
    }
    public static float PotWeight(float totalMoney, float currentBet, float highestBet, float potSize, float potWeight)
    {
        // Calculate the ratio for required chips to call
        float callRatio = Mathf.Clamp01(currentBet / highestBet);

        // Calculate the ratio for pot size
        float potRatio = Mathf.Clamp01(potSize / highestBet);

        // Calculate the ratio for available chips relative to the highest bet
        float chipRatio = Mathf.Clamp01(totalMoney / highestBet);

        // Calculate the pot weight as a combination of call ratio, pot ratio, and chip ratio
        float potRatioSum = callRatio + potRatio + chipRatio;

        float potWeightedSum = potRatioSum / 3 * potWeight;

        Debug.Log($"Pot Weight: {potWeightedSum}");

        return potWeightedSum;
    }
}
