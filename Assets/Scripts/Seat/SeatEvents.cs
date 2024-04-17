using System;
using UnityEngine;

public static class SeatEvents
{
    // Event for when a seat is created
    public static Action<int, Seat> OnSeatCreated;
    public static void CallSeatCreated(int seatId, Seat seat) { OnSeatCreated?.Invoke(seatId, seat); }

    // Event for when a seat is selected
    public static Action<GameObject, int> OnSeatSelected;
    public static void CallSeatSelected(GameObject player, int seatID) { OnSeatSelected?.Invoke(player, seatID); }

    // Event for getting the seat ID
    public static Action<int, Seat> OnGetSeatId;
    public static void CallGetSeatId(int seatID, Seat seat) { OnGetSeatId?.Invoke(seatID, seat); }

    // Event for updating the bet text
    public static Action<int> OnUpdateBetText;
    public static void CallUpdateBetText(int amount) { OnUpdateBetText?.Invoke(amount); }

    // Event for updating the money text
    public static Action<int> OnUpdateMoneyText;
    public static void CallUpdateMoneyText(int amount) { OnUpdateMoneyText?.Invoke(amount); }
}
