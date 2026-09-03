using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseSystem : MonoBehaviour
{
    public static PauseSystem Instance;

    [Header("UI & Scripts")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnPause;

    [Header("Configurações do Cursor (Durante o Jogo)")]
    [SerializeField] private CursorLockMode gameplayLockMode = CursorLockMode.Locked;
    [SerializeField] private bool gameplayCursorVisible = false;

    private bool paused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (FindFirstObjectByType<EventSystem>() == null)
            Debug.LogError("Não tem EventSystem na cena! Cria um: botão direito na Hierarchy > UI > Event System");

        AplicarEstadoCursor(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        paused = !paused;

        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;

        if (pauseMenu != null)
            pauseMenu.SetActive(paused);

        foreach (MonoBehaviour script in scriptsToDisableOnPause)
        {
            if (script != null)
                script.enabled = !paused;
        }

        AplicarEstadoCursor(paused);
    }

    private void AplicarEstadoCursor(bool estaPausado)
    {
        if (estaPausado)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = gameplayLockMode;
            Cursor.visible = gameplayCursorVisible;
        }
    }

    public void Resume()
    {
        if (paused)
            TogglePause();
    }

    public void Restart()
    {
        RestaurarEstadoGeral();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        RestaurarEstadoGeral();
        SceneManager.LoadScene("TitleScene");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RestaurarEstadoGeral()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}