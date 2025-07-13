using UnityEngine;

public class HexCell_Interactor : MonoBehaviour, I_Hoverable, I_Interactable
{
    private HexCell hexCell;

    [SerializeField] private GameObject cellHiglighter;

    private bool isSelected = false;

    private void Awake()
    {
        hexCell = transform.parent.GetComponent<HexCell>();
    }

    public void Hover(bool isHovering)
    {
        if (!isSelected)
        {
            cellHiglighter.SetActive(isHovering);
        }
    }

    public void Interact()
    {
        isSelected = true;
        cellHiglighter.SetActive(true);
        GameManager.Instance.uiManager.SwitchHexDetails(true, hexCell);
    }


}
