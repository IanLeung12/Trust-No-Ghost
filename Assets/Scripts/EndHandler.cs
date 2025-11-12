using UnityEngine;
using UnityEngine.SceneManagement;
public class EndHandler : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject winScreen;

    void Start()
    {
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    public void OnDeath()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void OnWin()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartEntireGame()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
