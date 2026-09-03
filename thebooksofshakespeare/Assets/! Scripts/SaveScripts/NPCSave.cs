using UnityEngine;

public class NPCSave : MonoBehaviour
{
    public string idUnico;
    public int vida = 100;

    public NPCSavesData ObterDados()
    {
        return new NPCSavesData
        {
            IDdoNPC = this.idUnico,
            VidaAtual = this.vida,
            Posicao = new float[] { transform.position.x, transform.position.y, transform.position.z }
        };
    }

    public void AplicarDados(NPCSavesData dados)
    {
        if (dados == null) return;

        this.vida = dados.VidaAtual;

        if (this.vida <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = new Vector3(dados.Posicao[0], dados.Posicao[1], dados.Posicao[2]);
    }
}