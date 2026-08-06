using UnityEngine;
using UnityEngine.SceneManagement; // IMPORTANTE: Necessário para mudar de cena

public class ChairInteraction : MonoBehaviour
{
    public Transform cadeira;
    public Vector3 offset = Vector3.zero;
    public string doorScript = "Doors"; 

    private bool perto = false;
    private GameObject player;
    private HeadBob headBobScript;

    private PlayerMoviment playerMoviment;
    private bool sentado = false;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            perto = true;
            player = collision.gameObject;
            playerMoviment = player.GetComponent<PlayerMoviment>();
            headBobScript = player.GetComponentInChildren<HeadBob>();
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            perto = false;
        }
    }

    void Update()
    {
        if (perto && Input.GetKeyDown(KeyCode.E))
        {
            if (!sentado)
            {
                Sentar();
            }
            else
            {
                Levantar();
            }
        }
    }

    void Sentar()
    {
        sentado = true;
        player.transform.position = cadeira.position + offset;
        player.transform.rotation = cadeira.rotation;

        if (playerMoviment != null) playerMoviment.enabled = false;

        if (headBobScript != null) headBobScript.enabled = false;

        if (!string.IsNullOrEmpty(doorScript))
        {
            Debug.Log("Teletransportando para: Doors");
            SceneManager.LoadScene(doorScript);
        }
        
    }

    void Levantar()
    {
        sentado = false;
        if (playerMoviment != null) playerMoviment.enabled = true;
        if (headBobScript != null) headBobScript.enabled = true;
    }
}