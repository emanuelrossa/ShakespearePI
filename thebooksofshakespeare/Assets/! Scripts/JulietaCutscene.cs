using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JulietaCutscene : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public Transform playerSpawnPoint;

    [Header("Configuração de Movimento")]
    public bool congelarPlayerDuranteCutscene = true;

    [Header("Julieta")]
    public GameObject julieta;
    public Transform julietaSpawnPoint;

    [Header("Moradores (Opcional)")]
    public GameObject[] moradores;
    public Transform[] moradoresSpawnPoints;

    [System.Serializable]
    public struct DialogoLinha
    {
        public string nomeFalante;
        [TextArea] public string fala;
    }

    [Header("Diálogos da Nova Cena (Múltiplos Falantes)")]
    public DialogoLinha[] dialogoLinhas;

    [Header("Cenário que vai sumir (primeiro)")]
    public GameObject[] cenarioObjects;

    [Header("Cavaleiros que somem um por vez (depois do cenário)")]
    public GameObject[] cavaleiros;

    [Header("Configurações do desaparecimento")]
    public float dissolveDuration = 2f;
    public float delayBetweenObjects = 0.3f;
    public float delayBetweenCavaleiros = 1f;

    [Header("Diálogos por etapa")]
    public DialogoLinha[] dialogoAntesCenario;
    public DialogoLinha[] dialogoPorCavaleiro;
    public DialogoLinha[] dialogoFinal;

    [Header("Troca de Cena e Fade")]
    public string nomeProximaCena = "NomeDaProximaCena";
    public float delayAntesTrocaCena = 1.5f;
    public CanvasGroup fadeCanvasGroup; 
    public float duracaoFade = 1.5f;  

    private Dictionary<Renderer, Material> materiaisOriginais = new Dictionary<Renderer, Material>();

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        SpawnAllCharacters();

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(RotinaCutsceneCompleta());
    }

    public void SpawnAllCharacters()
    {
        if (player != null && playerSpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;

            if (cc != null) cc.enabled = true;
        }

        if (julieta != null && julietaSpawnPoint != null)
        {
            julieta.transform.position = julietaSpawnPoint.position;
            julieta.transform.rotation = julietaSpawnPoint.rotation;
        }

        if (moradores != null && moradoresSpawnPoints != null)
        {
            int limit = Mathf.Min(moradores.Length, moradoresSpawnPoints.Length);
            for (int i = 0; i < limit; i++)
            {
                if (moradores[i] != null && moradoresSpawnPoints[i] != null)
                {
                    moradores[i].transform.position = moradoresSpawnPoints[i].position;
                    moradores[i].transform.rotation = moradoresSpawnPoints[i].rotation;
                }
            }
        }
    }

    private IEnumerator RotinaCutsceneCompleta()
    {
        if (congelarPlayerDuranteCutscene && player != null)
        {
            AlternarMovimentoPlayer(false);
        }

        yield return new WaitForSeconds(0.5f);

        yield return MostrarDialogo(dialogoLinhas);

        yield return MostrarDialogo(dialogoAntesCenario);

        yield return DesaparecerGrupo(cenarioObjects, dissolveDuration, delayBetweenObjects);

        if (cavaleiros != null)
        {
            for (int i = 0; i < cavaleiros.Length; i++)
            {
                if (dialogoPorCavaleiro != null && i < dialogoPorCavaleiro.Length)
                {
                    yield return MostrarDialogo(new DialogoLinha[] { dialogoPorCavaleiro[i] });
                }

                if (cavaleiros[i] != null)
                {
                    yield return DissolveObjeto(cavaleiros[i], dissolveDuration);
                }

                yield return new WaitForSeconds(delayBetweenCavaleiros);
            }
        }

        yield return MostrarDialogo(dialogoFinal);

        if (congelarPlayerDuranteCutscene && player != null)
        {
            AlternarMovimentoPlayer(true);
        }

        yield return new WaitForSeconds(delayAntesTrocaCena);

      
        if (fadeCanvasGroup != null)
        {
            float t = 0f;
            while (t < duracaoFade)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(t / duracaoFade);
                yield return null;
            }
        }

        if (!string.IsNullOrEmpty(nomeProximaCena))
        {
            SceneManager.LoadScene(nomeProximaCena);
        }
    }

    private void AlternarMovimentoPlayer(bool ativar)
    {
        MonoBehaviour[] scriptsPlayer = player.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scriptsPlayer)
        {
            if (script != this && (script.GetType().Name.Contains("Move") || script.GetType().Name.Contains("Look") || script.GetType().Name.Contains("Controller")))
            {
                script.enabled = ativar;
            }
        }
    }

    private IEnumerator MostrarDialogo(DialogoLinha[] linhas)
    {
        if (linhas == null || linhas.Length == 0) yield break;
        if (DialogueManager.Instance == null) yield break;

        foreach (var linha in linhas)
        {
            string[] falaUnica = new string[] { linha.fala };
            DialogueManager.Instance.StartDialogue(linha.nomeFalante, falaUnica, null);

            while (DialogueManager.Instance.IsTalking)
            {
                yield return null;
            }
        }
    }

    private IEnumerator DesaparecerGrupo(GameObject[] grupo, float duracao, float delay)
    {
        if (grupo == null) yield break;

        for (int i = 0; i < grupo.Length; i++)
        {
            if (grupo[i] == null) continue;

            StartCoroutine(DissolveObjeto(grupo[i], duracao));
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(duracao);
    }

    private IEnumerator DissolveObjeto(GameObject obj, float duracao)
    {
        if (obj == null) yield break;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            yield return new WaitForSeconds(duracao);
            obj.SetActive(false);
            yield break;
        }

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                ConfigurarMaterialTransparente(mat);
                Color c = mat.color;
                c.a = 1f;
                mat.color = c;
            }
        }

        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(t / duracao);

            foreach (var r in renderers)
            {
                if (r == null) continue;
                foreach (var mat in r.materials)
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
            yield return null;
        }

        obj.SetActive(false);
    }

    private void ConfigurarMaterialTransparente(Material mat)
    {
        if (mat.HasProperty("_Mode"))
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        else if (mat.HasProperty("_Surface"))
        {
            mat.SetFloat("_Surface", 1);
            mat.SetFloat("_Blend", 0);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = 3000;
        }
    }
}