using System.Collections.Generic;

public static class BiomeMetrics
{
    public const float MountainThreshold = 0.8f; // You can tweak this


    // Polar & Tundra thresholds
    public const float PolarCoreThreshold = 0.05f;
    public const float TundraBandThreshold = 0.15f;
    public const float TundraFadeThreshold = 0.25f;

    // Desert band
    public const float DesertBandMin = 0.4f;
    public const float DesertBandMax = 0.6f;

    // Probabilities
    public const float TundraSnowProbability = 0.3f;
    public const float TundraFadeProbability = 0.4f;

    // Smoothing
    public const float LatitudeNoiseAmplitude = 0.1f;

    // Biome noise scales
    public static readonly Dictionary<BiomeType, float> BiomeScales = new Dictionary<BiomeType, float>
    {
        { BiomeType.Desert, 0.1f },
        { BiomeType.Grassland, 0.2f },
        { BiomeType.Forest, 0.3f },
        { BiomeType.Tundra, 0.15f },
        { BiomeType.Snow, 0.05f }
    };
}
