using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public void SairDoGame()
    {
        SceneManager.LoadScene("TitleScene");
        Time.timeScale = 1f;
    }
}