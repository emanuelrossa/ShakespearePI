using UnityEngine;

public class effect : MonoBehaviour
{
    [Header("Duração")]
    public float duracao = 5f;

    [Header("Inclinação")]
    public float intensidadeRotacao = 4f;
    public float velocidadeRotacao = 1.5f;

    [Header("Balanço")]
    public float intensidadeBalanco = 0.08f;
    public float velocidadeBalanco = 1.2f;

    [Header("FOV")]
    public float intensidadeFOV = 8f;
    public float velocidadeFOV = 1f;

    [Header("Transição")]
    public float velocidadeSaida = 2f;

    private float tempo;
    private Camera cam;
    private float fovOriginal;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam != null)
        {
            fovOriginal = cam.fieldOfView;
        }
    }

    void Update()
    {
        tempo += Time.deltaTime;

        float intensidade = 1f;

        if (tempo < duracao)
        {
            // Começa forte e vai diminuindo
            intensidade = 1f - (tempo / duracao);

            // Faz a intensidade não desaparecer tão rápido
            intensidade = Mathf.SmoothStep(0f, 1f, intensidade);

            // Rotação da câmera
            float inclinacao = Mathf.Sin(Time.time * velocidadeRotacao)
                * intensidadeRotacao * intensidade;

            float balancoRotacao = Mathf.Sin(Time.time * velocidadeRotacao * 0.7f)
                * intensidadeRotacao * 0.5f * intensidade;

            transform.localRotation = Quaternion.Euler(
                balancoRotacao,
                0f,
                inclinacao
            );

            // Movimento físico da câmera
            float movimentoX = Mathf.Sin(Time.time * velocidadeBalanco)
                * intensidadeBalanco * intensidade;

            float movimentoY = Mathf.Sin(Time.time * velocidadeBalanco * 0.8f)
                * intensidadeBalanco * 0.6f * intensidade;

            transform.localPosition = new Vector3(
                movimentoX,
                movimentoY,
                transform.localPosition.z
            );

            // FOV pulsando
            if (cam != null)
            {
                float fov = Mathf.Sin(Time.time * velocidadeFOV)
                    * intensidadeFOV * intensidade;

                cam.fieldOfView = fovOriginal + fov;
            }
        }
        else
        {
            // Volta suavemente ao normal
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.identity,
                Time.deltaTime * velocidadeSaida
            );

            Vector3 posicaoOriginal = new Vector3(0f, 0f, transform.localPosition.z);

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                posicaoOriginal,
                Time.deltaTime * velocidadeSaida
            );

            if (cam != null)
            {
                cam.fieldOfView = Mathf.Lerp(
                    cam.fieldOfView,
                    fovOriginal,
                    Time.deltaTime * velocidadeSaida
                );
            }
        }
    }
}