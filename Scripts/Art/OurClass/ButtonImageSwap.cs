using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageSwap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public CustomBtn targetButton;

    // 마우스를 올렸을 때 (하이라이트)
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetButton.SetVisualState("highlighted");
    }

    // 마우스가 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        targetButton.SetVisualState("normal");
    }

    // 버튼 눌렀을 때
    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetImage != null && pressedSprite != null)
            targetImage.sprite = pressedSprite;

        targetButton.SetVisualState("pressed");
    }

    // 버튼 뗐을 때
    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetImage != null && normalSprite != null)
            targetImage.sprite = normalSprite;

        // 마우스가 여전히 버튼 위에 있으면 Highlight 유지
        if (RectTransformUtility.RectangleContainsScreenPoint(
            targetButton.GetComponent<RectTransform>(),
            eventData.position,
            eventData.enterEventCamera))
        {
            targetButton.SetVisualState("highlighted");
        }
        else
        {
            targetButton.SetVisualState("normal");
        }
    }

    
}
