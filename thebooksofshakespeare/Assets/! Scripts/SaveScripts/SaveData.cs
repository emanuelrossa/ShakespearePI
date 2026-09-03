using System.Collections.Generic;

public class SaveData
{
    // --- DADOS DO PLAYER ---
    public string NomeJogador;
    public int Nivel;
    public float[] PosicaoPlayer;

    // --- DADOS DOS INIMIGOS / MUNDO ---
    public List<NPCSavesData> Inimigos = new List<NPCSavesData>();
}

// Estrutura para salvar cada inimigo individualmente
[System.Serializable]
public class NPCSavesData
{
    public string IDdoNPC;
    public float[] Posicao;
    public int VidaAtual;
}