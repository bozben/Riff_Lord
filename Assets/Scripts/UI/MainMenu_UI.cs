using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_UI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.musicSource.Stop();

        }
    }
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("CombatScene");
    }

    public void OnSettingsClicked()
    {
        settingsPanel.SetActive(true);
    }

    public void OnCloseSettingsClicked()
    {
        settingsPanel.SetActive(false);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
    public void OnVolumeChanged(float volume)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetVolume(volume);
        }
    }
}