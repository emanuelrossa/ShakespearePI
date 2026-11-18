using UnityEngine;

public class CamaTrigger : MonoBehaviour
{
    public JulietaCutscene cutscene;

    private void OnTriggerEnter(Collider other)
    {
        // Se o jogador pisou na área
        if (other.CompareTag("Player"))
        {
            // Avista o script principal que o jogador chegou
            cutscene.AvisarQueChegouNaCama();
        }
    }
}