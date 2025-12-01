using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}