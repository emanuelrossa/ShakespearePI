using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseSystem : MonoBehaviour
{
    public static PauseSystem Instance;
    public static bool IsPaused { get; private set; }

    [Header("UI & Scripts")]
    [SerializeField] private GameObject pauseMenu;
    [Tooltip("Scripts extras pra desativar no pause (opcional, o do Starter Assets é achado sozinho)")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnPause;

    [Header("Cena do menu principal")]
    [SerializeField] private string mainMenuSceneName = "TitleScene";

    [Header("Configurações do Cursor (Durante o Jogo)")]
    [SerializeField] private CursorLockMode gameplayLockMode = CursorLockMode.Locked;
    [SerializeField] private bool gameplayCursorVisible = false;

    // Nomes dos scripts do kit de primeira pessoa que vão ser desativados automaticamente
    private static readonly string[] autoDisableTypeNames =
    {
        "FirstPersonController",
        "StarterAssetsInputs"
    };

    private readonly List<MonoBehaviour> autoScripts = new List<MonoBehaviour>();
    private readonly List<MonoBehaviour> disabledByPause = new List<MonoBehaviour>();

    private void Awake()
    {
        Instance = this;
        IsPaused = false;
    }

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (FindFirstObjectByType<EventSystem>() == null)
            Debug.LogError("Não tem EventSystem na cena! Cria um: botão direito na Hierarchy > UI > Event System");

        BuscarScriptsAutomaticos();
        AplicarEstadoCursor(false);
    }

    private void BuscarScriptsAutomaticos()
    {
        autoScripts.Clear();
        MonoBehaviour[] todos = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour mb in todos)
        {
            if (mb == null || mb == this) continue;

            string nomeTipo = mb.GetType().Name;
            foreach (string alvo in autoDisableTypeNames)
            {
                if (nomeTipo == alvo)
                {
                    autoScripts.Add(mb);
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (EscapeApertado())
            TogglePause();
    }

    private bool EscapeApertado()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    public void TogglePause()
    {
        SetPause(!IsPaused);
    }

    private void SetPause(bool value)
    {
        IsPaused = value;

        Time.timeScale = value ? 0f : 1f;
        AudioListener.pause = value;

        if (pauseMenu != null)
            pauseMenu.SetActive(value);

        if (value)
        {
            // Desativa só o que tava ligado, e guarda pra religar depois
            disabledByPause.Clear();

            foreach (MonoBehaviour script in scriptsToDisableOnPause)
                DesativarSeLigado(script);

            foreach (MonoBehaviour script in autoScripts)
                DesativarSeLigado(script);
        }
        else
        {
            foreach (MonoBehaviour script in disabledByPause)
            {
                if (script != null)
                    script.enabled = true;
            }
            disabledByPause.Clear();
        }

        AplicarEstadoCursor(value);
    }

    private void DesativarSeLigado(MonoBehaviour script)
    {
        if (script == null || !script.enabled) return;
        script.enabled = false;
        disabledByPause.Add(script);
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

    // ---- Métodos pros botões ----

    public void Resume()
    {
        if (IsPaused)
            SetPause(false);
    }

    public void Restart()
    {
        RestaurarEstadoGeral();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        RestaurarEstadoGeral();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void Quit()
    {
        RestaurarEstadoGeral();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RestaurarEstadoGeral()
    {
        IsPaused = false;
        AudioListener.pause = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}