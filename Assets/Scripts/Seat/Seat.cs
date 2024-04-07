using UnityEngine;

public class Seat : MonoBehaviour
{
    public SeatUI SeatUI;

    public RandomNameList NameList;

    public Transform CameraLook;
    public Transform CameraFollow;

    public GameObject seatedObj;
    public Transform[] CardPositions;

    public int SeatId;

    private void Start()
    {
        SeatEvents.CallSeatCreated(SeatId, this);
    }
}
