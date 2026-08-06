using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    public float distance = 3f;

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                NPC npc = hit.collider.GetComponent<NPC>();

                if (npc != null)
                    npc.Interact();
            }
        }
    }
}