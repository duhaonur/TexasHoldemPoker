using System;
using System.Collections.Generic;
using System.Linq;
using static CardSettings;

// Static class to evaluate hands
public static class HandEvaluation
{
    // Method to evaluate the hand and determine its rank
    public static (HandRank handRank, int sumOfRanks) EvaluateHand(IEnumerable<Card> cards)
    {
        // Check each possible hand rank in decreasing order of strength
        var royalFlushResult = IsRoyalFlush(cards);
        if (royalFlushResult.exists)
            return (HandRank.RoyalFlush, royalFlushResult.sumOfRanks);

        var straightFlushResult = IsStraightFlush(cards);
        if (straightFlushResult.exists)
            return (HandRank.StraightFlush, straightFlushResult.sumOfRanks);

        var fourOfAKindResult = IsFourOfAKind(cards);
        if (fourOfAKindResult.exists)
            return (HandRank.FourOfAKind, fourOfAKindResult.sumOfRanks);

        var fullHouseResult = IsFullHouse(cards);
        if (fullHouseResult.exists)
            return (HandRank.FullHouse, fullHouseResult.sumOfRanks);

        var flushResult = IsFlush(cards);
        if (flushResult.exists)
            return (HandRank.Flush, flushResult.sumOfRanks);

        var straightResult = IsStraight(cards);
        if (straightResult.exists)
            return (HandRank.Straight, straightResult.sumOfRanks);

        var threeOfAKindResult = IsThreeOfAKind(cards);
        if (threeOfAKindResult.exists)
            return (HandRank.ThreeOfAKind, threeOfAKindResult.sumOfRanks);

        var twoPairResult = IsTwoPair(cards);
        if (twoPairResult.exists)
            return (HandRank.TwoPair, twoPairResult.sumOfRanks);

        var pairResult = IsPair(cards);
        if (pairResult.exists)
            return (HandRank.Pair, pairResult.sumOfRanks);

        var highCardResult = IsHighCard(cards);
        return (HandRank.HighCard, highCardResult.sumOfRanks);
    }

    // Method to check if the hand is Royal Flush
    public static (bool exists, int sumOfRanks) IsRoyalFlush(IEnumerable<Card> cards)
    {
        // Royal Flush: A, K, Q, J, 10 of the same suit
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            if (cards.Any(c => c.CardRank == Rank.Ace && c.CardSuit == suit) &&
                cards.Any(c => c.CardRank == Rank.King && c.CardSuit == suit) &&
                cards.Any(c => c.CardRank == Rank.Queen && c.CardSuit == suit) &&
                cards.Any(c => c.CardRank == Rank.Jack && c.CardSuit == suit) &&
                cards.Any(c => c.CardRank == Rank.Ten && c.CardSuit == suit))
            {
                int sumOfRanks = (int)Rank.Ace + (int)Rank.King + (int)Rank.Queen + (int)Rank.Jack + (int)Rank.Ten;
                return (true, sumOfRanks);
            }
        }
        return (false, 0);
    }

    // Method to check if the hand is Straight Flush
    public static (bool exists, int sumOfRanks) IsStraightFlush(IEnumerable<Card> cards)
    {
        // Straight Flush: Five consecutive cards of the same suit
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            for (int i = 2; i <= (int)Rank.Ace - 3; i++)
            {
                bool straightFlushFound = true;
                int sumOfRanks = 0;
                for (int j = i; j < i + 5; j++)
                {
                    // Adjust the value of Ace when it's considered as the lowest card (A-2-3-4-5)
                    Rank rankToCheck = (Rank)(j == (int)Rank.Ace ? 1 : j);
                    if (!cards.Any(c => c.CardRank == rankToCheck && c.CardSuit == suit))
                    {
                        straightFlushFound = false;
                        break;
                    }
                    sumOfRanks += (int)rankToCheck;
                }
                if (straightFlushFound)
                {
                    return (true, sumOfRanks);
                }
            }
        }
        return (false, 0);
    }

    // Method to check if the hand is Four of a Kind
    public static (bool exists, int sumOfRanks) IsFourOfAKind(IEnumerable<Card> cards)
    {
        // Four of a Kind: Four cards of the same rank
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            if (cards.Count(c => c.CardRank == rank) == 4)
            {
                int sumOfRanks = (int)rank * 4;
                return (true, sumOfRanks);
            }
        }
        return (false, 0);
    }

    // Method to check if the hand is Full House
    public static (bool exists, int sumOfRanks) IsFullHouse(IEnumerable<Card> cards)
    {
        var groupedCards = cards.GroupBy(c => c.CardRank);
        var threeOfAKind = groupedCards.FirstOrDefault(g => g.Count() == 3);
        var pair = groupedCards.FirstOrDefault(g => g.Count() == 2);

        if (threeOfAKind != null && pair != null && threeOfAKind.Key != pair.Key)
        {
            int sumOfRanks = (int)threeOfAKind.Key * 3 + (int)pair.Key * 2; // Sum of ranks for a full house
            return (true, sumOfRanks);
        }

        return (false, 0);
    }

    // Method to check if the hand is Flush
    public static (bool exists, int sumOfRanks) IsFlush(IEnumerable<Card> cards)
    {
        // Flush: All cards of the same suit
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            if (cards.Count(c => c.CardSuit == suit) >= 5)
            {
                // Calculate the sum of ranks for the flush
                int sumOfRanks = cards.Where(c => c.CardSuit == suit)
                                      .OrderByDescending(c => c.CardRank)
                                      .Take(5) // Take the top 5 cards of the same suit
                                      .Sum(c => (int)c.CardRank);
                return (true, sumOfRanks);
            }
        }
        return (false, 0);
    }

    // Method to check if the hand is Straight
    public static (bool exists, int sumOfRanks) IsStraight(IEnumerable<Card> cards)
    {
        // Straight: Five consecutive cards of any suit
        for (int i = 2; i <= (int)Rank.Ace - 3; i++)
        {
            bool straightFound = true;
            int sumOfRanks = 0;
            for (int j = i; j < i + 5; j++)
            {
                // Adjust the value of Ace when it's considered as the lowest card (A-2-3-4-5)
                Rank rankToCheck = (Rank)(j == (int)Rank.Ace ? 1 : j);
                if (!cards.Any(c => c.CardRank == rankToCheck))
                {
                    straightFound = false;
                    break;
                }
                sumOfRanks += (int)rankToCheck;
            }
            if (straightFound)
            {
                return (true, sumOfRanks);
            }
        }
        return (false, 0);
    }

    // Method to check if the hand is Three of a Kind
    public static (bool exists, int sumOfRanks) IsThreeOfAKind(IEnumerable<Card> cards)
    {
        // Three of a Kind: Three cards of the same rank
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            if (cards.Count(c => c.CardRank == rank) == 3)
            {
                int sumOfRanks = (int)rank * 3;
                return (true, sumOfRanks);
            }
        }
        return (false, 0);
    }

    // Method to check if the hand is Two Pair
    public static (bool exists, int sumOfRanks) IsTwoPair(IEnumerable<Card> cards)
    {
        // Two Pair: Two cards of one rank and two cards of another rank
        Rank firstPairRank = 0;
        Rank secondPairRank = 0;
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            if (cards.Count(c => c.CardRank == rank) == 2)
            {
                if (firstPairRank == 0)
                {
                    firstPairRank = rank;
                }
                else
                {
                    secondPairRank = rank;
                }
            }
        }
        if (firstPairRank != 0 && secondPairRank != 0)
        {
            int sumOfRanks = (int)firstPairRank * 2 + (int)secondPairRank * 2; // Sum of ranks for two pairs
            return (true, sumOfRanks);
        }
        return (false, 0);
    }

    // Method to check if the hand is a Pair
    public static (bool exists, int sumOfRanks) IsPair(IEnumerable<Card> cards)
    {
        // Pair: Two cards of the same rank
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            if (cards.Count(c => c.CardRank == rank) == 2)
            {
                int sumOfRanks = (int)rank * 2;
                return (true, sumOfRanks);
            }
        }
        return (false, 0);
    }

    // Method to check if the hand is a High Card
    public static (bool exists, int sumOfRanks) IsHighCard(IEnumerable<Card> cards)
    {
        // High Card: Highest card in the hand
        var sortedCards = cards.OrderByDescending(c => c.CardRank);
        int sumOfRanks = (int)sortedCards.First().CardRank;
        return (true, sumOfRanks);
    }
}
