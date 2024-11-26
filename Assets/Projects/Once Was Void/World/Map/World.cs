using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField, Range(1, 400)]
    int mapResolution = 50;

    [SerializeField, Range(1, 30)]
    public int renderDistance = 8;

    [SerializeField]
    MapChunk chunkPrefab;

    MapChunk[] chunks;

    public int chunkResolution = 8;


    public int seed;

    public float noiseScale = 5f;
    [Range(1, 30)]
    public int noiseOctaves = 5;
    [Range(0, 10)]
    public float noisePersistance = 0.5f;
    public float noiseLacunarity = 0.5f;
    public Vector2 noiseOffset;

    public int minNoiseHeight = 0;
    public int maxNoiseHeight = 40;
    public int minWorldHeight = 0;
    public int maxWorldHeight = 40;

    public int drawMode = 0;


    GameObject playerCamera;
    Vector3 cameraPos, cameraRotation;
    Vector3 activeChunk = new Vector3(0, 0, 0);

    int activeDirection = 4;

    bool mapDirty = true;


    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.Find("WorldCamera");
        chunks = new MapChunk[renderDistance * (renderDistance + renderDistance - 1)];
    }

    // Update is called once per frame
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

    private void OnValidate()
    {
        mapDirty = true;
    }

    void Generate()
    {
        if(drawMode == 0)
        {


        }
        else if(drawMode == 1)
        {
            GenerateFlatMap();
        }
    }

    void GenerateFlatMap()
    {

        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i] != null)
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
        Vector3 currentChunk = new Vector3(Mathf.Floor(cameraPos.x / 8), 0, Mathf.Floor(cameraPos.z / 8));
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
