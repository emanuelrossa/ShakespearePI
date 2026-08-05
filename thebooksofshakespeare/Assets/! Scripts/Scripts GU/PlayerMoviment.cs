using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoviment : MonoBehaviour
{
    [Header("Controle de Estado")]
    public bool canControl = true; // Se falso, trava o mouse e o movimento

    [Header("Velocidade")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;

    [Header("Estamina - Configurações")]
    public float maxStamina = 100f;
    public float drainRate = 25f;
    public float timeBeforeRegen = 2f;
    public float regenSpeed = 100f;
    public float fadeSpeed = 5f;

    [Header("Estamina - UI")]
    public float currentStamina;
    public Slider staminaBar;
    public CanvasGroup staminaCanvasGroup;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    public float maxLookAngle = 85f;

    CharacterController controller;
    float xRotation = 0f;
    bool isRunning;
    bool isExhausted = false;
    Coroutine regenCoroutine;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = maxStamina;
        }

        if (staminaCanvasGroup != null) staminaCanvasGroup.alpha = 0f;

        SetCursorState(true);
    }

    void Update()
    {
        // Se canControl for falso (em diálogo), não processa input do mouse nem movimento
        if (!canControl) return;

        HandleMouseLook();
        HandleMovement();
        HandleUIVisibility();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        if (playerCamera != null)
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f);

        if (Input.GetKey(KeyCode.LeftShift) && isMoving && !isExhausted && currentStamina > 0)
        {
            if (!isRunning)
            {
                isRunning = true;
                if (regenCoroutine != null) StopCoroutine(regenCoroutine);
            }

            currentStamina -= drainRate * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
                StopRunning();
            }
        }
        else if (isRunning)
        {
            StopRunning();
        }

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.right * x + transform.forward * z;
        controller.SimpleMove(move.normalized * speed);
    }

    void StopRunning()
    {
        isRunning = false;
        if (regenCoroutine != null) StopCoroutine(regenCoroutine);
        regenCoroutine = StartCoroutine(RegenStaminaDelayed());
    }

    IEnumerator RegenStaminaDelayed()
    {
        yield return new WaitForSeconds(timeBeforeRegen);

        while (currentStamina < maxStamina)
        {
            currentStamina += regenSpeed * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            yield return null;
        }

        isExhausted = false;
    }

    void HandleUIVisibility()
    {
        if (staminaBar == null || staminaCanvasGroup == null) return;

        staminaBar.value = currentStamina;
        float targetAlpha = (isRunning || currentStamina < maxStamina) ? 1f : 0f;
        staminaCanvasGroup.alpha = Mathf.MoveTowards(staminaCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    // Método para ligar/desligar o cursor facilmente
    public void SetCursorState(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }
}