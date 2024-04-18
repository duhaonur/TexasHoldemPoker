using TMPro;
using UnityEngine;

public class UILeaderboard : MonoBehaviour
{
    public Transform LeaderboardRoot;
    public GameObject LeaderboardItem;

    public UIAuth UIAuth;
    private void Start()
    {
        //SendRandomScores();
        FillLeaderboard();
    }

    //private void SendRandomScores()
    //{
    //    LeaderboardManager.Instance.SubmitScore(); 
    //}

    public void FillLeaderboard()
    {
        for (int i = 0; i < LeaderboardRoot.childCount; i++)
        {
            Destroy(LeaderboardRoot.GetChild(i).gameObject);
        }
        
        LeaderboardManager.Instance.GetLeaderboardData((data) =>
        {
            GameObject item;
            for (int i = 0; i < data.Count; i++)
            {
                item = Instantiate(LeaderboardItem, LeaderboardRoot);
                item.transform.Find("TxtUserName").GetComponent<TextMeshProUGUI>().text = data[i].UserName.ToString();
                item.transform.Find("TxtScore").GetComponent<TextMeshProUGUI>().text = data[i].Score.ToString().PadLeft(6, '0');
            }
        });
    }

    public void FillLeaderboardButton()
    {
        for (int i = 0; i < LeaderboardRoot.childCount; i++)
        {
            Destroy(LeaderboardRoot.GetChild(i).gameObject);
        }

        LeaderboardManager.Instance.GetLeaderboardData((data) =>
        {
            GameObject item;
            for (int i = 0; i < data.Count; i++)
            {
                item = Instantiate(LeaderboardItem, LeaderboardRoot);
                item.transform.Find("TxtUserName").GetComponent<TextMeshProUGUI>().text = data[i].UserName.ToString();
                item.transform.Find("TxtScore").GetComponent<TextMeshProUGUI>().text = data[i].Score.ToString().PadLeft(6, '0');
            }
        });
        UIAuth.OpenPanel(UIAuth.LeaderboardPanel);
    }
}
