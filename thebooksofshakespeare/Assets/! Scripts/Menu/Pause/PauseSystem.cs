using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool paused = false;
    private MonoBehaviour cameraControl;

    private void Start()
    {
        // CORRIGIDO: Usando FindFirstObjectByType
        cameraControl = FindFirstObjectByType<StarterAssets.StarterAssetsInputs>();
        if (cameraControl == null)
            cameraControl = FindFirstObjectByType<StarterAssets.ThirdPersonController>();

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Método separado para pausar (melhor prática)
    public void TogglePause()
    {
        paused = !paused;

        Time.timeScale = paused ? 0 : 1;

        if (pauseMenu != null)
            pauseMenu.SetActive(paused);

        if (cameraControl != null)
            cameraControl.enabled = !paused;

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    // Métodos para os botões
    public void Resume()
    {
        if (paused)
            TogglePause();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
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