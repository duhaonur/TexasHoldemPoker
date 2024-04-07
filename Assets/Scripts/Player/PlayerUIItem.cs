using UnityEngine;
using UnityEngine.UI;
public class PlayerUIItem : MonoBehaviour
{
    [HideInInspector] public Image ItemImage;
    [HideInInspector] public Transform ItemTransform;

    private void Awake()
    {
        ItemImage = GetComponent<Image>();
        ItemTransform = transform;
    }

}
