using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static Action<Transform> OnPlayerSeated;
    public static void CallPlayerSeated(Transform player) { OnPlayerSeated?.Invoke(player); }
    public static Action<Stack<Card>> OnSentDeck;
    public static void CallSentDeck(Stack<Card> deck) { OnSentDeck?.Invoke(deck); }
    public static Action<Dictionary<int, Seat>> OnSendPlayerCount;
    public static void CallSendPlayerCount(Dictionary<int, Seat> seats) { OnSendPlayerCount?.Invoke(seats); }
    public static Action<int, Card> OnSendCardToHand;
    public static void CallSendCardToHand(int playerId, Card sentCard) { OnSendCardToHand?.Invoke(playerId, sentCard); }
    public static Action<int> OnSetSmallBlind;
    public static void CallSetSmallBlind(int playerId) { OnSetSmallBlind?.Invoke(playerId); }
    public static Action<int> OnSetBigBlind;
    public static void CallSetBigBlind(int playerId) { OnSetBigBlind?.Invoke(playerId); }
    public static Action<int, CurrentGameState> OnGivePlayerTheTurn;
    public static void CallGivePlayerTheTurn(int playerId, CurrentGameState state) { OnGivePlayerTheTurn?.Invoke(playerId, state); }
    public static Action OnStartGame;
    public static void CallStartGame() { OnStartGame?.Invoke(); }
    public static Action<Card> OnCommunityCard;
    public static void CallCommunityCard(Card card) { OnCommunityCard?.Invoke(card); }
    public static Action<int> OnPlayerFold;
    public static void CallPlayerFold(int id) { OnPlayerFold?.Invoke(id); }
    public static Action<bool> OnPlayerRaise;
    public static void CallPlayerRaise(bool value) { OnPlayerRaise?.Invoke(value); }
    public static Action<int, int, int> OnPlayerFinishedTurn;
    public static void CallPlayerFinishedTurn(int betAmount, int currentBet, int id) { OnPlayerFinishedTurn?.Invoke(betAmount, currentBet, id); }
    public static Action OnUpdatePotText;
    public static void CallUpdatePotText() { OnUpdatePotText?.Invoke(); }

}
