using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CardSettings;

public class PlayerUIManager : MonoBehaviour
{
    // Player's seat ID
    public int SeatId;

    // Canvas groups for UI elements
    [SerializeField] private CanvasGroup _raiseCanvasGroup;
    [SerializeField] private CanvasGroup _wholeCanvasGroup;

    // Slider for raising bets
    [SerializeField] private Slider _raiseSlider;

    // Text elements
    [SerializeField] private TextMeshProUGUI _raiseText;
    [SerializeField] private TextMeshProUGUI _checkCallText;
    [SerializeField] private TextMeshProUGUI _quickActionOneText;
    [SerializeField] private TextMeshProUGUI _quickActionTwoText;
    [SerializeField] private TextMeshProUGUI _totalMoneyText;
    [SerializeField] private TextMeshProUGUI _handText;

    // Buttons for quick actions
    [SerializeField] private Button _quickActionButtonOne;
    [SerializeField] private Button _quickActionButtonTwo;

    // Fade duration for canvas groups
    [SerializeField] private float _fadeDuration;

    // Coroutines for fading canvas groups
    private Coroutine _fadeCoroutine;
    private Coroutine _wholeCanvasCoroutine;

    // Flag to track if the raise canvas is active
    private bool _isRaiseCanvasActive = false;

    // Subscribe to events when enabled
    private void OnEnable()
    {
        PlayerEvents.OnDisplayUI += DisplayUI;
        PlayerEvents.OnDisplayHand += DisplayHand;
        PlayerEvents.OnDisplayTotalMoney += DisplayTotalMoney;
    }

    // Unsubscribe from events when disabled
    private void OnDisable()
    {
        PlayerEvents.OnDisplayUI -= DisplayUI;
        PlayerEvents.OnDisplayHand -= DisplayHand;
        PlayerEvents.OnDisplayTotalMoney -= DisplayTotalMoney;
    }

    // Start method to initialize UI
    private void Start()
    {
        HideUI();
        _handText.text = "";
    }

    // Button method for folding
    public void FoldButton()
    {
        PlayerEvents.CallPlayerFold();
        HideUI();
    }

    // Button method for checking or calling
    public void CheckCallButton()
    {
        if (PlayerData.CurrentBet == SharedData.HighestBet)
        {
            PlayerEvents.CallPlayerCheck();
        }
        else
        {
            int callAmount = Mathf.Max(SharedData.HighestBet - PlayerData.CurrentBet, 0);
            PlayerEvents.CallPlayerCall(callAmount);
        }

        HideUI();
    }

    // Button method for activating the raise canvas
    public void RaiseCanvasGroup()
    {
        // Set max value for the raise slider
        _raiseSlider.maxValue = PlayerData.TotalMoney;

        // Check if the slider value is greater than 0
        if (_raiseSlider.value > 0)
        {
            // Check if the raised amount is greater than or equal to the highest bet
            if (_raiseSlider.value >= SharedData.HighestBet)
            {
                PlayerEvents.CallPlayerRaise((int)_raiseSlider.value);
                HideUI();
            }
            _raiseSlider.value = 0;
        }

        // Stop the fade coroutine if running
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        // Toggle the raise canvas visibility
        if (!_isRaiseCanvasActive)
        {
            _isRaiseCanvasActive = true;
            _raiseText.text = "Close";
            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(_raiseCanvasGroup, _raiseCanvasGroup.alpha, 1f, _fadeDuration));
        }
        else
        {
            _isRaiseCanvasActive = false;
            _raiseText.text = "Raise";
            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(_raiseCanvasGroup, _raiseCanvasGroup.alpha, 0f, _fadeDuration));
        }
    }

    // Method to update raise text based on slider value
    public void SliderChangeRaiseText()
    {
        if (_raiseSlider.value == 0)
        {
            _raiseText.text = "Close";
        }
        else
        {
            _raiseText.text = $"Raise {CurrencyFormatter.FormatCurrency(_raiseSlider.value)}";
        }
    }

    // Event handler to display the player's hand
    private void DisplayHand(HandRank hand)
    {
        _handText.text = hand.ToString();
    }

    // Event handler to display total money
    private void DisplayTotalMoney(int totalMoney)
    {
        _totalMoneyText.text = $"${totalMoney}";
    }

    // Event handler to display UI
    private void DisplayUI()
    {
        // Set text for quick action buttons based on game state
        if (PlayerData.CurrentBet == SharedData.HighestBet)
        {
            SetQuickActionButtonOneText("1/4 Pot");
            SetQuickActionButtonTwoText("1/2 Pot");
            SetCheckCallText("Check");
        }
        else
        {
            SetQuickActionButtonOneText("2x");
            SetQuickActionButtonTwoText("4x");
            SetCheckCallText("Call");
        }

        // Show quick action buttons
        QuickActionButtonOne();
        QuickActionButtonTwo();

        // Stop the whole canvas coroutine if running
        if (_wholeCanvasCoroutine != null)
            StopCoroutine(_wholeCanvasCoroutine);

        // Fade in the whole canvas group
        _wholeCanvasCoroutine = StartCoroutine(FadeCanvasGroup(_wholeCanvasGroup, _wholeCanvasGroup.alpha, 1, _fadeDuration));
    }

    // Method to hide UI elements
    private void HideUI()
    {
        // Stop the whole canvas coroutine if running
        if (_wholeCanvasCoroutine != null)
            StopCoroutine(_wholeCanvasCoroutine);

        // Fade out the whole canvas group
        _wholeCanvasCoroutine = StartCoroutine(FadeCanvasGroup(_wholeCanvasGroup, _wholeCanvasGroup.alpha, 0, _fadeDuration));
    }

    // Method to handle quick action button one
    private void QuickActionButtonOne()
    {
        if (PlayerData.CurrentBet == SharedData.HighestBet)
        {
            int oneQuarterOfThePot = SharedData.Pot / 4;
            if (oneQuarterOfThePot >= PlayerData.TotalMoney)
            {
                SetQuickActionButtonOneText("ALL IN");
            }
            else
            {
                _quickActionButtonOne.gameObject.SetActive(true);
                _quickActionButtonOne.onClick?.RemoveAllListeners();
                _quickActionButtonOne.onClick.AddListener(OneQuarterOfThePot);
            }
        }
        else
        {
            int twoTimes = SharedData.HighestBet * 2;
            if (twoTimes >= PlayerData.TotalMoney)
            {
                SetQuickActionButtonOneText("ALL IN");
            }
            else
            {
                _quickActionButtonOne.gameObject.SetActive(true);
                _quickActionButtonOne.onClick?.RemoveAllListeners();
                _quickActionButtonOne.onClick.AddListener(TwoTimesOfTheHighestBet);
            }
        }
    }

    // Method to handle quick action button two
    private void QuickActionButtonTwo()
    {
        if (PlayerData.CurrentBet == SharedData.HighestBet)
        {
            int halfOfThePot = SharedData.Pot / 2;
            if (halfOfThePot >= PlayerData.TotalMoney)
            {
                _quickActionButtonTwo.gameObject.SetActive(false);
            }
            else
            {
                _quickActionButtonTwo.gameObject.SetActive(true);
                _quickActionButtonTwo.onClick?.RemoveAllListeners();
                _quickActionButtonTwo.onClick.AddListener(HalfOfThePot);
            }
        }
        else
        {
            int fourTimes = SharedData.HighestBet * 4;
            if (fourTimes >= PlayerData.TotalMoney)
            {
                _quickActionButtonTwo.gameObject.SetActive(false);
            }
            else
            {
                _quickActionButtonTwo.gameObject.SetActive(true);
                _quickActionButtonTwo.onClick?.RemoveAllListeners();
                _quickActionButtonTwo.onClick.AddListener(FourTimesOfTheHighestBet);
            }
        }
    }

    // Method to set text for check/call button
    private void SetCheckCallText(string text)
    {
        if (text == "Call")
        {
            int callAmount = Mathf.Max(SharedData.HighestBet - PlayerData.CurrentBet, 0);
            if (callAmount >= PlayerData.TotalMoney)
            {
                _checkCallText.text = "ALL IN";
                return;
            }
        }

        _checkCallText.text = text;
    }

    // Method to set text for quick action button one
    private void SetQuickActionButtonOneText(string text)
    {
        _quickActionOneText.text = text;
    }

    // Method to set text for quick action button two
    private void SetQuickActionButtonTwoText(string text)
    {
        _quickActionTwoText.text = text;
    }

    // Method for betting half of the pot
    private void HalfOfThePot()
    {
        PlayerEvents.CallPlayerRaise(SharedData.Pot / 2);
    }

    // Method for betting one quarter of the pot
    private void OneQuarterOfThePot()
    {
        PlayerEvents.CallPlayerRaise(SharedData.Pot / 4);
    }

    // Method for betting two times the highest bet
    private void TwoTimesOfTheHighestBet()
    {
        PlayerEvents.CallPlayerRaise(SharedData.HighestBet * 2);
    }

    // Method for betting four times the highest bet
    private void FourTimesOfTheHighestBet()
    {
        PlayerEvents.CallPlayerRaise(SharedData.HighestBet * 4);
    }

    // Coroutine to fade canvas group alpha
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float fadeDuration)
    {
        float currentTime = 0f;

        if (targetAlpha == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (currentTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / fadeDuration);
            canvasGroup.alpha = alpha;
            currentTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
