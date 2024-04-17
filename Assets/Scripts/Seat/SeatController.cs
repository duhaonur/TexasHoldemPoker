using System.Collections.Generic;
using UnityEngine;

public class SeatController : MonoBehaviour
{
    // Dictionary to store seats
    public Dictionary<int, Seat> Seats = new Dictionary<int, Seat>();

    private void OnEnable()
    {
        // Subscribe to seat events
        SeatEvents.OnSeatCreated += AddSeat;
        SeatEvents.OnSeatSelected += Seated;
    }

    private void OnDisable()
    {
        // Unsubscribe from seat events
        SeatEvents.OnSeatCreated -= AddSeat;
        SeatEvents.OnSeatSelected -= Seated;
    }

    // Method to add a seat to the dictionary
    private void AddSeat(int seatId, Seat seat)
    {
        Seats.Add(seatId, seat);
    }

    // Method to handle when a player is seated in a seat
    private void Seated(GameObject obj, int seatID)
    {
        // Check if the seat exists in the dictionary
        if (Seats.TryGetValue(seatID, out Seat seat))
        {
            seat.seatedObj = obj;
        }
    }
}
