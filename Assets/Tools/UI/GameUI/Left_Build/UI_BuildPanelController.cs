using UnityEngine;

public class UI_BuildPanelController : MonoBehaviour
{
    [SerializeField] private Transform elementsContainer;


    [SerializeField] private UI_BuildElement[] elements = new UI_BuildElement[3];

    private UI_BuildElement selectedElement;

    private void Awake()
    {

    }

    private void OnDisable()
    {
        selectedElement = null;
    }

    public void OnSelectElement(int index)
    {
        if (selectedElement == null)
        {
            selectedElement = elements[index];
        }
        else
        {
            selectedElement.OnCancelClicked();
            selectedElement = elements[index];
        }
    }
}
