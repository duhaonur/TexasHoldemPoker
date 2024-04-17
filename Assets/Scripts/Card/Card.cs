using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Card properties
    public CardSettings.Suit CardSuit;
    public CardSettings.Rank CardRank;
    public string CardName;

    // References
    private MeshRenderer _meshRenderer;
    private Material _newMaterial;

    // Dissolve shader property ID
    private int _dissolveAmount = Shader.PropertyToID("_Dissolve_Amount");

    // Start is called before the first frame update
    private void Start()
    {
        // Get the MeshRenderer component
        _meshRenderer = GetComponent<MeshRenderer>();

        // Create a new material instance to manipulate
        _newMaterial = new Material(_meshRenderer.material);

        // Assign the new material to the MeshRenderer
        _meshRenderer.material = _newMaterial;

        // Initialize dissolve amount to fully dissolved
        _newMaterial.SetFloat(_dissolveAmount, 1);
    }

    // Coroutine to display the card with dissolve effect
    public IEnumerator DisplayCard(float displayDuration)
    {
        float currentTime = 0f;

        while (currentTime < displayDuration)
        {
            // Calculate the dissolve amount based on time
            float dissolveAmount = Mathf.Lerp(1, 0, currentTime / displayDuration);

            // Set the dissolve amount in the material
            _newMaterial.SetFloat(_dissolveAmount, dissolveAmount);

            // Increment time
            currentTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the card is fully displayed
        _newMaterial.SetFloat(_dissolveAmount, 0);
    }

    // Coroutine to hide the card with dissolve effect
    public IEnumerator HideCard(float hideDuration)
    {
        float currentTime = 0f;

        while (currentTime < hideDuration)
        {
            // Calculate the dissolve amount based on time
            float dissolveAmount = Mathf.Lerp(0, 1, currentTime / hideDuration);

            // Set the dissolve amount in the material
            _newMaterial.SetFloat(_dissolveAmount, dissolveAmount);

            // Increment time
            currentTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the card is fully hidden
        _newMaterial.SetFloat(_dissolveAmount, 1);
    }

}
