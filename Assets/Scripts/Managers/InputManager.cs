using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    // Tek ve ortak Input Actions kopyamýz.
    public Inputs inputActions;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new Inputs();

        LoadBindingOverrides();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void SaveBindingOverrides()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("InputOverrides", rebinds);
        PlayerPrefs.Save();
    }

    public void LoadBindingOverrides()
    {
        string rebinds = PlayerPrefs.GetString("InputOverrides");
        if (!string.IsNullOrEmpty(rebinds))
        {
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }
}