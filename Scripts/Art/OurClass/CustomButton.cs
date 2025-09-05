using UnityEngine;
using UnityEngine.UI;

public class CustomButton : Button
{
    public void SetVisualState(string state)
    {
        switch (state.ToLower())
        {
            case "normal":
                DoStateTransition(Selectable.SelectionState.Normal, true);
                break;
            case "highlighted":
                DoStateTransition(Selectable.SelectionState.Highlighted, true);
                break;
            case "pressed":
                DoStateTransition(Selectable.SelectionState.Pressed, true);
                break;
            case "selected":
                DoStateTransition(Selectable.SelectionState.Selected, true);
                break;
            case "disabled":
                DoStateTransition(Selectable.SelectionState.Disabled, true);
                break;
            default:
                Debug.LogWarning("Unknown state: " + state);
                break;
        }
    }
}
