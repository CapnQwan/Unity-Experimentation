using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMesh : MonoBehaviour
{
    Mesh worldMesh;
    MeshCollider meshCollider;

    public enum drawMode
    {
        Cube,
        Sphere
    }

    public bool squareCirlceDemo = false;

    public drawMode currentDrawMode = drawMode.Cube;

    [Range(1, 6)]
    public int activeFace = 1;
    public int resolution = 3;
    public int radius = 10;

    public int meshOffset = 1;

    bool isDirty = false;

    List<Vector3> vertices;
    List<Vector3> circleVects;
    List<Vector3> squareVects;
    List<int> triangles;
    List<Color> colors;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = worldMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        worldMesh.name = "World Mesh";
        vertices = new List<Vector3>();
        circleVects = new List<Vector3>();
        squareVects = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    void Start()
    {
        generateMesh();
    }

    void OnValidate()
    {
        isDirty = true;
    }

    void Update()
    {
        if (isDirty)
        {
            generateMesh();
        }
    }

    void generateMesh()
    {
        worldMesh.Clear();
        vertices.Clear();
        squareVects.Clear();
        circleVects.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < 6; i++)
        {
            int vertexCount = vertices.Count;
            generateFaceVerticies(i, 1);
            if (i % 2 == 0)
            {
                generateFaceTris(vertexCount);
            }
            else
            {
                generateInvertedFaceTris(vertexCount);
            }
        }
        
        vertices.Add(new Vector3(0, 0, 0));
        colors.Add(new Color(0.5f, 0.5f, 0.5f, 1.0f));

        if (squareCirlceDemo)
        {
            generateSquare();
        }
        
        worldMesh.vertices = vertices.ToArray();
        worldMesh.triangles = triangles.ToArray();
        worldMesh.colors = colors.ToArray();
        worldMesh.RecalculateNormals();
    }

    void generateSquare()
    {
        float step = 2f / resolution;
        float halfRad = (float)radius / 2.0f;
        for (int i = 0; i <= resolution; i++)
        {
            createPoint(i * step - 1f, -1f);
            createPoint(i * step - 1f, 1f);
        }
        for (int i = 1; i < resolution; i++)
        {
            createPoint(-1f, i * step - 1f);
            createPoint(1f, i * step - 1f);
        }
    }

    void createPoint(float x, float z)
    {
        Vector3 point = new Vector3(x, 0, z);
        Vector3 circlePoint = Vector3.zero;
        circlePoint.x = (x * Mathf.Sqrt(1f - z * z * 0.5f));
        circlePoint.z = (z * Mathf.Sqrt(1f - x * x * 0.5f));
        point *= radius;
        circlePoint *= radius;
        squareVects.Add(point);
        circleVects.Add(circlePoint);
    }


    void generateFaceVerticies(int face, int lod)
    {
        for (int u = 0; u <= resolution; u++)
        {
            for (int v = 0; v <= resolution; v++)
            {
                generateFaceVertices(face, u, v);
            }
        }
    }

    void generateFaceVertices(int face, int u, int v)
    {
        Vector3 CV = Vector3.zero;
        float resPart = 2f / resolution;
        switch (face)
        {
            case 0:
                CV = new Vector3(-1f + resPart * u, -1f + resPart * v, 1f);
                break;
            case 1:
                CV = new Vector3(-1f + resPart * u, 1f, -1f + resPart * v);
                break;
            case 2:
                CV = new Vector3(1f, -1f + resPart * u, -1f + resPart * v);
                break;
            case 3:
                CV = new Vector3(-1f + resPart * u, -1f + resPart * v, -1f);
                break;
            case 4:
                CV = new Vector3(-1f + resPart * u, -1f, -1f + resPart * v);
                break;
            case 5:
                CV = new Vector3(-1f, -1f + resPart * u, -1f + resPart * v);
                break;
            default:
                break;
        }
        if(currentDrawMode == drawMode.Cube)
        {
            vertices.Add(CV * radius);
            colors.Add(new Color(0.5f, 0.5f, 0.5f, 1.0f));
        }
        else
        {
            float x2 = CV.x * CV.x;
            float y2 = CV.y * CV.y;
            float z2 = CV.z * CV.z;
            Vector3 s;
            s.x = CV.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
            s.y = CV.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
            s.z = CV.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
            s *= radius;
            vertices.Add(s);
            colors.Add(new Color(0.5f, 0.5f, 0.5f, 1.0f));
        }
    }

    void generateFaceTris(int vertexCount)
    {
        for (int u = 0; u < resolution; u++)
        {
            for (int v = 0; v < resolution; v++)
            {
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + resolution + 1);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + resolution + 1);
                triangles.Add(vertexCount + resolution + 2);
                vertexCount++;
            }
            vertexCount++;
        }
    }

    void generateInvertedFaceTris(int vertexCount)
    {
        for (int u = 0; u < resolution; u++)
        {
            for (int v = 0; v < resolution; v++)
            {
                triangles.Add(vertexCount + resolution + 1);
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + resolution + 2);
                triangles.Add(vertexCount + resolution + 1);
                vertexCount++;
            }
            vertexCount++;
        }
    }

    private void OnDrawGizmos()
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
        }
        
        for (int i = 0; i < squareVects.ToArray().Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(new Vector3(squareVects[i].x + transform.position.x, squareVects[i].y + transform.position.y, squareVects[i].z + transform.position.z), 0.05f);
        }
        for (int i = 0; i < circleVects.ToArray().Length; i++)
        {
            Gizmos.color = Color.white;
            Vector3 cPoint = new Vector3(circleVects[i].x + transform.position.x, circleVects[i].y + transform.position.y, circleVects[i].z + transform.position.z);
            Gizmos.DrawSphere(cPoint, 0.05f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(squareVects[i].x + transform.position.x, squareVects[i].y + transform.position.y, squareVects[i].z + transform.position.z), cPoint);

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(cPoint, Vector2.zero);
        }
        
    }
}
