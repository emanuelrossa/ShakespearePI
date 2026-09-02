using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseSystem : MonoBehaviour
{
    public static PauseSystem Instance;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnPause; // arrasta aqui o script de movimento/câmera

    private bool paused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        // checagem de segurança, printa no console se faltar algo essencial
        if (FindFirstObjectByType<EventSystem>() == null)
            Debug.LogError("Não tem EventSystem na cena! Cria um: botão direito na Hierarchy > UI > Event System");
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

        if (pauseMenu != null)
            pauseMenu.SetActive(paused);

        foreach (MonoBehaviour script in scriptsToDisableOnPause)
        {
            if (script != null)
                script.enabled = !paused;
        }

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void Resume()
    {
        if (paused)
            TogglePause();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
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
}