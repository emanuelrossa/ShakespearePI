using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : MonoBehaviour
{
<<<<<<< HEAD
    public static PauseSystem Instance;

    [Header("UI & Scripts")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnPause;

    [Header("Configurações do Cursor (Durante o Jogo)")]
    [SerializeField] private CursorLockMode gameplayLockMode = CursorLockMode.Locked;
    [SerializeField] private bool gameplayCursorVisible = false;

=======
    [SerializeField] private GameObject pauseMenu;
>>>>>>> parent of 774fff2 (sistema de save meio feito)
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
<<<<<<< HEAD

        if (FindFirstObjectByType<EventSystem>() == null)
            Debug.LogError("Não tem EventSystem na cena! Cria um: botão direito na Hierarchy > UI > Event System");

        AplicarEstadoCursor(false);
=======
>>>>>>> parent of 774fff2 (sistema de save meio feito)
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

<<<<<<< HEAD
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
=======
        Time.timeScale = paused ? 0 : 1;
>>>>>>> parent of 774fff2 (sistema de save meio feito)

        if (pauseMenu != null)
            pauseMenu.SetActive(paused);

        if (cameraControl != null)
            cameraControl.enabled = !paused;

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

    // Métodos para os botões
    public void Resume()
    {
        if (paused)
            TogglePause();
    }

    public void Restart()
    {
<<<<<<< HEAD
        RestaurarEstadoGeral();
=======
        Time.timeScale = 1;
>>>>>>> parent of 774fff2 (sistema de save meio feito)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
<<<<<<< HEAD
        RestaurarEstadoGeral();
=======
        Time.timeScale = 1;
>>>>>>> parent of 774fff2 (sistema de save meio feito)
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