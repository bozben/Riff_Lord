using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RebindUI : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Input Action'daki tam ismi yaz (Örn: Lane1)")]
    public string actionName;

    [Header("UI Elements")]
    public TextMeshProUGUI bindingText; // Tuþun adýný yazan text (A, S, D...)
    public Button rebindButton;
    public TextMeshProUGUI waitingText; // "Tuþa Bas..." yazýsý
    public GameObject duplicateWarningObj; // Opsiyonel: "Bu tuþ dolu!" uyarýsý

    private InputAction inputAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        inputAction = InputManager.instance.inputActions.asset.FindActionMap("RhythmGame").FindAction(actionName);

        UpdateUI();

        rebindButton.onClick.AddListener(StartRebinding);

        if (duplicateWarningObj != null) duplicateWarningObj.SetActive(false);
    }

    void StartRebinding()
    {
        inputAction.Disable();

        if (bindingText != null) bindingText.gameObject.SetActive(false);
        if (waitingText != null) waitingText.gameObject.SetActive(true);
        if (duplicateWarningObj != null) duplicateWarningObj.SetActive(false);

        rebindingOperation = inputAction.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    void RebindComplete()
    {
        if (CheckForDuplicate())
        {
            inputAction.RemoveBindingOverride(0);

            if (duplicateWarningObj != null) duplicateWarningObj.SetActive(true);

            rebindingOperation.Dispose();
            inputAction.Enable();

            if (bindingText != null) bindingText.gameObject.SetActive(true);
            if (waitingText != null) waitingText.gameObject.SetActive(false);
            UpdateUI();
            return;
        }

        rebindingOperation.Dispose();
        inputAction.Enable();

        if (bindingText != null) bindingText.gameObject.SetActive(true);
        if (waitingText != null) waitingText.gameObject.SetActive(false);

        UpdateUI();

        InputManager.instance.SaveBindingOverrides();
    }

    bool CheckForDuplicate()
    {
        InputActionMap actionMap = InputManager.instance.inputActions.asset.FindActionMap("RhythmGame");

        string newBindingPath = inputAction.bindings[0].effectivePath;

        foreach (InputAction action in actionMap.actions)
        {
            if (action == inputAction) continue;

            if (action.bindings[0].effectivePath == newBindingPath)
            {
                return true;
            }
        }
        return false; 
    }

    void UpdateUI()
    {
        if (inputAction != null)
        {
            bindingText.text = inputAction.GetBindingDisplayString(0).ToUpper();
        }
    }
}