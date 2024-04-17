//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class LeaderboardManager : MonoBehaviour
//{
//    public static LeaderboardManager Instance;
//    private void Awake()
//    {
//        DontDestroyOnLoad(this);
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    LeaderboardProvider provider = null;
//    public enum LeaderboardProviderTypes {Firebase };
//    public LeaderboardProviderTypes LeaderboardProviderType= LeaderboardProviderTypes.Firebase;
//    private void Start()
//    {
//        switch (LeaderboardProviderType)
//        {
//            case LeaderboardProviderTypes.Firebase:
//                provider=GetComponent<LeaderboardFirebaseProvider>();
//                break;
//        }
//        Debug.Log("Leaderboard provider : " + provider.GetVendor());
//    }

//    public void GetLeaderboardData(Action<List<LeaderboardProvider.LeaderboardItem>> onComplete)
//    {
//        provider?.GetData(onComplete);       
//    }
//    public void SubmitScore(string userName=null ,int score=0)
//    {
//        provider?.SetScore(userName,score);
//    }
//}
