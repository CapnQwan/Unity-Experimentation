/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMeshGenerator : MonoBehaviour
{
    public int seed;

    public int height;
    public float trunkThickness;
    public float trunkThicknessFalloff;
    public float trunkSegmentFalloff;
    public float trunkSway;
    public int trunkSegments;
    Vector3[] trunkVertices;
    Vector3[] trunkSpine;
    float[] trunkThicknesses;

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


    Vector3[] twigVertices;

    Vector3[] vertices;
    int[] tris;

    float[,] noiseMap;

    void OnValidate()
    {
        if (height <= 0)
        {
            height = 1;
        }
        if (trunkSegments <= 0)
        {
            trunkSegments = 0;
        }

        if (branchCount <= 0)
        {
            branchCount = 1;
        }
        if (branchSpread < 0f)
        {
            branchSpread = 0f;
        }
        if (branchSegmentsCount <= 0)
        {
            branchSegmentsCount = 1;
        }

        if (twigCount < 0)
        {
            twigCount = 0;
        }
        if (twigsSpread < 0f)
        {
            twigsSpread = 0f;
        }
        if (twigsSegmentsCount <= 0)
        {
            twigsSegmentsCount = 1;
        }

        Generate();
    }

    void Generate()
    {
        tris = new int[(trunkSegments * 24) + (branchSegmentsCount * 24 * branchCount) + (twigsSegmentsCount * 24 * twigCount)];
        vertices = new Vector3[((trunkSegments + 1) * 4) + ((branchSegmentsCount + 1) * 4 * branchCount) + ((twigsSegmentsCount + 1) * 4 * twigCount)];

        GenerateTrunk();
        GenerateBranches();
        GenerateTree();
    }

    void GenerateTrunk()
    {
        float[,] trunkMap = Noise.GenerateRandomNoiseMap(trunkSegments + 1, 3, seed, new Vector2(pos.x, pos.y));
        trunkSpine = new Vector3[trunkSegments + 1];
        trunkThicknesses = new float[trunkSegments + 1];
        trunkVertices = new Vector3[trunkSegments * 4 + 4];
        float segmentHeight = (float)height / trunkSegments;

        for (int i = 0; i <= trunkSegments; i++)
        {
            float offset = trunkThickness - ((float)trunkThicknessFalloff * i * segmentHeight * 0.25f) - ((float)trunkSegmentFalloff * i * i * 0.05f);
            float xOffset = trunkSway * trunkMap[i, 0];
            float yOffset;
            if (i == 0)
            {
                yOffset = 0f;
            }
            else
            {
                yOffset = trunkSway * trunkMap[i, 1] * 0.5f;
            }
            float zOffset = trunkSway * trunkMap[i, 2];

            trunkSpine[i] = new Vector3(xOffset, segmentHeight * i + yOffset, zOffset);
            trunkThicknesses[i] = offset;

            vertices[i * 4 + 0] = new Vector3(offset + xOffset, segmentHeight * i + yOffset, -offset + zOffset);
            vertices[i * 4 + 1] = new Vector3(offset + xOffset, segmentHeight * i + yOffset, offset + zOffset);
            vertices[i * 4 + 2] = new Vector3(-offset + xOffset, segmentHeight * i + yOffset, offset + zOffset);
            vertices[i * 4 + 3] = new Vector3(-offset + xOffset, segmentHeight * i + yOffset, -offset + zOffset);
        }

        *//*        for (int i = 0; i < trunkSegments; i++)
                {
                    tris[i * 24 + 0] = 4 * i + 0;
                    tris[i * 24 + 1] = 4 * i + 4;
                    tris[i * 24 + 2] = 4 * i + 1;
                    tris[i * 24 + 3] = 4 * i + 1;
                    tris[i * 24 + 4] = 4 * i + 4;
                    tris[i * 24 + 5] = 4 * i + 5;

                    tris[i * 24 + 6] = 4 * i + 1;
                    tris[i * 24 + 7] = 4 * i + 5;
                    tris[i * 24 + 8] = 4 * i + 2;
                    tris[i * 24 + 9] = 4 * i + 2;
                    tris[i * 24 + 10] = 4 * i + 5;
                    tris[i * 24 + 11] = 4 * i + 6;

                    tris[i * 24 + 12] = 4 * i + 2;
                    tris[i * 24 + 13] = 4 * i + 6;
                    tris[i * 24 + 14] = 4 * i + 3;
                    tris[i * 24 + 15] = 4 * i + 3;
                    tris[i * 24 + 16] = 4 * i + 6;
                    tris[i * 24 + 17] = 4 * i + 7;

                    tris[i * 24 + 18] = 4 * i + 0;
                    tris[i * 24 + 19] = 4 * i + 3;
                    tris[i * 24 + 20] = 4 * i + 4;
                    tris[i * 24 + 21] = 4 * i + 4;
                    tris[i * 24 + 22] = 4 * i + 3;
                    tris[i * 24 + 23] = 4 * i + 7;
                }*//*
    }

    void GenerateBranches()
    {
        branches = new TreeSegment[branchCount];

        for (int branch = 0; branch < branchCount; branch++)
        {
            Vector3[] branchSpine = new Vector3[branchSegmentsCount + 1];
            Vector3[] branchVerticies = new Vector3[(branchSegmentsCount + 1) * 4];
            float[,] branchMap = Noise.GenerateRandomNoiseMap(branchSegmentsCount + 1, 3, seed, new Vector2(pos.x + trunkSegments + branch, pos.y + trunkSegments + branch));

            float trunkPoint = trunkSegments * branchMap[0, 0];

            Vector3 A = trunkSpine[Mathf.CeilToInt(trunkPoint)];
            Vector3 B = trunkSpine[Mathf.CeilToInt(trunkPoint) - 1];
            Vector3 AB = B - A;

            branchSpine[0] = A + ((trunkPoint - Mathf.FloorToInt(trunkPoint)) * AB);

            AB.Normalize();

            for (int segment = 1; segment <= branchSegmentsCount; segment++)
            {
                Vector3 previousBranch = branchSpine[segment - 1];

                float xSpread;
                float ySpread;

                float baseBranchSpread = branchSpread / branchSegmentsCount;

                xSpread = baseBranchSpread + (baseBranchSpread * ((branchMap[segment, 1]) * branchSegmentNoisePlanePersistance));
                ySpread = baseBranchSpread + (baseBranchSpread * ((branchMap[segment, 2]) * branchSegmentNoisePlanePersistance));

                float yOffset = previousBranch.y + (branchDropoff * (branchMap[0, 0] * branchNoiseHeightPersistance)) + (branchSegmentDropoff * segment * (branchMap[segment, 0] * branchNoiseHeightPersistance));
                float xOffset = previousBranch.x + (xSpread * (branchMap[0, 1] * 2.0f - 1.0f));
                float zOffset = previousBranch.z + (ySpread * (branchMap[0, 2] * 2.0f - 1.0f));

                branchSpine[segment] = new Vector3(xOffset, yOffset, zOffset);
            }

            branches[branch].partSpine = branchSpine;


            float initialBranchThickness;

            if (trunkThicknesses[Mathf.CeilToInt(trunkPoint)] > trunkThicknesses[Mathf.CeilToInt(trunkPoint) - 1])
            {
                initialBranchThickness = trunkThicknesses[Mathf.CeilToInt(trunkPoint) - 1];
            }
            else
            {
                initialBranchThickness = trunkThicknesses[Mathf.CeilToInt(trunkPoint)];
            }

            Vector3 between = branchSpine[0] - branchSpine[1];
            Vector3 leftPoint = Vector3.Cross(Vector3.down, between);
            Vector3 rightPoint = Vector3.Cross(Vector3.up, between);
            Vector3 upPoint = Vector3.Cross(Vector3.forward, between);
            Vector3 downPoint = Vector3.Cross(Vector3.back, between);

            leftPoint.Normalize();
            rightPoint.Normalize();
            upPoint.Normalize();
            downPoint.Normalize();

            int vertOffset = ((trunkSegments + 1) * 4) + ((branchSegmentsCount + 1) * 4 * branch);

            vertices[vertOffset + 0] = leftPoint + branchSpine[0];
            vertices[vertOffset + 2] = rightPoint + branchSpine[0];
            vertices[vertOffset + 1] = upPoint + branchSpine[0];
            vertices[vertOffset + 3] = downPoint + branchSpine[0];

            for (int i = 1; i < branchSpine.Length; i++)
            {
                float branchThickness = initialBranchThickness;

                between = branchSpine[i] - branchSpine[i - 1];
                leftPoint = Vector3.Cross(Vector3.down, between);
                rightPoint = Vector3.Cross(Vector3.up, between);
                upPoint = Vector3.Cross(Vector3.forward, between);
                downPoint = Vector3.Cross(Vector3.back, between);

                leftPoint.Normalize();
                rightPoint.Normalize();
                upPoint.Normalize();
                downPoint.Normalize();

                vertices[vertOffset + i * 4 + 1] = leftPoint * branchThickness + branchSpine[i];
                vertices[vertOffset + i * 4 + 3] = rightPoint * branchThickness + branchSpine[i];
                vertices[vertOffset + i * 4 + 2] = upPoint * branchThickness + branchSpine[i];
                vertices[vertOffset + i * 4 + 0] = downPoint * branchThickness + branchSpine[i];
            }


            for (int i = 0; i < branchSegmentsCount; i++)
            {
                int triOffset = (trunkSegments * 24) + (branch * 24 * branchSegmentsCount) + (i * 24);
                vertOffset = ((trunkSegments + 1) * 4) + ((branchSegmentsCount + 1) * 4 * branch) + (i * 4);
                tris[triOffset + 0] = vertOffset + 0;
                tris[triOffset + 2] = vertOffset + 1;
                tris[triOffset + 3] = vertOffset + 1;
                tris[triOffset + 1] = vertOffset + 4;
                tris[triOffset + 4] = vertOffset + 4;
                tris[triOffset + 5] = vertOffset + 5;

                tris[triOffset + 6] = vertOffset + 1;
                tris[triOffset + 7] = vertOffset + 5;
                tris[triOffset + 8] = vertOffset + 2;
                tris[triOffset + 9] = vertOffset + 2;
                tris[triOffset + 10] = vertOffset + 5;
                tris[triOffset + 11] = vertOffset + 6;

                tris[triOffset + 12] = vertOffset + 2;
                tris[triOffset + 13] = vertOffset + 6;
                tris[triOffset + 14] = vertOffset + 3;
                tris[triOffset + 15] = vertOffset + 3;
                tris[triOffset + 16] = vertOffset + 6;
                tris[triOffset + 17] = vertOffset + 7;

                tris[triOffset + 18] = vertOffset + 0;
                tris[triOffset + 19] = vertOffset + 3;
                tris[triOffset + 20] = vertOffset + 4;
                tris[triOffset + 21] = vertOffset + 4;
                tris[triOffset + 22] = vertOffset + 3;
                tris[triOffset + 23] = vertOffset + 7;
            }

            branches[branch].treeParts = branchVerticies;
            branches[branch].partSpine = branchSpine;
        }


    }

    void GenerateTree()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        //

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if (trunkVertices == null)
        {
            return;
        }

        for (int i = 0; i < trunkSpine.Length; i++)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawSphere(trunkSpine[i], 0.02f);
            if (i != 0)
            {
                Gizmos.DrawLine(trunkSpine[i - 1], trunkSpine[i]);
            }
        }

        for (int i = 0; i < branches.Length; i++)
        {
            for (int n = 0; n < branches[i].partSpine.Length; n++)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawSphere(branches[i].partSpine[n], 0.03f);
                if (n != 0)
                {
                    Gizmos.DrawLine(branches[i].partSpine[n - 1], branches[i].partSpine[n]);
                }
            }
        }

        for (int i = 0; i < branches.Length; i++)
        {
            for (int n = 0; n < branches[i].treeParts.Length; n++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(branches[i].treeParts[n], 0.03f);
                if (n != 0)
                {
                    //Gizmos.DrawLine(branches[i].treeParts[n - 1], branches[i].treeParts[n]);
                }
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(vertices[i], 0.05f);
        }
    }
}
*/