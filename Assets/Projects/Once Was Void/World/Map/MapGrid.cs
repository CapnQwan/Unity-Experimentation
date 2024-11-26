using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField]
    ComputeShader computeShader;

    [SerializeField, Range(1, maxResolution)]
    int mapResolution = 1000;

    public int chunkResolution = 8;

    //[SerializeField, Range(1, 100)]
    //int chunkHeight = 50;

    [SerializeField, Range(1, 50)]
    public int renderDistance = 10;

    [SerializeField]
    MapChunk chunkPrefab;

    public float noiseScale = 0.3f;
    public float multitude = 3;
    public float octaves = 1;
    public float persistance = 1;
    public float lacunarity = 1;

    const int maxResolution = 1000;

    GameObject noiseDisplay;

    MapChunk[] chunks;

    Mesh mesh;

    GameObject playerCamera;

    public Vector3 cameraPos, cameraRotation;
    public Vector3 activeChunk = new Vector3(0, 0, 0);


    public int  activeDirection = 4;

    public float noiseXOrg;
    public float noiseYOrg;

    bool mapDirty = true;

    void OnEnable()
    {
        chunks = new MapChunk[renderDistance * (renderDistance + renderDistance - 1)];
    }

    private void Start()
    {
        playerCamera = GameObject.Find("WorldCamera");
    }

    void Update()
    {
        cameraPos = playerCamera.transform.position;
        cameraRotation = playerCamera.transform.eulerAngles;

        CheckPlayerPos();

        if (mapDirty)
        {
            Debug.Log("<color=red>Generating World </color>");
            Generate();
            mapDirty = false;
        }
    }

    void Generate()
    {

        for (int i = 0; i < chunks.Length; i++)
        {
            if(chunks[i] != null)
            {
                Destroy(chunks[i].gameObject);
            }
        }


        chunks = new MapChunk[renderDistance * (renderDistance + renderDistance - 1)];

        Vector3 startLocation = new Vector3(Mathf.Floor(cameraPos.x / 8) * 8, 0, Mathf.Floor(cameraPos.z / 8) * 8);

        Vector3 primaryIncrament = new Vector3(0, 0, 0);
        Vector3 LeftIncrament = new Vector3(0, 0, 0);
        Vector3 RightIncrament = new Vector3(0, 0, 0);

        switch (activeDirection)
        {
            case 4:
            case -4:
                primaryIncrament = new Vector3(0, 0, 1);
                LeftIncrament = new Vector3(1, 0, 0);
                RightIncrament = new Vector3(-1, 0, 0);
                break;
            case -1:
                primaryIncrament = new Vector3(1, 0, -1);
                LeftIncrament = new Vector3(-1, 0, 0);
                RightIncrament = new Vector3(0, 0, 1);
                break;
            case -2:
                primaryIncrament = new Vector3(1, 0, 0);
                LeftIncrament = new Vector3(0, 0, 1);
                RightIncrament = new Vector3(0, 0, -1);
                break;
            case -3:
                primaryIncrament = new Vector3(1, 0, 1);
                LeftIncrament = new Vector3(-1, 0, 0);
                RightIncrament = new Vector3(0, 0, -1);
                break;
            case 0:
                primaryIncrament = new Vector3(0, 0, -1);
                LeftIncrament = new Vector3(-1, 0, 0);
                RightIncrament = new Vector3(1, 0, 0);
                break;
            case 3:
                primaryIncrament = new Vector3(-1, 0, 1);
                LeftIncrament = new Vector3(1, 0, 0);
                RightIncrament = new Vector3(0, 0, -1);
                break;
            case 2:
                primaryIncrament = new Vector3(-1, 0, 0);
                LeftIncrament = new Vector3(0, 0, -1);
                RightIncrament = new Vector3(0, 0, 1);
                break;
            case 1:
                primaryIncrament = new Vector3(-1, 0, -1);
                LeftIncrament = new Vector3(1, 0, 0);
                RightIncrament = new Vector3(0, 0, 1);
                break;
        }

        // Generate the chunks list based on the currently active chunk
        if (activeDirection % 2 == 0)
        {
            for (int i = 0, chunkIndex = 0; i < renderDistance; i++)
            {
                Vector3 newChunk = (startLocation + (primaryIncrament * i * chunkResolution));
                CreateChunk(newChunk, chunkIndex++);
                for (int n = 1; n < renderDistance; n++)
                {
                    CreateChunk((newChunk + (LeftIncrament * n * chunkResolution)), chunkIndex++);
                    CreateChunk((newChunk + (RightIncrament * n * chunkResolution)), chunkIndex++);
                }
            }
        }
        else
        {
            for (int i = 0, chunkIndex = 0; i < renderDistance; i++)
            {
                Vector3 newChunk = (startLocation + (primaryIncrament * i * chunkResolution));
                CreateChunk(newChunk, chunkIndex++);
                for (int n = 1; n < i * 2 + 1; n++)
                {
                    CreateChunk((newChunk + (LeftIncrament * n * chunkResolution)), chunkIndex++);
                    CreateChunk((newChunk + (RightIncrament * n * chunkResolution)), chunkIndex++);
                }
            }
        }
    }

    private void ChangeEvent()
    {
        Generate();
    }

    void CreateChunk(Vector3 location, int iteration)
    {
        int chunkID = (int)(Mathf.Abs(location.x % mapResolution) + (Mathf.Abs(location.y % mapResolution) * mapResolution));
        int chunkInstance = chunks.Length;
        
        MapChunk chunk = chunks[iteration] = Instantiate(chunkPrefab);
        chunk.transform.SetParent(transform);
        chunk.transform.position = new Vector3(location.x, 0.0f, location.z);
        chunk.name = "Chunk: " + chunkID;
    }

    void CheckPlayerPos()
    {
        Vector3 currentChunk = new Vector3 (Mathf.Floor(cameraPos.x / 8), 0, Mathf.Floor(cameraPos.z / 8));
        if (currentChunk != activeChunk)
        {
            activeChunk = currentChunk;
            mapDirty = true;
        }

        int currentDirection = (int)Mathf.Round((cameraRotation.y - 180) / 45);
        //Debug.Log("<color=red>Direction: </color>" + cameraRotation.y);
        if (currentDirection != activeDirection)
        {
            activeDirection = currentDirection;
            mapDirty = true;
        }
    }
}
