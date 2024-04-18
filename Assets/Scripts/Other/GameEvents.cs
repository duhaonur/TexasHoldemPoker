using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    // Event to notify when a player is seated
    public static Action<Transform> OnPlayerSeated;
    public static void CallPlayerSeated(Transform player) { OnPlayerSeated?.Invoke(player); }

    // Event to send the deck to subscribers
    public static Action<Stack<Card>> OnSentDeck;
    public static void CallSentDeck(Stack<Card> deck) { OnSentDeck?.Invoke(deck); }

    // Event to send the player count to subscribers
    public static Action<Dictionary<int, Seat>> OnSendPlayerCount;
    public static void CallSendPlayerCount(Dictionary<int, Seat> seats) { OnSendPlayerCount?.Invoke(seats); }

    // Event to send a card to a player's hand
    public static Action<int, Card> OnSendCardToHand;
    public static void CallSendCardToHand(int playerId, Card sentCard) { OnSendCardToHand?.Invoke(playerId, sentCard); }

    // Event to set the small blind
    public static Action<int> OnSetSmallBlind;
    public static void CallSetSmallBlind(int playerId) { OnSetSmallBlind?.Invoke(playerId); }

    // Event to set the big blind
    public static Action<int> OnSetBigBlind;
    public static void CallSetBigBlind(int playerId) { OnSetBigBlind?.Invoke(playerId); }

    // Event to give a player the turn
    public static Action<int, CurrentGameState> OnGivePlayerTheTurn;
    public static void CallGivePlayerTheTurn(int playerId, CurrentGameState state) { OnGivePlayerTheTurn?.Invoke(playerId, state); }

    // Event to start the game
    public static Action OnStartGame;
    public static void CallStartGame() { OnStartGame?.Invoke(); }

    // Event to reset the game
    public static Action OnResetGame;
    public static void CallResetGame() { OnResetGame?.Invoke(); }

    // Event to notify when the game is ready
    public static Action OnGameIsReady;
    public static void CallGameIsReady() { OnGameIsReady?.Invoke(); }

    // Event to display a community card
    public static Action<Card> OnCommunityCard;
    public static void CallCommunityCard(Card card) { OnCommunityCard?.Invoke(card); }

    // Event to notify when a player folds
    public static Action<int> OnPlayerFold;
    public static void CallPlayerFold(int id) { OnPlayerFold?.Invoke(id); }

    // Event to notify when a player raises
    public static Action<bool> OnPlayerRaise;
    public static void CallPlayerRaise(bool value) { OnPlayerRaise?.Invoke(value); }

    // Event to notify when a player finishes their turn
    public static Action<int, int, int> OnPlayerFinishedTurn;
    public static void CallPlayerFinishedTurn(int betAmount, int currentBet, int id) { OnPlayerFinishedTurn?.Invoke(betAmount, currentBet, id); }

    // Event to update the pot text
    public static Action OnUpdatePotText;
    public static void CallUpdatePotText() { OnUpdatePotText?.Invoke(); }

    // Event to declare the winner
    public static Action<int, int> OnWinner;
    public static void CallWinner(int seatId, int wonChips) { OnWinner?.Invoke(seatId, wonChips); }

    // Event to display the winner text
    public static Action<int> OnDisplayWinnerText;
    public static void CallDisplayWinnerText(int seatId) { OnDisplayWinnerText?.Invoke(seatId); }
}
