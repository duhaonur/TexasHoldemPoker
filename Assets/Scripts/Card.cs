using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    public CardSettings.Suit CardSuit;
    public CardSettings.Rank CardRank;

    public string CardName;

    private MeshRenderer _meshRenderer;
    private Material _newMaterial;

    private int _dissolveAmount = Shader.PropertyToID("_Dissolve_Amount");

    private float _currentTime = 0;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _newMaterial = new Material(_meshRenderer.material);
        _meshRenderer.material = _newMaterial;
        _newMaterial.SetFloat(_dissolveAmount, 1);
    }
    public IEnumerator DisplayCard(float displayDuration)
    {
        float currentTime = 0f;

        while (currentTime < displayDuration)
        {
            float dissolveAmount = Mathf.Lerp(1, 0, currentTime / displayDuration);
            _newMaterial.SetFloat(_dissolveAmount, dissolveAmount);
            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the card is fully dissolved
        _newMaterial.SetFloat(_dissolveAmount, 0);
    }
    public IEnumerator HideCard(float displayDuration)
    {
        float currentTime = 0f;

        while (currentTime < displayDuration)
        {
            float dissolveAmount = Mathf.Lerp(0, 1, currentTime / displayDuration);
            _newMaterial.SetFloat(_dissolveAmount, dissolveAmount);
            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the card is fully dissolved
        _newMaterial.SetFloat(_dissolveAmount, 1);
    }
    private IEnumerator Dissolve(Material dissolveMaterial, int dissolvePropertyID, float startValue, float endValue, float duration)
    {
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float dissolveAmount = Mathf.Lerp(startValue, endValue, currentTime / duration);
            dissolveMaterial.SetFloat(dissolvePropertyID, dissolveAmount);
            currentTime += Time.deltaTime;
            yield return null;
        }
        dissolveMaterial.SetFloat(dissolvePropertyID, endValue);
    }
}
