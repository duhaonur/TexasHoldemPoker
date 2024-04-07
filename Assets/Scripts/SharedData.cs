public static class SharedData
{
    public static int MinimumBet { get; set; }
    public static int HighestBet { get; set; }
    public static int BigBlindBet { get; set; }
    public static int SmallBlindBet { get; set; }
    public static int Pot { get; set; }

    public static bool IsSmallBlindPlayed { get; set; }
    public static bool IsBigBlindPlayed { get; set; }
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
