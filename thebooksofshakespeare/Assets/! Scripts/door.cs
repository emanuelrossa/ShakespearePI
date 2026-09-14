using UnityEngine;
using UnityEngine.SceneManagement;

public class door : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Nome da cena para a qual o jogador irá")]
    public string nomeDaCena;

    [Tooltip("Tecla usada para interagir com a porta")]
    public KeyCode teclaInteracao = KeyCode.F;

    private bool jogadorPerto = false;

    private void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(teclaInteracao))
        {
            SceneManager.LoadScene(nomeDaCena);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
        
        }
    }
}