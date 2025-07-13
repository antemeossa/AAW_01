using TMPro;
using UnityEngine;

public class UI_SelectedHex : MonoBehaviour
{
    [SerializeField] private TMP_Text coordText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text biomeText;

    public void SetHexPanel(Vector2Int coord, HexCellType type, BiomeType biomeType)
    {
        coordText.text = "Hex Coord: (x: " + coord.x + " z: " + coord.y + ")";
        typeText.text = "Hex Type: " + type.ToString();
        biomeText.text = "Biome: " + biomeType.ToString();
    }
}
