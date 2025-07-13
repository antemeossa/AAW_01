using UnityEngine;

public static class NoiseGenerator
{
    public static float Generate(float x, float z, int seed, NoiseProfile profile)
    {
        switch (profile.noiseType)
        {
            case noiseTypeEnum.Perlin:
                return GeneratePerlin(x, z, seed, profile);
            case noiseTypeEnum.Fractal:
                return GenerateFractal(x, z, seed, profile);
            case noiseTypeEnum.Ridged:
                return GenerateRidged(x, z, seed, profile);
            default:
                Debug.LogWarning("Unknown noise type, returning 0");
                return 0f;
        }
    }

    private static float GeneratePerlin(float x, float z, int seed, NoiseProfile profile)
    {
        return Mathf.PerlinNoise(
            (x + seed) * profile.elevationScale,
            (z + seed) * profile.elevationScale
        );
    }

    private static float GenerateFractal(float x, float z, int seed, NoiseProfile profile)
    {
        float amplitude = 1f;
        float frequency = profile.elevationScale;
        float noiseHeight = 0f;

        for (int i = 0; i < profile.elevationOctaves; i++)
        {
            float nx = (x + seed) * frequency;
            float nz = (z + seed) * frequency;
            float perlin = Mathf.PerlinNoise(nx, nz);
            noiseHeight += perlin * amplitude;

            amplitude *= profile.elevationPersistence;
            frequency *= profile.elevationLacunarity;
        }

        return noiseHeight;
    }

    private static float GenerateRidged(float x, float z, int seed, NoiseProfile profile)
    {
        float amplitude = 1f;
        float frequency = profile.elevationScale;
        float noiseHeight = 0f;

        for (int i = 0; i < profile.elevationOctaves; i++)
        {
            float nx = (x + seed) * frequency;
            float nz = (z + seed) * frequency;
            float perlin = Mathf.PerlinNoise(nx, nz);
            // Ridged noise: 1 - abs(noise - 0.5) * 2
            float ridge = 1f - Mathf.Abs(perlin - 0.5f) * 2f;
            noiseHeight += ridge * amplitude;

            amplitude *= profile.elevationPersistence;
            frequency *= profile.elevationLacunarity;
        }

        return noiseHeight;
    }
}
