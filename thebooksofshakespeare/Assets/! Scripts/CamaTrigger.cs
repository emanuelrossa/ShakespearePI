using UnityEngine;

public class CamaTrigger : MonoBehaviour
{
    // Agora ele puxa o script certo (ShakespeareCutscene)
    public ShakespeareCutscene cutscene;

    private void OnTriggerEnter(Collider other)
    {
        // Se o jogador (Shakespeare) pisar na área
        if (other.CompareTag("Player"))
        {
            if (cutscene != null)
            {
                cutscene.AvisarQueChegouNaCama();
            }
            else
            {
                Debug.LogWarning("Faltou arrastar o GameManager no campo 'Cutscene' do CamaTrigger!");
            }
        }
    }
}