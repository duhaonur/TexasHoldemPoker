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
    [SerializeField] private TextMeshProUGUI _quickActionOneText;
    [SerializeField] private TextMeshProUGUI _quickActionTwoText;

    [SerializeField] private float _fadeDuration;

    private Coroutine _fadeCoroutine;

    private bool _isRaiseCanvasActive = false;
    public void FoldButton()
    {
        PlayerEvents.CallPlayerFold();
    }
    public void CheckButton()
    {
        PlayerEvents.CallPlayerCheck();
    }
    public void RaiseCanvasGroup()
    {
        if (_raiseSlider.value > 0)
        {
            if (_raiseSlider.value >= SharedData.HighestBet)
            {
                PlayerEvents.CallPlayerRaise((int)_raiseSlider.value);
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
        _raiseText.text = $"Raise {CurrencyFormatter.FormatCurrency(_raiseSlider.value)}";
    }
    public void QuickActionButtonOne()
    {

    }
    public void QuickActionButtonTwo()
    {

    }
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float fadeDuration)
    {
        float currentTime = 0f;

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
