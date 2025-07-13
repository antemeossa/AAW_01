using System.Collections.Generic;
using UnityEngine;
using static HexCoordnates;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    [Header("Prefabs")]
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject mountainPrefab;

    [Header("Biome Settings")]
    [SerializeField] private List<BiomeDefinition> biomeDefinitions = new List<BiomeDefinition>();

    [Header("Noise Profiles")]
    [SerializeField] private NoiseProfile elevationProfile;
    [SerializeField] private NoiseProfile mountainProfile;
    [SerializeField] private NoiseProfile biomeProfile;

    [Header("Thresholds")]
    [SerializeField] private float waterThreshold = 0.4f;
    [SerializeField] private float mountainThreshold = 0.6f;

    private Dictionary<Vector2Int, HexCell> cellsDict = new Dictionary<Vector2Int, HexCell>();
    private Dictionary<Vector2Int, TerrainType> terrainDict = new Dictionary<Vector2Int, TerrainType>();
    private Dictionary<Vector2Int, BiomeType> biomeDict = new Dictionary<Vector2Int, BiomeType>();

    private void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        cellsDict.Clear();
        terrainDict.Clear();
        biomeDict.Clear();

        // First pass: Determine terrain and biomes
        int localX = 0;
        int localZ = 0;
        int offsetX = 0;

        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector2Int pos = new Vector2Int(localX, localZ);

                // Elevation noise
                float elevationNoise = NoiseGenerator.Generate(x, z, 0, elevationProfile);

                TerrainType terrain;
                BiomeType biome = BiomeType.None;

                if (elevationNoise < waterThreshold)
                {
                    terrain = TerrainType.Water;
                }
                else
                {
                    float mountainNoise = NoiseGenerator.Generate(x, z, 0, mountainProfile);

                    if (mountainNoise >= mountainThreshold)
                    {
                        terrain = TerrainType.Mountain;
                    }
                    else
                    {
                        terrain = TerrainType.Plains;

                        float latitude = (float)z / mapHeight;
                        float biomeNoise = NoiseGenerator.Generate(x, z, 0, biomeProfile);

                        if (latitude <= BiomeMetrics.PolarCoreThreshold || latitude >= 1f - BiomeMetrics.PolarCoreThreshold)
                        {
                            biome = BiomeType.Snow;
                        }
                        else if (latitude <= BiomeMetrics.TundraBandThreshold || latitude >= 1f - BiomeMetrics.TundraBandThreshold)
                        {
                            biome = BiomeType.Tundra;
                        }
                        else if (latitude >= BiomeMetrics.DesertBandMin && latitude <= BiomeMetrics.DesertBandMax)
                        {
                            biome = biomeNoise < 0.33f ? BiomeType.Desert :
                                    biomeNoise < 0.66f ? BiomeType.Grassland :
                                    BiomeType.Forest;
                        }
                        else
                        {
                            biome = biomeNoise < 0.5f ? BiomeType.Grassland : BiomeType.Forest;
                        }
                    }
                }

                terrainDict.Add(pos, terrain);
                biomeDict.Add(pos, biome);

                localX++;
            }

            localZ++;
            if (localZ % 2 == 0)
            {
                offsetX -= 1;
            }
            localX = offsetX;
        }

        // Second pass: Instantiate prefabs and set up HexCells
        localX = 0;
        localZ = 0;
        offsetX = 0;

        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector2Int pos = new Vector2Int(localX, localZ);

                TerrainType terrain = terrainDict[pos];
                BiomeType biome = biomeDict[pos];

                GameObject selectedPrefab = null;

                if (terrain == TerrainType.Water)
                {
                    selectedPrefab = waterPrefab;
                }
                else if (terrain == TerrainType.Mountain)
                {
                    selectedPrefab = mountainPrefab;
                }
                else if (terrain == TerrainType.Plains)
                {
                    bool isShore = CheckForWaterNeighbor(pos);

                    var biomeDef = biomeDefinitions.Find(b => b.biomeType == biome);
                    if (biomeDef != null)
                    {
                        selectedPrefab = isShore ? biomeDef.shorePrefab : biomeDef.inlandPrefab;
                    }
                }

                if (selectedPrefab == null)
                {
                    Debug.LogWarning($"No prefab found for terrain={terrain}, biome={biome}");
                    localX++;
                    continue;
                }

                Vector3 worldPos = GetWorldPosition(x, z);
                GameObject tileObj = Instantiate(selectedPrefab, worldPos, Quaternion.identity, transform);
                tileObj.name = $"Hex_{localX}_{localZ}";

                HexCell cell = tileObj.GetComponent<HexCell>();
                if (cell != null)
                {
                    cell.Terrain = terrain;
                    cell.Biome = biome;
                    cell.Coordinates = new HexCoordinates(localX, localZ);
                }

                cellsDict.Add(pos, cell);

                localX++;
            }

            localZ++;
            if (localZ % 2 == 0)
            {
                offsetX -= 1;
            }
            localX = offsetX;
        }

        // Finally assign neighbors
        foreach (var kv in cellsDict)
        {
            SetNeighbors(kv.Key, kv.Value);
        }

        Debug.Log("Map generation complete.");
    }


    private bool CheckForWaterNeighbor(Vector2Int pos)
    {
        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(0, +1),   // NE
            new Vector2Int(+1, 0),   // E
            new Vector2Int(+1, -1),  // SE
            new Vector2Int(0, -1),   // SW
            new Vector2Int(-1, 0),   // W
            new Vector2Int(-1, +1)   // NW
        };

        foreach (var offset in offsets)
        {
            Vector2Int neighborPos = pos + offset;
            if (terrainDict.ContainsKey(neighborPos))
            {
                if (terrainDict[neighborPos] == TerrainType.Water)
                {
                    return true;
                }
            }

        }
        return false;
    }

    private void SetNeighbors(Vector2Int cellCoord, HexCell cell)
    {
        Vector2Int[] offsets = new Vector2Int[]
       {
            new Vector2Int(0, +1),   // NE
            new Vector2Int(+1, 0),   // E
            new Vector2Int(+1, -1),  // SE
            new Vector2Int(0, -1),   // SW
            new Vector2Int(-1, 0),   // W
            new Vector2Int(-1, +1)   // NW
       };

        HexCell[] tmpNeighbors = new HexCell[6];

        int counter = 0;

        foreach (var offset in offsets)
        {
            if (cellsDict.ContainsKey(cellCoord + offset))
            {
                tmpNeighbors[counter] = cellsDict[cellCoord + offset];

            }

            counter++;
        }

        cell.SetNeighbors(tmpNeighbors);
    }
    private Vector3 GetWorldPosition(int x, int z)
    {
        float innerRadius = HexMetrics.innerRadius;
        float outerRadius = HexMetrics.outerRadius;
        float innerDiameter = innerRadius * 2f;

        float xOffset = (z & 1) == 0 ? 0f : innerRadius;
        float xPos = (x * innerDiameter) + xOffset;
        float zPos = z * outerRadius * 1.5f;

        return new Vector3(xPos, 0, zPos);
    }
}
