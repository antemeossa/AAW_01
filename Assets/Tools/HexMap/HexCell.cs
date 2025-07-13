using TMPro;
using UnityEngine;
using static HexCoordnates;
public enum HexCellType
{
    Water = 0,
    Shore = 1,
    Inland = 2
}

public class HexCell : MonoBehaviour
{
    public TerrainType Terrain;
    public BiomeType Biome;

    [SerializeField]
    private HexCellType cellType;

    public HexCellType CellType
    {
        get { return cellType; }
        set { cellType = value; }
    }

    [SerializeField] private Vector2 coord;

    private HexCoordinates coordinates;
    public HexCoordinates Coordinates
    {
        get { return coordinates; }
        set
        {
            coordinates = value;
            coord.x = coordinates.X; coord.y = coordinates.Z;
        }
    }

    public HexCell[] Neighbors = new HexCell[6];

    public void SetNeighbors(HexCell[] neighbors)
    {
        Neighbors = neighbors;
    }
}
