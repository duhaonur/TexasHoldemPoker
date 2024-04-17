//using Firebase.Database;
//using Firebase.Extensions;
//using System.Linq;
//using System;
//using System.Collections.Generic;
//public class LeaderboardFirebaseProvider : LeaderboardProvider
//{        
//    Dictionary<string, int> _leaderboardData = new Dictionary<string, int>();
//    public override void GetData(Action<List<LeaderboardItem>> onComplete)
//    {
//        List<LeaderboardItem> result = new List<LeaderboardItem>();

//        FirebaseDatabase.DefaultInstance.GetReference("Userss").OrderByChild("point").LimitToLast(5).GetValueAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsFaulted || task.IsCanceled)
//            {
//                return;
//            }
//            DataSnapshot snapshot = task.Result;
//            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
//            {
//                string name = childSnapshot.Child("username").Value.ToString();
//                int point = int.Parse(childSnapshot.Child("point").Value.ToString());              
//                _leaderboardData.Add(name, point);
//            }
//        });

     
//        foreach (var item in _leaderboardData)
//        {
//            result.Add(new LeaderboardItem()
//            {
//                UserName=item.Key,
//                Score=item.Value
//            });
//        }
//        onComplete?.Invoke(result);
//    }

//    public override string GetVendor()
//    {
//        return "Firebase Provider";
//    }

//    public override void SetScore(string username, int score)
//    {
//        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("Userss").Child(FirebaseManager.Instance.User.UserId);
//        reference.Child("point").SetValueAsync(3131);
//        reference.Child("level").SetValueAsync(25);  
//    }     
//}
