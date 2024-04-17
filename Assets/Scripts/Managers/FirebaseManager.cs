//using Firebase;
//using Firebase.Auth;
//using Firebase.Database;
//using Firebase.Extensions;
//using System;
//using System.Linq;
//using UnityEngine;

//public class FirebaseManager : MonoBehaviour
//{
//    public static FirebaseManager Instance;
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

//    public FirebaseAuth Auth;
//    public FirebaseUser User;

//    private void Start()
//    {
//        CheckFirebase();
//    }

//    private void CheckFirebase()
//    {
//        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
//        {
//            DependencyStatus status = task.Result;
//            if (status == DependencyStatus.Available)
//            {
//                Auth = FirebaseAuth.DefaultInstance;
//                User = Auth.CurrentUser;
//            }
//            else
//            {
//                Debug.Log("Baglanti Basarisiz");
//            }          
//        });
//    }
//    public void SetPlayerData(string key,int value)
//    {
//        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("Userss").Child(User.UserId);
//        reference.Child(key).SetValueAsync(value);
//    }
//    public void PlayerData(Action<string, int> onSendData)
//    {
//        FirebaseDatabase.DefaultInstance.GetReference("Userss").OrderByKey().EqualTo(User.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsFaulted || task.IsCanceled)
//            {
//                Debug.Log("Vericekme Hatasi");
//                return;
//            }
//            DataSnapshot snapshot = task.Result;
//            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
//            {
//                string userName = childSnapshot.Child("UserName").Value.ToString();
//                int totalMoney = int.Parse(childSnapshot.Child("TotalMoney").Value.ToString());
//                onSendData?.Invoke(userName, totalMoney);
//            }
//        });
//    }
//    private void GetData(string userName, int totalMoney)
//    {
//        Debug.Log(userName + " " + totalMoney + " ");
//    }
//}
