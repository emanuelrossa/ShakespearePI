using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject title;
    public GameObject creditos;

    private void Start()
    {
        title.SetActive(true);
        creditos.SetActive(false);
    }
    public void Comecar()
    {
        SceneManager.LoadScene("Game");
    }

    public void Creditos()
    {
        title.SetActive(false);
        creditos.SetActive(true);
    }

    public void CreditosVoltar()
    {
        creditos.SetActive(false);
        title.SetActive(true);
    }

    public void Sair()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}