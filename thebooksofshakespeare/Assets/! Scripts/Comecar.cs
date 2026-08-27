using UnityEngine;
using UnityEngine.SceneManagement;

public class Comecar : MonoBehaviour
{
    public void ComecarGame()
    {
        SceneManager.LoadScene("Começo");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }
}