using UnityEngine;

public enum noiseTypeEnum
{
    Perlin,
    Fractal,
    Ridged
}

[CreateAssetMenu(fileName = "NewNoiseProfile", menuName = "Hex Map/New Noise Profile")]
public class NoiseProfile : ScriptableObject
{
    [Tooltip("Selects the algorithm used to generate noise. Perlin creates smooth patterns, Fractal adds detail, Ridged produces sharp mountain ridges.")]
    public noiseTypeEnum noiseType = noiseTypeEnum.Perlin;

    [Tooltip("Controls the scale of the noise pattern. Lower values create large smooth shapes; higher values create smaller detailed features.")]
    public float elevationScale = 0.1f;
    // Lower scale = big continents
    // Higher scale = small noisy islands

    [Tooltip("Number of layers (octaves) in fractal noise. More octaves add more detail and complexity.")]
    public int elevationOctaves = 1;
    // 1 octave = simple noise
    // 3+ octaves = rich detail

    [Tooltip("Controls how much each octave contributes to the final noise. Lower persistence makes higher octaves fade out faster.")]
    public float elevationPersistence = 0.5f;
    // Higher persistence = rougher terrain

    [Tooltip("Controls the frequency increase between octaves. Higher lacunarity means each octave has smaller features.")]
    public float elevationLacunarity = 2.0f;
    // 2.0 is typical

    [Tooltip("Scale used for biome selection noise. Controls the size and variation of biomes.")]
    public float biomeScale = 0.1f;
    // Lower = large biome regions
    // Higher = scattered biomes
}
