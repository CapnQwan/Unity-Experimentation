using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public int seed;

    public int height;
    public int trunkSegments;
    public float trunkThickness;
    public float trunkThicknessFalloff;
    public float trunkSway;
    public float trunkSwayFalloff;
    [Range(0, 1)]
    public float trunkSwayPersistanct;
    TreeSegment trunk;

    public int branchCount;
    public float branchSpread;
    public float branchDropoff;
    public float branchSegmentDropoff;
    public float branchThicknessFalloff;
    public float branchThicknessSegmentFalloff;
    public int branchSegmentsCount;
    public float branchNoiseHeightPersistance;
    public float branchSegmentNoiseHeightPersistance;
    public float branchSegmentNoisePlanePersistance;
    TreeSegment[] branches;

    public int twigCount;
    public float twigsSpread;
    public float twigsDropoff;
    public float twigsSegmentDropoff;
    public float twigsThicknessFalloff;
    public int twigsSegmentsCount;
    public float twigsNoiseHeightPersistance;
    public float twigsNoisePlanePersistance;
    TreeSegment[] twigs;

    public Vector3 pos = Vector3.zero;

    Mesh mesh;
    public MeshRenderer meshRender;
    public MeshFilter meshFilter;

    void OnValidate()
    {
        if(height <= 0)
        {
            height = 1;
        }

        if (trunkSegments <= 0)
        {
            trunkSegments = 1;
        }

        if (branchSegmentsCount <= 0)
        {
            branchSegmentsCount = 1;
        }

        if (twigsSegmentsCount <= 0)
        {
            twigsSegmentsCount = 1;
        }

        Generate();
    }

    void Generate()
    {
        GenerateTrunk();
        //GenerateBranches();
    }

    void GenerateTrunk()
    {
        float[,] trunkMap = Noise.GenerateRandomNoiseMap(trunkSegments + 1, 3, seed, new Vector2(pos.x, pos.y));
        Vector3[] trunkSpine = new Vector3[trunkSegments + 1];
        float[] trunkThicknesses = new float[trunkSegments + 1];
        float segmentHeight = (float)height / trunkSegments;

        float offset = trunkThickness;
        float xOffset = normalizeNoiseArray(trunkMap[0, 0]);
        float yOffset = 0.0f;
        float zOffset = normalizeNoiseArray(trunkMap[0, 2]);

        trunkSpine[0] = new Vector3(xOffset, yOffset, zOffset);
        trunkThicknesses[0] = offset;

        for (int i = 1; i <= trunkSegments; i++)
        {
            offset = trunkThickness * ((float)(trunkSegments - i) / trunkSegments);
            xOffset = xOffset + trunkSway * (normalizeNoiseArray(trunkMap[i, 0]) * (trunkSwayFalloff / i) * (1.0f - trunkSwayPersistanct)) + trunkSway * (xOffset * (trunkSwayFalloff / i) * trunkSwayPersistanct);
            yOffset = trunkSway * trunkMap[i, 1] * 0.5f;
            zOffset = xOffset + trunkSway * (normalizeNoiseArray(trunkMap[i, 2]) * (trunkSwayFalloff / i) * (1.0f - trunkSwayPersistanct)) + trunkSway * (yOffset * (trunkSwayFalloff / i) * trunkSwayPersistanct);

            trunkSpine[i] = new Vector3(xOffset, segmentHeight * i + yOffset, zOffset);
            trunkThicknesses[i] = offset;
        }

        trunk.partSpine = trunkSpine;
        trunk.partThicknesses = trunkThicknesses;
    }

    void GenerateBranches()
    {
    }

    void GenerateTwigs()
    {
    }

    float normalizeNoiseArray(float noise)
    {
        return (noise * 2.0f - 1.0f);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < trunk.partSpine.Length; i++)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawSphere(trunk.partSpine[i], trunk.partThicknesses[i]);
            if(i != 0)
            {
                Gizmos.DrawLine(trunk.partSpine[i - 1], trunk.partSpine[i]);
            }
        }
    }
}

public struct TreeSegment
{
    public int type;
    public Vector3[] partSpine;
    public float[] partThicknesses;
}