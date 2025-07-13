using UnityEngine;

public class Manager_Input : MonoBehaviour
{
    public LayerMask hoverLayer;

    private Camera mainCamera;

    // Keep track of which I_Hoverable we are hovering over
    private I_Hoverable currentHover;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMouseHover();
        HandleMouseInput();
    }

    private void HandleMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hoverLayer))
        {
            if (hit.collider.TryGetComponent<I_Hoverable>(out var hoverable))
            {
                if (currentHover != hoverable)
                {
                    // Notify previous hovered object
                    if (currentHover != null)
                        currentHover.Hover(false);

                    // Notify new hovered object
                    hoverable.Hover(true);
                    currentHover = hoverable;
                }
                return;
            }
        }

        // If no hoverable object was hit, clear previous hover
        if (currentHover != null)
        {
            currentHover.Hover(false);
            currentHover = null;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    private void HandleLeftClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hoverLayer))
        {
            Debug.Log("Left click on: " + hit.collider.gameObject.name);

            if (hit.collider.TryGetComponent<I_Interactable>(out var interactable))
            {
                interactable.Interact();
            }
        }
    }

    private void HandleRightClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hoverLayer))
        {
            Debug.Log("Right click on: " + hit.collider.gameObject.name);
        }
    }
}