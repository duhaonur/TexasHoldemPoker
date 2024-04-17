using UnityEngine;

// ScriptableObject to hold AI decision weights
[CreateAssetMenu(fileName = "Weights", menuName = "Create New Weight Settings")]
public class AIDecisionWeightsSO : ScriptableObject
{
    // Pre-Flop settings
    [Header("Pre-Flop Settings")]
    public float PreFlopRaiseThreshold = 0.7f;
    public float PreFlopCallThreshold = 0.3f;
    public float PreFlopRaiseAggressiveness = 0.25f;

    // Pre-Flop weights
    [Header("Pre-Flop Weights")]
    public float PreFlopHandWeight = 0.4f;
    public float PreFlopPotWeight = 0.2f;
    public float PreFlopFutureHandWeight = 0.4f;

    // Flop settings
    [Header("Flop Settings")]
    public float FlopRaiseThreshold = 0.7f;
    public float FlopCallThreshold = 0.5f;
    public float FlopRaiseAggressiveness = 0.25f;

    // Flop weights
    [Header("Flop Weights")]
    public float FlopHandWeight = 0.2f;
    public float FlopPotWeight = 0.15f;
    public float FlopCommunityCardsWeight = 0.2f;
    public float FlopFullHandWeight = 0.2f;
    public float FlopFutureHandWeight = 0.25f;

    // Turn settings
    [Header("Turn Settings")]
    public float TurnRaiseThreshold = 1f;
    public float TurnCallThreshold = 0.50f;
    public float TurnRaiseAggressiveness = 0.25f;

    // Turn weights
    [Header("Turn Weights")]
    public float TurnHandWeight = 0.1f;
    public float TurnPotWeight = 0.15f;
    public float TurnCommunityCardsWeight = 0.25f;
    public float TurnFullHandWeight = 0.25f;
    public float TurnFutureHandWeight = 0.25f;

    // River settings
    [Header("River Settings")]
    public float RiverRaiseThreshold = 1f;
    public float RiverCallThreshold = 0.50f;
    public float RiverRaiseAggressiveness = 0.25f;

    // River weights
    [Header("River Weights")]
    public float RiverHandWeight = 0f;
    public float RiverPotWeight = 0.5f;
    public float RiverCommunityCardsWeight = 0f;
    public float RiverFullHandWeight = 0.5f;
    public float RiverFutureHandWeight = 0f;
}
