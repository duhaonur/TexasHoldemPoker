using System;
using System.Collections.Generic;
using UnityEngine;

public static class SeatEvents
{
    public static Action<int, Seat> OnSeatCreated;
    public static void CallSeatCreated(int seatId, Seat seat) { OnSeatCreated?.Invoke(seatId, seat); }

    public static Action<GameObject, int> OnSeatSelected;
    public static void CallSeatSelected(GameObject player, int seatID) { OnSeatSelected?.Invoke(player, seatID); }
    public static Action<int, Seat> OnGetSeatId;
    public static void CallGetSeatId(int seatID, Seat seat) { OnGetSeatId?.Invoke(seatID, seat); }
    public static Action<int> OnUpdateBetText;
    public static void CallUpdateBetText(int amount) { OnUpdateBetText?.Invoke(amount); }
    public static Action<int> OnUpdateMoneyText;
    public static void CallUpdateMoneyText(int amount) { OnUpdateMoneyText?.Invoke(amount); }
}
