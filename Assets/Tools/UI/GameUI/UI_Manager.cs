using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private UI_SelectedHex selectedHexPanel;

    private void Start()
    {
        if (GameManager.Instance.uiManager == null)
        {
            GameManager.Instance.uiManager = this;
        }
    }

    public void SwitchHexDetails(bool activate, HexCell cell)
    {
        selectedHexPanel.gameObject.SetActive(activate);
        Vector2Int tmpVec = new Vector2Int(cell.Coordinates.X, cell.Coordinates.Z);
        selectedHexPanel.SetHexPanel(tmpVec, cell.CellType, cell.Biome);
    }


}
