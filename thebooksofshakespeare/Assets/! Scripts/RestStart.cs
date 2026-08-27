using UnityEngine;
using UnityEngine.SceneManagement;

public class RestStart : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Mansão");
        Cursor.lockState = CursorLockMode.Locked; 
        Time.timeScale = 1f;
    }
}