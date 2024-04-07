using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weights", menuName = "Create New Weight Settings")]
public class AIDecisionWeightsSO : ScriptableObject
{
    [Header("Pre-Flop Settings")]
    public float PreFlopRaiseThreshold = 0.7f;
    public float PreFlopCallThreshold = 0.3f;

    public float PreFlopHandWeight = 0.4f;
    public float PreFlopPotWeight = 0.6f;
    [Header("Flop Settings")]
    public float FlopRaiseThreshold = 0.7f;
    public float FlopCallThreshold = 0.5f;

    public float FlopHandWeight = 0.2f;
    public float FlopPotWeight = 0.15f;
    public float FlopCommunityCardsWeight = 0.3f;
    public float FlopFullHandWeight = 0.35f;
    [Header("Turn Settings")]
    public float TurnRaiseThreshold = 1f;
    public float TurnCallThreshold = 0.50f;

    public float TurnHandWeight = 0.2f;
    public float TurnPotWeight = 0.15f;
    public float TurnCommunityCardsWeight = 0.3f;
    public float TurnFullHandWeight = 0.35f;
    [Header("River Settings")]
    public float RiverRaiseThreshold = 1f;
    public float RiverCallThreshold = 0.50f;

    public float RiverHandWeight = 0.2f;
    public float RiverPotWeight = 0.15f;
    public float RiverCommunityCardsWeight = 0.3f;
    public float RiverFullHandWeight = 0.35f;
}
