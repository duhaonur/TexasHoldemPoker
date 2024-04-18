using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LeaderboardProvider : MonoBehaviour
{
    [Serializable]
    public struct LeaderboardItem
    {
        public string UserName;
        public int Score;
    }
    public abstract void GetData(Action<List<LeaderboardItem>> onComplete);
    public abstract void SetScore(string username=null,int score = 0);
    public abstract string GetVendor();
  
}
