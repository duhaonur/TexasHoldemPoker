using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class PlayerEvents
{
    public static Action<int> OnPlayerRaise;
    public static void CallPlayerRaise(int raiseAmount) { OnPlayerRaise?.Invoke(raiseAmount); }
    public static Action<int> OnPlayerCall;
    public static void CallPlayerCall(int callAmount) { OnPlayerCall?.Invoke(callAmount); }
    public static Action OnPlayerCheck;
    public static void CallPlayerCheck() { OnPlayerCheck?.Invoke(); }
    public static Action OnPlayerFold;
    public static void CallPlayerFold() { OnPlayerFold?.Invoke(); }
    public static Action<float, float> OnMoveCamera;
    public static void CallMoveCamera(float x, float y) { OnMoveCamera?.Invoke(x, y); }
}
