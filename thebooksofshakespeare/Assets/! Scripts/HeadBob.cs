using UnityEngine;

public partial class HeadBob : MonoBehaviour
{
    [Header("Configurações de Caminhada")]
    [SerializeField] private float walkingBobSpeed = 10f;
    [SerializeField] private float walkingBobAmount = 0.05f;

    [Header("Configurações de Corrida")]
    [SerializeField] private float runningBobSpeed = 16f;
    [SerializeField] private float runningBobAmount = 0.1f;

    [Header("Suavização")]
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Referências")]
    [SerializeField] private CharacterController playerController;

    private float timer = 0f;
    private float defaultPosY = 0f;

    void Start()
    {
        defaultPosY = transform.localPosition.y;
    }

    void Update()
    {
        // --- NOVA VERIFICAÇÃO ---
        // Se o diálogo estiver ativo, forçamos a câmera a voltar para a posição original e paramos o código aqui
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            ResetarPosicao();
            return; // Sai do Update e não executa o balanço abaixo
        }

        // Detecta se o player está se movendo significativamente
        float moveMagnitude = new Vector3(playerController.velocity.x, 0, playerController.velocity.z).magnitude;

        if (moveMagnitude > 0.1f && playerController.isGrounded)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            float currentSpeed = isRunning ? runningBobSpeed : walkingBobSpeed;
            float currentAmount = isRunning ? runningBobAmount : walkingBobAmount;

            timer += Time.deltaTime * currentSpeed;

            float newY = defaultPosY + Mathf.Sin(timer) * currentAmount;

            Vector3 targetPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
        }
        else
        {
            ResetarPosicao();
        }
    }

    // Criamos um método para evitar repetição de código
    void ResetarPosicao()
    {
        timer = 0;
        Vector3 resetPosition = new Vector3(transform.localPosition.x, defaultPosY, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, resetPosition, Time.deltaTime * smoothSpeed);
    }
}