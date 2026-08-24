using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    public float distance = 3f;

    private NPC currentNPC;

    void Update()
    {
        UpdateOutline();

        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsTalking)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(
                new Vector3(
                    Screen.width / 2f,
                    Screen.height / 2f
                )
            );

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                NPC npc = hit.collider.GetComponent<NPC>();

                if (npc != null)
                    npc.Interact();
            }
        }
    }

    void UpdateOutline()
    {
        Ray ray = cam.ScreenPointToRay(
            new Vector3(
                Screen.width / 2f,
                Screen.height / 2f
            )
        );

        NPC newNPC = null;

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            newNPC = hit.collider.GetComponent<NPC>();
        }

        if (newNPC == currentNPC)
            return;

        if (currentNPC != null)
            currentNPC.SetOutline(false);

        currentNPC = newNPC;

        if (currentNPC != null)
            currentNPC.SetOutline(true);
    }
}