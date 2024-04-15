using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public int SeatId;

    [SerializeField] private CanvasGroup _raiseCanvasGroup;
    [SerializeField] private CanvasGroup _wholeCanvasGroup;

    [SerializeField] private Slider _raiseSlider;

    [SerializeField] private TextMeshProUGUI _raiseText;
    [SerializeField] private TextMeshProUGUI _checkCallText;
    [SerializeField] private TextMeshProUGUI _quickActionOneText;
    [SerializeField] private TextMeshProUGUI _quickActionTwoText;

    [SerializeField] private Button _quickActionButtonOne;
    [SerializeField] private Button _quickActionButtonTwo;

    [SerializeField] private float _fadeDuration;

    private Coroutine _fadeCoroutine;
    private Coroutine _wholeCanvasCoroutine;

    private bool _isRaiseCanvasActive = false;
    private void OnEnable()
    {
        PlayerEvents.OnDisplayUI += DisplayUI;
    }
    private void OnDisable()
    {
        PlayerEvents.OnDisplayUI -= DisplayUI;
    }
    private void Start()
    {
        HideUI();
    }

    public void FoldButton()
    {
        PlayerEvents.CallPlayerFold();
        HideUI();
    }
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
    public void RaiseCanvasGroup()
    {
        if (_raiseSlider.value > 0)
        {
            if (_raiseSlider.value >= SharedData.HighestBet)
            {
                PlayerEvents.CallPlayerRaise((int)_raiseSlider.value);
                HideUI();
            }
            _raiseSlider.value = 0;
        }

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        if (!_isRaiseCanvasActive)
        {
            _isRaiseCanvasActive = true;
            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(_raiseCanvasGroup, _raiseCanvasGroup.alpha, 1f, _fadeDuration));
        }
        else
        {
            _isRaiseCanvasActive = false;
            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(_raiseCanvasGroup, _raiseCanvasGroup.alpha, 0f, _fadeDuration));
        }
    }
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
    private void DisplayUI()
    {
        if (PlayerData.CurrentBet == SharedData.HighestBet)
        {
            SetQuickActionButtonOneText("1 / 4");
            SetQuickActionButtonTwoText("1 / 2");
            SetCheckCallText("Check");
        }
        else
        {
            SetQuickActionButtonOneText("2x");
            SetQuickActionButtonTwoText("4x");
            SetCheckCallText("Call");
        }

        QuickActionButtonOne();
        QuickActionButtonTwo();

        if (_wholeCanvasCoroutine != null)
            StopCoroutine(_wholeCanvasCoroutine);

        _wholeCanvasCoroutine = StartCoroutine(FadeCanvasGroup(_wholeCanvasGroup, _wholeCanvasGroup.alpha, 1, _fadeDuration));
    }
    private void HideUI()
    {
        if (_wholeCanvasCoroutine != null)
            StopCoroutine(_wholeCanvasCoroutine);

        _wholeCanvasCoroutine = StartCoroutine(FadeCanvasGroup(_wholeCanvasGroup, _wholeCanvasGroup.alpha, 0, _fadeDuration));
    }
    private void QuickActionButtonOne()
    {
        if (PlayerData.CurrentBet == SharedData.HighestBet)
        {
            int halfOfThePot = SharedData.Pot / 2;
            if (halfOfThePot >= PlayerData.TotalMoney)
            {
                _quickActionButtonOne.gameObject.SetActive(false);
            }
            else
            {
                _quickActionButtonOne.gameObject.SetActive(true);
                _quickActionButtonOne.onClick?.RemoveAllListeners();
                _quickActionButtonOne.onClick.AddListener(HalfOfThePot);
            }
        }
        else
        {
            int twoTimes = SharedData.HighestBet * 2;
            if (twoTimes >= PlayerData.TotalMoney)
            {
                _quickActionButtonOne.gameObject.SetActive(false);
            }
            else
            {
                _quickActionButtonOne.gameObject.SetActive(true);
                _quickActionButtonOne.onClick?.RemoveAllListeners();
                _quickActionButtonOne.onClick.AddListener(TwoTimesOfTheHighestBet);
            }
        }
    }
    private void QuickActionButtonTwo()
    {
        if (PlayerData.CurrentBet == SharedData.HighestBet)
        {
            int oneQuarterOfThePot = SharedData.Pot / 4;
            if (oneQuarterOfThePot >= PlayerData.TotalMoney)
            {
                _quickActionButtonTwo.gameObject.SetActive(false);
            }
            else
            {
                _quickActionButtonTwo.gameObject.SetActive(true);
                _quickActionButtonTwo.onClick?.RemoveAllListeners();
                _quickActionButtonTwo.onClick.AddListener(OneQuarterOfThePot);
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
    private void SetCheckCallText(string text)
    {
        _checkCallText.text = text;
    }
    private void SetQuickActionButtonOneText(string text)
    {
        _quickActionOneText.text = text;
    }
    private void SetQuickActionButtonTwoText(string text)
    {
        _quickActionTwoText.text = text;
    }
    private void HalfOfThePot()
    {
        PlayerEvents.CallPlayerRaise(SharedData.Pot / 2);
    }
    private void OneQuarterOfThePot()
    {
        PlayerEvents.CallPlayerRaise(SharedData.Pot / 4);
    }
    private void TwoTimesOfTheHighestBet()
    {
        PlayerEvents.CallPlayerRaise(SharedData.HighestBet * 2);
    }
    private void FourTimesOfTheHighestBet()
    {
        PlayerEvents.CallPlayerRaise(SharedData.HighestBet * 4);
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float fadeDuration)
    {
        float currentTime = 0f;

        if(targetAlpha == 0)
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
