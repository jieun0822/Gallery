using UnityEngine;
using UnityEngine.UI;

public class SliderScrollSync : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Slider slider;
    public GameObject dropdownPanel;
    public GameObject dropdownBtn;
    public Sprite[] btnSprites;
    public CustomDropdownManager dropdownManager;

    private bool isUpdating = false;
    public bool isActive = false;

    void Start()
    {
        // �����̴� �� ��ũ�� ��ġ
        slider.onValueChanged.AddListener(OnSliderChanged);
        // ��ũ�� �� �����̴� ��ġ
        scrollRect.onValueChanged.AddListener(OnScrollChanged);

        // �ʱⰪ ����ȭ
        slider.value = 1f - scrollRect.verticalNormalizedPosition;
        dropdownPanel.SetActive(isActive);
    }

    void OnSliderChanged(float value)
    {
        if (isUpdating) return;
        isUpdating = true;
        scrollRect.verticalNormalizedPosition = 1f - value;
        isUpdating = false;
    }

    void OnScrollChanged(Vector2 pos)
    {
        if (isUpdating) return;
        isUpdating = true;
        slider.value = 1f - pos.y;
        isUpdating = false;
    }

    public void ShowContent()
    {
        var img = dropdownBtn.GetComponent<Image>();
        img.sprite = isActive? btnSprites[0] : btnSprites[1];

        isActive = !isActive;
        dropdownPanel.SetActive(isActive);
    }
}
