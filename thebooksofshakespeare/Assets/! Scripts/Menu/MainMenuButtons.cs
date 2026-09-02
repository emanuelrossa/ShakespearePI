using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject title;
    public GameObject creditos;
    public Button continuarButton; // arrasta o botão de "Continuar" aqui no Inspector

    private void Start()
    {
        title.SetActive(true);
        creditos.SetActive(false);

        // deixa o botão de continuar cinza se não tiver save
        continuarButton.interactable = SaveManager.Instance.HasSave();
    }

    public void Comecar()
    {
        SceneManager.LoadScene("Game");
    }

    public void Continuar()
    {
        SaveManager.Instance.Load();
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