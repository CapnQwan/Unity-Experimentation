using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    public int resolution;

    float[,] noiseMap;

    public Vector2 offset;

    World world;

    public Renderer textureRender;

    GameObject playerCamera;

    Vector3 cameraPos;

    // Start is called before the first frame update
    void Start()
    {
        textureRender = GetComponent<Renderer>();
        world = GameObject.Find("World").GetComponent<World>();
        playerCamera = GameObject.Find("WorldCamera");

        generateNoise();
    }

    void generateNoise()
    {
        noiseMap = new float[resolution, resolution];
        if (world != null)
        {
            noiseMap = NoiseFunctionLibrary.generateNoise(
                resolution,
                world.seed,
                world.noiseScale,
                world.noiseOctaves,
                world.noisePersistance,
                world.noiseLacunarity,
                0,
                0,
                world.minNoiseHeight,
                world.maxNoiseHeight,
                world.noiseOffset);

            Texture2D texture = new Texture2D(resolution, resolution);

            Color[] colourMap = new Color[resolution * resolution];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    if (x == Mathf.Round(cameraPos.x) && y == Mathf.Round(cameraPos.z))
                    {
                        colourMap[y * resolution + x] = new Color(255, 0, 0);
                    }
                    else
                    {
                        colourMap[y * resolution + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                    }
                }
            }
            texture.SetPixels(colourMap);
            texture.Apply();

            textureRender.material.mainTexture = texture;
        }
    }

    void Update()
    {
        if (playerCamera != null)
        {
            cameraPos = playerCamera.transform.position;
            generateNoise();
        }
    }

}
