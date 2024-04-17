using UnityEngine;

public class Seat : MonoBehaviour
{
    public SeatUI SeatUI; // Reference to the UI elements for this seat

    public RandomNameList NameList; // List of random names for the seat

    public Transform CameraLook; // Transform for the camera look position
    public Transform CameraFollow; // Transform for the camera follow position

    public GameObject seatedObj; // Reference to the object seated in the seat
    public Transform[] CardPositions; // Array of positions for cards in the seat

    public int SeatId; // ID of the seat

    public bool isAllIn = false; // Flag indicating if the player is all-in

    private void Start()
    {
        // Call the seat created event when the seat starts
        SeatEvents.CallSeatCreated(SeatId, this);
    }

    private void OnEnable()
    {
        // Subscribe to the event for displaying winner text
        GameEvents.OnDisplayWinnerText += DisplayWinner;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event for displaying winner text
        GameEvents.OnDisplayWinnerText -= DisplayWinner;
    }

    // Method to display winner text for this seat
    private void DisplayWinner(int seatId)
    {
        if (seatId == SeatId)
        {
            // Change the information text in the UI to indicate the winner
            SeatUI.ChangeInformationText("Winner");
        }
    }
}
