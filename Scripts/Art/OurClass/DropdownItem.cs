using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownItem : MonoBehaviour
{
    public TMP_Text label; // 항목 텍스트
    public Color selectedColor = Color.white;
    public Color normalColor = Color.black;

    private Button button;
    public CustomDropdownManager dropdownManager;

    public void SetSelected(bool selected)
    {
        label.color = selected ? selectedColor : normalColor;
        var img = GetComponent<Image>();

        var sprite = dropdownManager.selectedSprite;
        img.sprite = selected ? sprite : null;
    }

    public void OnClick()
    {
        FindFirstObjectByType<CustomDropdownManager>().SelectItem(this);
    }
}
