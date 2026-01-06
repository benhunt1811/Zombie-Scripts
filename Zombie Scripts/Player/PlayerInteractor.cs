using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Player Cam")]
    public GameObject fpsCam;

    [Header("Interact Info")]
    private Interactable interactableObject;
    [SerializeField] public LayerMask layer;
    [SerializeField] private float interactRange;

    // Update is called once per frame
    void Update()
    {
        CheckForInteractable();
    }

    public void InteractInput()
    {
        if (interactableObject != null)
        {
            interactableObject.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Vector3 startPos = fpsCam.transform.position;
        Vector3 direction = fpsCam.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        // Layer means only checks for objects with that layer, eg player
        if (Physics.Raycast(startPos, direction, out hit, interactRange, layer))
        {
            Debug.DrawRay(startPos, direction * interactRange, Color.green);
            interactableObject = hit.collider.gameObject.GetComponent<Interactable>();
            interactableObject.ShowInteractable();

        }

        else
        {
            Debug.DrawRay(startPos, direction * interactRange, Color.red);
            if (interactableObject != null)
            {
                interactableObject.HideInteractable();
                interactableObject = null;
            }
        }
    }
}
