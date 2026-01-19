using UnityEngine;
using UnityEngine.UI;

public class UIHeldItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject itemContainer;
    [SerializeField] private Image heldItemImage;

    private GameObject _currentItem;

    private void Awake()
    {
        Clear();
    }

    public void SetHeldItem(GameObject item)
    {
        _currentItem = item;

        if (_currentItem == null)
        {
            Clear();
            return;
        }

        itemContainer.SetActive(true);

        SpriteRenderer sr = _currentItem.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogWarning("Held item has no SpriteRenderer");
            Clear();
            return;
        }

        heldItemImage.sprite = sr.sprite;
        heldItemImage.enabled = true;
    }

    public void Clear()
    {
        _currentItem = null;
        heldItemImage.sprite = null;
        heldItemImage.enabled = false;
        itemContainer.SetActive(false);
    }
}
