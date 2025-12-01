using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class LaneKeyDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Text objects for lanes.")]
    public TextMeshProUGUI[] keyTexts;

    void Start()
    {
        UpdateKeyDisplays();
    }

    public void UpdateKeyDisplays()
    {
        if (InputManager.instance == null) return;

        var actions = InputManager.instance.inputActions.RhythmGame;

        if (keyTexts.Length > 0) keyTexts[0].text = actions.Lane1.GetBindingDisplayString(0).ToUpper();
        if (keyTexts.Length > 1) keyTexts[1].text = actions.Lane2.GetBindingDisplayString(0).ToUpper();
        if (keyTexts.Length > 2) keyTexts[2].text = actions.Lane3.GetBindingDisplayString(0).ToUpper();
        if (keyTexts.Length > 3) keyTexts[3].text = actions.Lane4.GetBindingDisplayString(0).ToUpper();
        if (keyTexts.Length > 4) keyTexts[4].text = actions.Lane5.GetBindingDisplayString(0).ToUpper();
    }
}