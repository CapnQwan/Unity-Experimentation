using UnityEngine;
using System.Collections;

public static class NoiseFunctionLibrary
{
    public static float[,] generateNoise(int resolution, int seed, float scale, int octaves, float persistance, float lacunarity, float offSetX, float offSetZ, int minNoiseHeight, int maxNoiseHeight, Vector2 offset)
    {
        float[,] noiseMap = new float[resolution, resolution];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float octaveOffsetX = prng.Next(-10000, 10000) + offset.x;
            float octaveOffsety = prng.Next(-10000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(octaveOffsetX, octaveOffsety);
        }

        float halfSize = resolution / 2;

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float amplitude = 1;
                float freqency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = ((x - halfSize) + offSetX) / scale * freqency + octaveOffsets[i].x;
                    float sampleZ = ((z - halfSize) + offSetZ) / scale * freqency + octaveOffsets[i].y;

                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += noiseValue * amplitude;

                    amplitude *= persistance;
                    freqency *= lacunarity;
                }

                noiseHeight = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseHeight);
                noiseMap[x, z] = noiseHeight;
            }
        }
        return noiseMap;
    }
}
