using UnityEngine;
public enum TerrainType
{
    Water,
    Plains,
    Mountain
}
public enum BiomeType
{
    None,        // No biome assigned (e.g., water)
    Grassland,
    Forest,
    Desert,
    Tundra,
    Snow,
    Swamp
}


[CreateAssetMenu(fileName = "BiomeDefinition", menuName = "Hex Map/Biome Definition")]
public class BiomeDefinition : ScriptableObject
{
    public BiomeType biomeType;

    public GameObject inlandPrefab;
    public GameObject shorePrefab;
    public float probability = .5f;

    [Header("Feature Variants")]
    public GameObject[] forestFeatures;
    public GameObject[] mountainFeatures;
    public GameObject[] otherFeatures;
}
