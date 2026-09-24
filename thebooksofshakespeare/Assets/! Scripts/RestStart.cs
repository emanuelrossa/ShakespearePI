using UnityEngine;
using UnityEngine.SceneManagement;

public class RestStart : MonoBehaviour
{
    [Header("Configuração de Cena")]
    [SerializeField] private string sceneToLoad = "";

    public void ResetGame()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadScene(sceneToLoad);
        }

    }
}