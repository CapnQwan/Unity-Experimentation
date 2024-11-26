using System.Collections.Generic;
using UnityEngine;

public class MapChunk : MonoBehaviour
{
    int chunkID;
    int[] voxelIDs;
    float[,] noiseMap;

    World world;

    Mesh voxelMesh;
    MeshCollider meshCollider;

    //float maxNoiseHeight = -40;
    //float minNoiseHeight = 30;

    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;

    Vector3 WorldPos;

    private Renderer rend;

    //bool isDirty = false;
    bool simpleMesh = true;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = voxelMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        voxelMesh.name = "Voxel Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    // Start is called before the first frame update
    void Start()
    {
        world = transform.parent.GetComponent<World>();

        if(voxelMesh != null)
        {
            noiseMap = NoiseFunctionLibrary.generateNoise(
                world.chunkResolution + 1,
                world.seed,
                world.noiseScale,
                world.noiseOctaves,
                world.noisePersistance,
                world.noiseLacunarity,
                transform.localPosition.x,
                transform.localPosition.z,
                world.minNoiseHeight,
                world.maxNoiseHeight,
                world.noiseOffset);

            createMesh();
        }
    }

    void createMesh()
    {
        voxelMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        if (simpleMesh)
        {
            int vert = 0;
            for (int x = 0; x <= world.chunkResolution; x++)
            {
                for (int z = 0; z <= world.chunkResolution; z++)
                {
                    generatePlaneVertices(x, z);
                }
            }
            for (int x = 0; x < world.chunkResolution; x++)
            {
                for (int z = 0; z < world.chunkResolution; z++)
                {
                    generatePlaneTris(vert);
                    vert++;
                }
                vert++;
            }
        }
        else
        {
            for (int x = 0; x < world.chunkResolution; x++)
            {
                for (int z = 0; z < world.chunkResolution; z++)
                {
                    //createVoxels(x, z);
                }
            }
        }

        voxelMesh.vertices = vertices.ToArray();
        voxelMesh.triangles = triangles.ToArray();
        voxelMesh.colors = colors.ToArray();
        voxelMesh.RecalculateNormals();

        //meshCollider.sharedMesh = voxelMesh;
    }

    void createVoxels(int x, int z)
    {
        float y = Mathf.Lerp(0, 40, noiseMap[x, z]);
        float yx = Mathf.Lerp(0, 40, noiseMap[x + 1, z]);
        float yz = Mathf.Lerp(0, 40, noiseMap[x, z + 1]);

        float color = noiseMap[x, z];
        float xColor = noiseMap[x + 1, z];
        float zColor = noiseMap[x, z + 1];

        int vertexIndex = vertices.Count;

        vertices.Add(new Vector3(x, y, z));
        vertices.Add(new Vector3(x + 1, y, z));
        vertices.Add(new Vector3(x, y, z + 1));
        vertices.Add(new Vector3(x + 1, y, z + 1));

        colors.Add(new Color(0.0f, color, 0.0f, 1.0f));
        colors.Add(new Color(0.0f, color, 0.0f, 1.0f));
        colors.Add(new Color(0.0f, color, 0.0f, 1.0f));
        colors.Add(new Color(0.0f, color, 0.0f, 1.0f));

        vertices.Add(new Vector3(x + 1, yx, z));
        vertices.Add(new Vector3(x + 1, yx, z + 1));
        vertices.Add(new Vector3(x, yz, z + 1));
        vertices.Add(new Vector3(x + 1, yz, z + 1));

        colors.Add(new Color(0.0f, xColor, 0.0f, 1.0f));
        colors.Add(new Color(0.0f, xColor, 0.0f, 1.0f));
        colors.Add(new Color(0.0f, zColor, 0.0f, 1.0f));
        colors.Add(new Color(0.0f, zColor, 0.0f, 1.0f));

        triangles.Add(vertexIndex + 0);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);

        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 4);
        triangles.Add(vertexIndex + 4);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 5);

        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 6);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 6);
        triangles.Add(vertexIndex + 7);
    }

    void generatePlaneVertices(int x, int z)
    {
        float y = Mathf.Lerp(world.minWorldHeight, world.maxWorldHeight, noiseMap[x, z]);
        float color = noiseMap[x, z];

        vertices.Add(new Vector3(x, y, z));

        if(color < 0.1f)
        {
            colors.Add(new Color(0.0f, 0.0f, 0.8f, 1.0f));
        }
        else if (color > 0.9f)
        {
            colors.Add(new Color(0.9f, 0.9f, 0.9f, 1.0f));
        }
        else if (color > 0.7f)
        {
            colors.Add(new Color(0.3f, 0.3f, 0.3f, 1.0f));
        }
        else
        {
            colors.Add(new Color(0.0f, 1.0f - color, 0.0f, 1.0f));
        }
    }

    void generatePlaneTris(int vert)
    {
        triangles.Add(vert + world.chunkResolution + 1);
        triangles.Add(vert);
        triangles.Add(vert + 1);
        triangles.Add(vert + 1);
        triangles.Add(vert + world.chunkResolution + 2);
        triangles.Add(vert + world.chunkResolution + 1);
    }

    void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /*private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.ToArray().Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(new Vector3(vertices[i].x + transform.position.x, vertices[i].y + transform.position.y, vertices[i].z + transform.position.z), 0.05f);
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawRay(vertices[i], normals[i]);
        }
    }*/
}
