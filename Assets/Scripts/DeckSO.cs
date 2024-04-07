using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Deck", menuName = "CreateNewDeck")]
public class DeckSO : ScriptableObject
{
    //public List<string> SuitNames = new List<string>();
    //public List <string> CardNameOrder = new List<string>();
    //public List<int> RankOrder = new List<int>();
    // Enum for hand ranks
    public enum HandRank
    {
        HighCard,
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
}
