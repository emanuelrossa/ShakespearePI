using UnityEngine;

public class PassosPlayer : MonoBehaviour
{
    [Header("Configuração de Áudio")]
    public AudioSource audioSource;
    public AudioClip[] somPassos;
    [Range(0.1f, 1f)] public float intervaloPasso = 0.5f;

    private float contadorPasso;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (characterController == null) return;

        bool estaMovendo = characterController.isGrounded && characterController.velocity.magnitude > 0.1f;

        if (estaMovendo)
        {
            contadorPasso -= Time.deltaTime;

            if (contadorPasso <= 0f)
            {
                TocarSomPasso();
                contadorPasso = intervaloPasso;
            }
        }
        else
        {
            contadorPasso = 0f;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void TocarSomPasso()
    {
        if (somPassos == null || somPassos.Length == 0 || audioSource == null) return;

        int indiceAleatorio = Random.Range(0, somPassos.Length);
        audioSource.clip = somPassos[indiceAleatorio];
        audioSource.Play();
    }
}