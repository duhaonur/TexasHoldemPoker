using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class SeatUI : MonoBehaviour
{
    public Canvas Canvas;

    public LookAtConstraint NameAndMoneyConstraint;
    public LookAtConstraint BetConstraint;

    public RectTransform NameAndMoneyTextRect;
    public RectTransform BetTextRect;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI BetText;

    private void Start()
    {
        Canvas.worldCamera = Camera.main;
        HideTexts();
    }
    private void OnEnable()
    {
        //SeatEvents.OnUpdateBetText += UpdateBetText;
        //SeatEvents.OnUpdateMoneyText += UpdateMoneyText;
    }
    private void OnDisable()
    {
        //SeatEvents.OnUpdateBetText -= UpdateBetText;
        //SeatEvents.OnUpdateMoneyText -= UpdateMoneyText;
    }
    public void DisplayTexts(string name)
    {
        NameText.text = name;
        BetText.text = "";
        MoneyText.text = "";
    }
    public void HideTexts()
    {
        NameText.text = "";
        BetText.text = "";
        MoneyText.text = "";
    }
    public void UpdateBetText(int amount, bool isAllIn)
    {
        if (!isAllIn)
            BetText.text = CurrencyFormatter.FormatCurrency(amount);
        else
            BetText.text = "AllIn";
    }
    public void UpdateTotalMoneyText(int amount)
    {
        MoneyText.text = CurrencyFormatter.FormatCurrency(amount);
    }
}
