using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SeatController : MonoBehaviour
{
    public Dictionary<int, Seat> _seats = new Dictionary<int, Seat>();

    private void OnEnable()
    {
        SeatEvents.OnSeatCreated += AddSeat;
        SeatEvents.OnSeatSelected += Seated;
    }
    private void OnDisable()
    {
        SeatEvents.OnSeatCreated -= AddSeat;
        SeatEvents.OnSeatSelected -= Seated;
    }

    private void AddSeat(int seatId, Seat seat)
    {
        _seats.Add(seatId, seat);
    }
    private void Seated(GameObject obj, int seatID)
    {
        if(_seats.TryGetValue(seatID, out Seat seat))
        {
            seat.seatedObj = obj;
        }
    }
}
