using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JulietaCutscene : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public Transform playerSpawnPoint;

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

    // =========================================================
    //  DESAPARECIMENTO SEQUENCIAL
    // =========================================================

    [Header("Cenário que vai sumir (primeiro)")]
    [Tooltip("Arraste aqui todos os objetos do cenário que devem desaparecer primeiro.")]
    public GameObject[] cenarioObjects;

    [Header("Cavaleiros que somem um por vez (depois do cenário)")]
    [Tooltip("Cada cavaleiro some individualmente, em ordem.")]
    public GameObject[] cavaleiros;

    [Header("Configurações do desaparecimento")]
    [Tooltip("Duração do dissolve de CADA objeto (segundos).")]
    public float dissolveDuration = 2f;
    [Tooltip("Tempo de espera entre um objeto e outro.")]
    public float delayBetweenObjects = 0.3f;
    [Tooltip("Tempo de espera entre um cavaleiro e outro.")]
    public float delayBetweenCavaleiros = 1f;

    [Header("Diálogos por etapa")]
    [Tooltip("Falas mostradas ANTES do cenário desaparecer.")]
    public DialogoLinha[] dialogoAntesCenario;

    [Tooltip("Falas mostradas entre cada cavaleiro sumindo. Uma entrada por cavaleiro.")]
    public DialogoLinha[] dialogoPorCavaleiro;

    [Tooltip("Falas mostradas no final, antes da troca de cena.")]
    public DialogoLinha[] dialogoFinal;

    [Header("Troca de cena")]
    [Tooltip("Nome exato da cena a carregar no final.")]
    public string nomeProximaCena = "NomeDaProximaCena";
    [Tooltip("Atraso antes de trocar de cena (segundos).")]
    public float delayAntesTrocaCena = 1.5f;

    // Cache dos materiais originais para restaurar (opcional)
    private Dictionary<Renderer, Material> materiaisOriginais = new Dictionary<Renderer, Material>();

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        SpawnAllCharacters();
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

    // =========================================================
    //  ROTINA PRINCIPAL (ordem das etapas)
    // =========================================================

    private IEnumerator RotinaCutsceneCompleta()
    {
        yield return new WaitForSeconds(0.5f);

        // ---------- ETAPA 1: diálogo inicial (cena normal) ----------
        yield return MostrarDialogo(dialogoLinhas);

        // ---------- ETAPA 2: diálogo ANTES do cenário sumir ----------
        yield return MostrarDialogo(dialogoAntesCenario);

        // ---------- ETAPA 3: cenário inteiro desaparece ----------
        yield return DesaparecerGrupo(cenarioObjects, dissolveDuration, delayBetweenObjects);

        // ---------- ETAPA 4: cada cavaleiro some um por vez, com fala entre eles ----------
        if (cavaleiros != null)
        {
            for (int i = 0; i < cavaleiros.Length; i++)
            {
                // Fala ANTES do cavaleiro sumir
                if (dialogoPorCavaleiro != null && i < dialogoPorCavaleiro.Length)
                {
                    yield return MostrarDialogo(new DialogoLinha[] { dialogoPorCavaleiro[i] });
                }

                // Faz o cavaleiro sumir (pode esperar o dissolve acabar OU não)
                if (cavaleiros[i] != null)
                {
                    yield return DissolveObjeto(cavaleiros[i], dissolveDuration);
                }

                yield return new WaitForSeconds(delayBetweenCavaleiros);
            }
        }

        // ---------- ETAPA 5: diálogo final ----------
        yield return MostrarDialogo(dialogoFinal);

        // ---------- ETAPA 6: troca de cena ----------
        yield return new WaitForSeconds(delayAntesTrocaCena);

        if (!string.IsNullOrEmpty(nomeProximaCena))
        {
            SceneManager.LoadScene(nomeProximaCena);
        }
    }

    // =========================================================
    //  HELPERS
    // =========================================================

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

    /// <summary>
    /// Faz todos os objetos do grupo desaparecerem, um após o outro (com leve sobreposição).
    /// </summary>
    private IEnumerator DesaparecerGrupo(GameObject[] grupo, float duracao, float delay)
    {
        if (grupo == null) yield break;

        for (int i = 0; i < grupo.Length; i++)
        {
            if (grupo[i] == null) continue;

            // Dispara o dissolve sem travar a fila (permite sobreposição)
            StartCoroutine(DissolveObjeto(grupo[i], duracao));
            yield return new WaitForSeconds(delay);
        }

        // Aguarda o último dissolve terminar completamente
        yield return new WaitForSeconds(duracao);
    }

    /// <summary>
    /// Faz um único GameObject desaparecer suavemente via alpha do material.
    /// Funciona com materiais em modo Transparent / Fade.
    /// </summary>
    private IEnumerator DissolveObjeto(GameObject obj, float duracao)
    {
        if (obj == null) yield break;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            // Sem renderer ? só desativa no final
            yield return new WaitForSeconds(duracao);
            obj.SetActive(false);
            yield break;
        }

        // Prepara materiais (garante modo Transparent e alpha 1)
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
        // Tenta configurar o material para modo Transparent (funciona com Standard/URP Lit)
        if (mat.HasProperty("_Mode"))
        {
            // Built-in Standard Shader
            mat.SetFloat("_Mode", 3); // Transparent
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
            // URP Lit / Unlit
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0);   // Alpha
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = 3000;
        }
    }
}