using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Necessário para o Starter Assets

public class SaveController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PlayerSaveData playerSaveData;

    private void Start()
    {
        // Se não foi arrastado no Inspector, tenta encontrar o PlayerSaveData na cena
        if (playerSaveData == null)
        {
            playerSaveData = FindFirstObjectByType<PlayerSaveData>();
        }
    }

    private void Update()
    {
        // Garante que o teclado está disponível
        if (Keyboard.current == null) return;

        // Pressione Z para SALVAR
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            SalvarJogo();
        }

        // Pressione X para CARREGAR
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            CarregarJogo();
        }
    }

    public void SalvarJogo()
    {
        SaveData dadosGerais = new SaveData();

        // 1. Coleta dados do Player
        if (playerSaveData != null)
        {
            SaveData dadosPlayer = playerSaveData.ObterDados();
            dadosGerais.NomeJogador = dadosPlayer.NomeJogador;
            dadosGerais.Nivel = dadosPlayer.Nivel;
            dadosGerais.PosicaoPlayer = dadosPlayer.PosicaoPlayer;
        }
 
        NPCSave[] todosInimigos = FindObjectsByType<NPCSave>(FindObjectsSortMode.None);
        dadosGerais.Inimigos = new List<NPCSavesData>();

        foreach (NPCSave inimigo in todosInimigos)
        {
            dadosGerais.Inimigos.Add(inimigo.ObterDados());
        }

        SaveManager.Salvar(dadosGerais);
    }

    public void CarregarJogo()
    {
        SaveData dadosCarregados = SaveManager.Carregar();

        if (dadosCarregados == null)
        {
            return;
        }

        if (playerSaveData != null)
        {
            playerSaveData.AplicarDados(dadosCarregados);
        }

        NPCSave[] todosInimigos = FindObjectsByType<NPCSave>(FindObjectsSortMode.None);
        foreach (NPCSave inimigo in todosInimigos)
        {
            NPCSavesData dadosDesteInimigo = dadosCarregados.Inimigos.Find(x => x.IDdoNPC == inimigo.idUnico);

            if (dadosDesteInimigo != null)
            {
                inimigo.AplicarDados(dadosDesteInimigo);
            }
        }

    }
}