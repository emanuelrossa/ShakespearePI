using UnityEngine;

public class effect : MonoBehaviour
{
    public float intensidade = 2f;
    public float velocidade = 1.5f;
    public float duracao = 5f;

    private float tempo;

    void Update()
    {
        tempo += Time.deltaTime;

        if (tempo < duracao)
        {
            float inclinacao = Mathf.Sin(Time.time * velocidade) * intensidade;
            float balanco = Mathf.Sin(Time.time * velocidade * 0.7f) * intensidade;

            transform.localRotation = Quaternion.Euler(
                balanco,
                0,
                inclinacao
            );
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.identity,
                Time.deltaTime * 2f
            );
        }
    }
}