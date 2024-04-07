using static CardSettings;
public interface IPlayer
{
    (HandRank fullHandRank, int fullHandSumOfRanks, int highCardRank, int seatId) SendHand();
}
