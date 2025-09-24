using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldFix : MonoBehaviour, IPointerClickHandler
{
    public InputField inputField;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!inputField.isFocused)
        {
            inputField.ActivateInputField();
        }
    }
}
