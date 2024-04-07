using UnityEngine;

[CreateAssetMenu(fileName = "New Random Name List", menuName = "ScriptableObjects/Random Name List")]
public class RandomNameList : ScriptableObject
{
    public string[] Names;
}