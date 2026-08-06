using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    [Header("Atributos do Player")]
    public string nome = "Heroi";
    public int nivel = 1;

    public SaveData ObterDados()
    {
        SaveData dados = new SaveData();
        dados.NomeJogador = this.nome;
        dados.Nivel = this.nivel;

        // Salva a posição atual do Player [x, y, z]
        dados.PosicaoPlayer = new float[]
        {
            transform.position.x,
            transform.position.y,
            transform.position.z
        };

        return dados;
    }

    // Método para aplicar os dados vindos do SaveController
    public void AplicarDados(SaveData dados)
    {
        if (dados == null) return;

        this.nome = dados.NomeJogador;
        this.nivel = dados.Nivel;

        // Se houver uma posição salva válida, teleporta o Player
        if (dados.PosicaoPlayer != null && dados.PosicaoPlayer.Length == 3)
        {
            Vector3 novaPosicao = new Vector3(
                dados.PosicaoPlayer[0],
                dados.PosicaoPlayer[1],
                dados.PosicaoPlayer[2]
            );

            // IMPORTANTE PARA O STARTER ASSETS:
            // Desativa o CharacterController antes de mover para a física não travar o personagem!
            CharacterController controller = GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                transform.position = novaPosicao;
                controller.enabled = true;
            }
            else
            {
                transform.position = novaPosicao;
            }
        }
    }
}