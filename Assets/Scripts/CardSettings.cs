using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSettings : MonoBehaviour
{
    // Enum for card ranks
    public enum Rank
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
        Jack, Queen, King, Ace
    }
    // Enum for card suits
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }
    // Enum for hand ranks
    public enum HandRank
    {
        HighCard = 1,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    public enum MaxSumOfRanks
    {
        HighCard = 14,
        Pair = 28,
        TwoPair = 54,
        ThreeOfAKind = 42,
        Straight = 60,
        Flush = 50,
        FullHouse = 68,
        FourOfAKind = 56,
        StraightFlush = 55,
        RoyalFlush = 60
    }
}
