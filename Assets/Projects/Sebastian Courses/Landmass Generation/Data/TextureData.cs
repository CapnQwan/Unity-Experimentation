using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] baseColours;
    [Range(0,1)]
    public float[] baseStartHeights;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        /*
        material.SetFloat("baseColourAHeight", baseStartHeights[0]);
        material.SetFloat("baseColourBHeight", baseStartHeights[0]);
        material.SetColor("baseColourA", baseColours[0]);
        material.SetColor("baseColourB", baseColours[0]);
        */

        material.SetColor("_baseColour", baseColours[0]);
        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        material.SetFloat("_minHeight", minHeight);
        material.SetFloat("_maxHeight", maxHeight);
    }
}
