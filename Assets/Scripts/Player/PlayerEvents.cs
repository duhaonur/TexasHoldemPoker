using System;
using static CardSettings;

public static class PlayerEvents
{
    // Event for player raising
    public static Action<int> OnPlayerRaise;

    // Method to invoke the player raise event
    public static void CallPlayerRaise(int raiseAmount) { OnPlayerRaise?.Invoke(raiseAmount); }

    // Event for player calling
    public static Action<int> OnPlayerCall;

    // Method to invoke the player call event
    public static void CallPlayerCall(int callAmount) { OnPlayerCall?.Invoke(callAmount); }

    // Event for player checking
    public static Action OnPlayerCheck;

    // Method to invoke the player check event
    public static void CallPlayerCheck() { OnPlayerCheck?.Invoke(); }

    // Event for player folding
    public static Action OnPlayerFold;

    // Method to invoke the player fold event
    public static void CallPlayerFold() { OnPlayerFold?.Invoke(); }

    // Event for moving the camera
    public static Action<float, float> OnMoveCamera;

    // Method to invoke the camera movement event
    public static void CallMoveCamera(float x, float y) { OnMoveCamera?.Invoke(x, y); }

    // Event for displaying UI
    public static Action OnDisplayUI;

    // Method to invoke the UI display event
    public static void CallDisplayUI() { OnDisplayUI?.Invoke(); }

    // Event for displaying the player's hand
    public static Action<HandRank> OnDisplayHand;

    // Method to invoke the hand display event
    public static void CallDisplayHand(HandRank hand) { OnDisplayHand?.Invoke(hand); }

    // Event for displaying total money
    public static Action<int> OnDisplayTotalMoney;

    // Method to invoke the total money display event
    public static void CallDisplayTotalMoney(int totalMoney) { OnDisplayTotalMoney?.Invoke(totalMoney); }
}
