using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerCrouch : MonoBehaviour
{
    [Header("Crouch")]
    public float crouchHeight = 1.1f;
    public float crouchCameraHeight = 0.9f;
    public float crouchTransitionSpeed = 8f;

    [Header("References")]
    public Transform cameraTarget;

    [Header("Visual (opcional)")]
    [Tooltip("Se o seu player tem um modelo 3D visível separado do CharacterController, arraste ele aqui. Se não tiver, deixa vazio que não muda nada.")]
    public Transform visualModel;
    [Tooltip("Quanto o modelo visual desce/encolhe no eixo Y quando agachado (escala relativa, 1 = tamanho normal).")]
    public float visualCrouchScaleY = 0.6f;

    [Header("Segurança")]
    [Tooltip("Camadas consideradas 'teto' pra checagem antes de levantar.")]
    public LayerMask ceilingCheckMask = ~0;

    private CharacterController controller;

    private float standingHeight;
    private float standingCameraHeight;

    private Vector3 standingCenter;
    private Vector3 visualStandingScale;

    public bool IsCrouching { get; private set; }
    private bool wantsToStandButBlocked;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("PlayerCrouch precisa estar no mesmo GameObject que o CharacterController.");
            enabled = false;
            return;
        }

        // Guarda os valores originais
        standingHeight = controller.height;
        standingCenter = controller.center;

        if (cameraTarget != null)
        {
            standingCameraHeight = cameraTarget.localPosition.y;
        }

        if (visualModel != null)
        {
            visualStandingScale = visualModel.localScale;
        }
    }

    void Update()
    {
        HandleInput();
        HandleCrouch();
    }

    void LateUpdate()
    {
        HandleCamera();
    }

    void HandleInput()
    {
        bool holdingCrouchKey;

#if ENABLE_INPUT_SYSTEM
        holdingCrouchKey = Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed;
#else
        holdingCrouchKey = Input.GetKey(KeyCode.LeftControl);
#endif

        if (holdingCrouchKey)
        {
            IsCrouching = true;
        }
        else if (IsCrouching)
        {
            // Só levanta de verdade se não tiver nada em cima da cabeça
            IsCrouching = !CanStandUp();
            wantsToStandButBlocked = IsCrouching;
        }
    }

    private static Collider[] ceilingCheckResults = new Collider[8];

    bool CanStandUp()
    {
        float radius = Mathf.Max(controller.radius - controller.skinWidth, 0.01f);

        // Só checa o "vão" entre o topo atual (agachado) e o topo em pé —
        // assim não colide com o chão nem com o próprio corpo do player lá embaixo.
        float currentTopY = controller.center.y + controller.height * 0.5f - radius;
        float standingTopY = standingCenter.y + standingHeight * 0.5f - radius;

        Vector3 from = transform.TransformPoint(new Vector3(controller.center.x, currentTopY, controller.center.z));
        Vector3 to = transform.TransformPoint(new Vector3(standingCenter.x, standingTopY, standingCenter.z));

        int hitCount = Physics.OverlapCapsuleNonAlloc(
            from, to, radius, ceilingCheckResults, ceilingCheckMask, QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = ceilingCheckResults[i];
            if (hit == null) continue;

            // Ignora qualquer collider que seja do próprio player
            if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;

            return false; // achou teto de verdade
        }

        return true;
    }

    void HandleCamera()
    {
        if (cameraTarget == null) return;

        float targetCameraY = IsCrouching
            ? crouchCameraHeight
            : standingCameraHeight;

        Vector3 cameraPosition = cameraTarget.localPosition;

        cameraPosition.y = Mathf.Lerp(
            cameraPosition.y,
            targetCameraY,
            crouchTransitionSpeed * Time.deltaTime
        );

        cameraTarget.localPosition = cameraPosition;
    }

    void HandleCrouch()
    {
        float targetHeight = IsCrouching
            ? crouchHeight
            : standingHeight;

        // ==========================================
        // MANTER OS PÉS NO MESMO LUGAR
        // ==========================================

        float feetPosition = standingCenter.y - standingHeight / 2f;

        float targetCenterY = feetPosition + targetHeight / 2f;

        Vector3 targetCenter = standingCenter;
        targetCenter.y = targetCenterY;

        // Altura
        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            crouchTransitionSpeed * Time.deltaTime
        );

        // Centro
        controller.center = Vector3.Lerp(
            controller.center,
            targetCenter,
            crouchTransitionSpeed * Time.deltaTime
        );

        // ==========================================
        // MODELO VISUAL (opcional)
        // ==========================================

        if (visualModel != null)
        {
            Vector3 targetScale = visualStandingScale;
            targetScale.y = visualStandingScale.y * (IsCrouching ? visualCrouchScaleY : 1f);

            visualModel.localScale = Vector3.Lerp(
                visualModel.localScale,
                targetScale,
                crouchTransitionSpeed * Time.deltaTime
            );
        }
    }
}