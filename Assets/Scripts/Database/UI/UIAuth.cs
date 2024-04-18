using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAuth : MonoBehaviour
{
    [Header("Set Name Settings")]
    public GameObject NamePanel;
    public TMP_InputField NameInput;
    public TextMeshProUGUI NameWarningText;
    public Button SetNameButton;

    public TextMeshProUGUI NameText;

    [Header("Login------------")]
    public GameObject LoginPanel;

    [Header("Game------------")]
    public GameObject GamePanel;
    public GameObject LeaderboardPanel;

    [Header("Email Login------------")]
    public GameObject EmailLoginPanel;
    public TMP_InputField EmailLoginInput;
    public TMP_InputField PasswordLoginInput;

    [Header("Email SingUp------------")]
    public GameObject EmailSingUpPanel;
    public TMP_InputField EmailSingUpInput;
    public TMP_InputField PasswordSingUpInput;


    [SerializeField] private FirebaseAuthentication firebaseAuthentication;
    private void Start()
    {
        NameInput.onValueChanged.AddListener(ChanegeUserNameListener);
    }
    public void ChanegeUserNameListener(string text)
    {
        if (text.Trim().Length > 2 && text.Trim().Length < 20)
        {
            firebaseAuthentication.CheckUserName(text.Trim(), SetNameButton);
        }
        else
        {
            NameWarningText.text = "Kullanici Adi 3-20 Haneli Olmali";
            NameWarningText.color = Color.red;
            SetNameButton.interactable = false;
        }
    }

    public void OpenPanel(GameObject panel)
    {
        ClosePanel();
        panel.SetActive(true);
    }

    private void ClosePanel()
    {
        NamePanel.SetActive(false);
        LoginPanel.SetActive(false);
        GamePanel.SetActive(false);
        EmailLoginPanel.SetActive(false);
        EmailSingUpPanel.SetActive(false);
        LeaderboardPanel.SetActive(false);
    }

    public void GetLevelDataStart(string name,int value=0)//burada gereken puan tamamlanýnca çaðýrýlacak kod var. OYUNCU BAÞARILI OLURSA ÇALIÞACAK.
    {
        NameText.text = name;
    }
}
