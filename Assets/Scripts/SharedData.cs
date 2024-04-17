using System.Collections.Generic;

// Static class to share data across the game
public static class SharedData
{
    // List to store the deck of cards
    public static List<Card> Deck;

    // Properties to store game-related data
    public static int MinimumBet { get; set; }
    public static int HighestBet { get; set; }
    public static int BigBlindBet { get; set; }
    public static int SmallBlindBet { get; set; }
    public static int Pot { get; set; }

    // Flags to track if blinds have been played
    public static bool IsSmallBlindPlayed { get; set; }
    public static bool IsBigBlindPlayed { get; set; }

    // Method to set initial values of game-related data
    public static void SetInitialValues()
    {
        HighestBet = 0;
        BigBlindBet = 0;
        SmallBlindBet = 0;
        Pot = 0;
        IsSmallBlindPlayed = false;
        IsBigBlindPlayed = false;
    }
}
