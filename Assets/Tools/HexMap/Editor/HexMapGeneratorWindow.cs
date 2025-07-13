using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static HexCoordnates;

public class HexMapGeneratorWindow : EditorWindow
{
    private int gridWidth = 5;
    private int gridHeight = 5;

    private GameObject waterPrefab;
    private GameObject mountainPrefab;

    [SerializeField]
    private List<BiomeDefinition> biomeDefinitions = new List<BiomeDefinition>();

    private Transform parentContainer;

    private float elevationScale = 0.1f;
    private int elevationOctaves = 1;
    private float elevationPersistence = 0.5f;
    private float elevationLacunarity = 2.0f;

    private float biomeScale = 0.1f;

    private float mountainScale = 0.1f;
    private int mountainSeed = 0;
    private bool randomizeMountainSeed = true;
    private float mountainThreshold = 0.6f;

    private float waterThreshold = 0.4f;

    private bool randomizeSeed = true;
    private int elevationSeed = 0;
    private int biomeSeed = 0;

    [MenuItem("Tools/Hex Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<HexMapGeneratorWindow>("Hex Map Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Size", EditorStyles.boldLabel);
        gridWidth = EditorGUILayout.IntField("Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Height", gridHeight);

        GUILayout.Space(10);
        GUILayout.Label("Prefabs", EditorStyles.boldLabel);
        waterPrefab = (GameObject)EditorGUILayout.ObjectField("Water Prefab", waterPrefab, typeof(GameObject), false);
        mountainPrefab = (GameObject)EditorGUILayout.ObjectField("Mountain Prefab", mountainPrefab, typeof(GameObject), false);


        SerializedObject so = new SerializedObject(this);
        SerializedProperty biomesProp = so.FindProperty("biomeDefinitions");
        EditorGUILayout.PropertyField(biomesProp, true);
        so.ApplyModifiedProperties();

        parentContainer = (Transform)EditorGUILayout.ObjectField("Parent Container", parentContainer, typeof(Transform), true);

        GUILayout.Space(10);
        GUILayout.Label("Noise Settings", EditorStyles.boldLabel);
        elevationScale = EditorGUILayout.FloatField("Elevation Scale", elevationScale);
        elevationOctaves = EditorGUILayout.IntSlider("Elevation Octaves", elevationOctaves, 1, 6);
        elevationPersistence = EditorGUILayout.Slider("Elevation Persistence", elevationPersistence, 0f, 1f);
        elevationLacunarity = EditorGUILayout.Slider("Elevation Lacunarity", elevationLacunarity, 1f, 3f);

        GUILayout.Space(10);
        GUILayout.Label("Mountain Noise Settings", EditorStyles.boldLabel);
        mountainScale = EditorGUILayout.FloatField("Mountain Scale", mountainScale);
        mountainThreshold = EditorGUILayout.Slider("Mountain Threshold", mountainThreshold, 0f, 1f);

        randomizeMountainSeed = EditorGUILayout.Toggle("Randomize Mountain Seed", randomizeMountainSeed);
        if (!randomizeMountainSeed)
        {
            mountainSeed = EditorGUILayout.IntField("Mountain Seed", mountainSeed);
        }


        biomeScale = EditorGUILayout.FloatField("Biome Scale", biomeScale);

        waterThreshold = EditorGUILayout.Slider("Water Threshold", waterThreshold, 0f, 1f);

        GUILayout.Space(5);
        randomizeSeed = EditorGUILayout.Toggle("Randomize Seeds", randomizeSeed);
        if (!randomizeSeed)
        {
            elevationSeed = EditorGUILayout.IntField("Elevation Seed", elevationSeed);
            biomeSeed = EditorGUILayout.IntField("Biome Seed", biomeSeed);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Map"))
        {
            GenerateMap();
        }

        if (GUILayout.Button("Reset Map"))
        {
            ResetMap();
        }
    }

    private void GenerateMap()
    {
        if (waterPrefab == null)
        {
            Debug.LogError("Water prefab is required.");
            return;
        }
        if (biomeDefinitions == null || biomeDefinitions.Count == 0)
        {
            Debug.LogError("Please assign at least one BiomeDefinition.");
            return;
        }

        if (randomizeSeed)
        {
            elevationSeed = Random.Range(0, 100000);
            biomeSeed = Random.Range(0, 100000);
        }

        if (randomizeMountainSeed)
        {
            mountainSeed = Random.Range(0, 100000);
        }


        ResetMap();

        Transform container = new GameObject("Hex Grid").transform;
        if (parentContainer != null)
            container.SetParent(parentContainer);

        Dictionary<Vector2Int, TerrainType> terrainDict = new Dictionary<Vector2Int, TerrainType>();
        Dictionary<Vector2Int, BiomeType> biomeDict = new Dictionary<Vector2Int, BiomeType>();
        HexCell[,] cells = new HexCell[gridWidth, gridHeight];
        Dictionary<Vector2Int, HexCell> localDict = new Dictionary<Vector2Int, HexCell>();

        int localX = 0;
        int localZ = 0;
        int offsetX = 0;

        // Step 1: Generate elevation and biome noise
        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                float elevationNoise = GenerateFractalNoise(x, z, elevationSeed, elevationScale, elevationOctaves, elevationPersistence, elevationLacunarity);

                TerrainType terrain;
                BiomeType biome = BiomeType.None;

                if (elevationNoise < waterThreshold)
                {
                    terrain = TerrainType.Water;
                }
                else
                {
                    // Not water: check mountain noise
                    float mountainValue = Mathf.PerlinNoise(
                        (x + mountainSeed) * mountainScale,
                        (z + mountainSeed) * mountainScale
                    );

                    if (mountainValue >= mountainThreshold)
                    {
                        terrain = TerrainType.Mountain;
                        biome = BiomeType.None; // or still assign biome if you want
                    }
                    else
                    {
                        terrain = TerrainType.Plains;
                    }


                    float latitude = (float)z / gridHeight;

                    // Precompute noise once
                    float noise = Mathf.PerlinNoise(
                        (x + biomeSeed) * biomeScale * 0.5f,
                        (z + biomeSeed) * biomeScale * 0.5f
                    );

                    float adjustedLatitude = latitude + (noise - 0.5f) * BiomeMetrics.LatitudeNoiseAmplitude;

                    if (adjustedLatitude <= BiomeMetrics.PolarCoreThreshold || adjustedLatitude >= 1f - BiomeMetrics.PolarCoreThreshold)
                    {
                        biome = BiomeType.Snow;
                    }
                    else if (adjustedLatitude <= BiomeMetrics.TundraBandThreshold || adjustedLatitude >= 1f - BiomeMetrics.TundraBandThreshold)
                    {
                        biome = (noise < BiomeMetrics.TundraSnowProbability) ? BiomeType.Snow : BiomeType.Tundra;
                    }
                    else if (adjustedLatitude <= BiomeMetrics.TundraFadeThreshold || adjustedLatitude >= 1f - BiomeMetrics.TundraFadeThreshold)
                    {
                        biome = (noise < BiomeMetrics.TundraFadeProbability) ? BiomeType.Tundra : SelectTemperateBiome(x, z);
                    }
                    else if (adjustedLatitude >= BiomeMetrics.DesertBandMin && adjustedLatitude <= BiomeMetrics.DesertBandMax)
                    {
                        biome = SelectDesertBiome(x, z);
                    }
                    else
                    {
                        biome = SelectTemperateBiome(x, z);
                    }
                }

                terrainDict.Add(new Vector2Int(localX, localZ), terrain);
                biomeDict.Add(new Vector2Int(localX, localZ), biome);


                localX++;
            }

            localZ++;

            if (localZ % 2 == 0)
            {
                offsetX--;
            }

            localX = offsetX;
        }

        int manualX = 0;
        int manualZ = 0;
        int Xoffset = 0;

        // Step 2: Instantiate tiles
        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2Int tmpVec = new Vector2Int(manualX, manualZ);

                if (!terrainDict.ContainsKey(tmpVec))
                {
                    continue;
                }
                TerrainType terrain = terrainDict[tmpVec];
                BiomeType biome = biomeDict[tmpVec];

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
                    var biomeDef = biomeDefinitions.FirstOrDefault(b => b.biomeType == biome);
                    if (biomeDef == null)
                    {
                        Debug.LogError($"No BiomeDefinition found for {biome}");
                        continue;
                    }

                    bool isShore = CheckForWaterNeighbor(manualX, manualZ, terrainDict);
                    selectedPrefab = isShore ? biomeDef.shorePrefab : biomeDef.inlandPrefab;
                }

                if (selectedPrefab == null)
                {
                    Debug.LogError($"Missing prefab for terrain={terrain}, biome={biome}");
                    continue;
                }

                Vector3 pos = GetWorldPosition(x, z);
                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                tile.transform.SetParent(container);
                tile.transform.localPosition = pos;
                tile.name = $"Hex_{manualX}_{manualZ}";

                HexCell cell = tile.GetComponent<HexCell>();
                if (cell != null)
                {
                    cell.Terrain = terrain;
                    cell.Biome = biome;
                    cell.Coordinates = new HexCoordinates(manualX, manualZ);
                }

                cells[x, z] = cell;
                localDict.Add(new Vector2Int(manualX, manualZ), cell);
                manualX++;
            }

            manualZ++;
            if (manualZ % 2 == 0)
            {
                Xoffset--;
            }
            manualX = Xoffset;
        }

        // Step 3: Assign neighbors

        foreach (var cell in localDict)
        {
            HexCell tmpCell = cell.Value;
            if (tmpCell == null)
            {
                continue;
            }

            tmpCell.Neighbors = new HexCell[6];
            Vector2Int[] offsets = GetNeighborOffsets(cell.Key.x, cell.Key.y);

            for (int dir = 0; dir < 6; dir++)
            {
                int nx = cell.Key.x + offsets[dir].x;
                int nz = cell.Key.y + offsets[dir].y;

                Vector2Int tmpVec = new Vector2Int(nx, nz);

                if (localDict.ContainsKey(tmpVec))
                {
                    tmpCell.Neighbors[dir] = localDict[tmpVec];
                }
            }
        }



        Debug.Log("Map generation complete.");
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

    private bool CheckForWaterNeighbor(int x, int z, Dictionary<Vector2Int, TerrainType> terrainTypes)
    {
        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(0, +1),   // NE
            new Vector2Int(+1, 0),   // E
            new Vector2Int(+1, -1),   // SE
            new Vector2Int(0, -1),  // SW
            new Vector2Int(-1, 0),   // W
            new Vector2Int(-1, +1)   // NW
        };

        foreach (var offset in offsets)
        {
            int nx = x + offset.x;
            int nz = z + offset.y;

            Vector2Int tmpVec = new Vector2Int(nx, nz);

            if (terrainTypes.ContainsKey(tmpVec) && terrainTypes[tmpVec] == TerrainType.Water)
            {
                return true;
            }


        }
        return false;
    }

    private void ResetMap()
    {
        GameObject existing = GameObject.Find("Hex Grid");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }
    }

    private float GenerateFractalNoise(float x, float z, int seed, float scale, int octaves, float persistence, float lacunarity)
    {
        float amplitude = 1f;
        float frequency = scale;
        float noiseHeight = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float nx = (x + seed) * frequency;
            float nz = (z + seed) * frequency;
            float perlin = Mathf.PerlinNoise(nx, nz);
            noiseHeight += perlin * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noiseHeight;
    }

    private Vector2Int[] GetNeighborOffsets(int x, int z)
    {

        return new Vector2Int[]
        {
            new Vector2Int(0, +1),   // NE
            new Vector2Int(+1, 0),   // E
            new Vector2Int(+1, -1),   // SE
            new Vector2Int(0, -1),  // SW
            new Vector2Int(-1, 0),   // W
            new Vector2Int(-1, +1)   // NW
        };

    }

    private BiomeType SelectRegularBiome(int x, int z)
    {
        float scale = BiomeMetrics.BiomeScales[BiomeType.Grassland];
        float biomeNoise = Mathf.PerlinNoise(
            (x + biomeSeed) * scale,
            (z + biomeSeed) * scale
        );

        if (biomeNoise < 0.5f)
        {
            var candidate = biomeDefinitions.FirstOrDefault(b => b.biomeType == BiomeType.Desert);
            if (candidate != null && Random.value <= candidate.probability)
            {
                return BiomeType.Desert;
            }
            else
            {
                return BiomeType.Grassland;
            }
        }
        else
        {
            var candidate = biomeDefinitions.FirstOrDefault(b => b.biomeType == BiomeType.Grassland);
            if (candidate != null && Random.value <= candidate.probability)
            {
                return BiomeType.Grassland;
            }
            else
            {
                return BiomeType.Desert;
            }
        }
    }

    private BiomeType SelectTemperateBiome(int x, int z)
    {
        float biomeNoise = Mathf.PerlinNoise(
            (x + biomeSeed) * biomeScale,
            (z + biomeSeed) * biomeScale
        );

        return (biomeNoise < 0.5f) ? BiomeType.Grassland : BiomeType.Forest;
    }

    private BiomeType SelectDesertBiome(int x, int z)
    {
        float biomeNoise = Mathf.PerlinNoise(
            (x + biomeSeed) * biomeScale,
            (z + biomeSeed) * biomeScale
        );

        if (biomeNoise < 0.33f)
            return BiomeType.Desert;
        else if (biomeNoise < 0.66f)
            return BiomeType.Grassland;
        else
            return BiomeType.Forest;
    }

}
