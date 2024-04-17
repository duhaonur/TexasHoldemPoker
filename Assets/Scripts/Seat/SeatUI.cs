using TMPro;
using UnityEngine;

public class SeatUI : MonoBehaviour
{
    // References to UI elements
    public Canvas Canvas;

    public RectTransform NameAndMoneyTextRect;
    public RectTransform BetTextRect;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI BetText;
    public TextMeshProUGUI InformationText;

    // Set up the UI camera and hide texts on start
    private void Start()
    {
        Canvas.worldCamera = Camera.main;
        HideTexts();
    }

    // Subscribe to game events when enabled
    private void OnEnable()
    {
        GameEvents.OnStartGame += GameStarted;
    }

    // Unsubscribe from game events when disabled
    private void OnDisable()
    {
        GameEvents.OnStartGame -= GameStarted;
    }

    // Display player name
    public void DisplayTexts(string name)
    {
        NameText.text = name;
        BetText.text = "";
        MoneyText.text = "";
        InformationText.text = "";
    }

    // Hide all texts
    public void HideTexts()
    {
        NameText.text = "";
        BetText.text = "";
        MoneyText.text = "";
        InformationText.text = "";
    }

    // Update bet text with amount and indicate if it's an all-in
    public void UpdateBetText(int amount, bool isAllIn)
    {
        if (!isAllIn)
            BetText.text = CurrencyFormatter.FormatCurrency(amount);
        else
            BetText.text = "AllIn";
    }

    // Update total money text
    public void UpdateTotalMoneyText(int amount)
    {
        MoneyText.text = CurrencyFormatter.FormatCurrency(amount);
    }

    // Change information text
    public void ChangeInformationText(string text)
    {
        InformationText.text = text;
    }

    // Reset bet text and information text when the game starts
    private void GameStarted()
    {
        BetText.text = "";
        InformationText.text = "";
    }
}
