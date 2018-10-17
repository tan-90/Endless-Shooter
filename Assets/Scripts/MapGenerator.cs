using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Map[] Maps;
    public int MapIndex;

    public Transform TilePrefab;
    public Transform ObstaclePrefab;
    public Transform NavMeshMaskPrefab;
    public Transform NavMeshFloor;
    public Transform MapFloor;
    public Vector2 MaxMapSize;

    [Range(0,1)]
    public float OutlinePercentage;

    public float TileSize;

    List<Coordinate> TileCoords;
    Queue<Coordinate> ShuffledTileCoords;
    Queue<Coordinate> ShuffledOpenTileCoords;


    Transform[,] TileMap;

    Map CurrentMap;

    void OnNewWave(int WaveNumber)
    {
        MapIndex = WaveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
        CurrentMap = Maps[MapIndex];
        TileMap = new Transform[CurrentMap.MapSize.x, CurrentMap.MapSize.y];
        System.Random PseudoRandom = new System.Random(CurrentMap.Seed);
        // Generating coordinates
        TileCoords = new List<Coordinate>();
        for (int x = 0; x < CurrentMap.MapSize.x; x++)
        {
            for (int y = 0; y < CurrentMap.MapSize.y; y++)
            {
                TileCoords.Add(new Coordinate(x, y));
            }
        }

        ShuffledTileCoords = new Queue<Coordinate>(Util.ShuffleArray(TileCoords.ToArray(), CurrentMap.Seed));
        // Map holder object
        string HolderName = "GenerateMap";

        if(transform.FindChild(HolderName))
        {
            DestroyImmediate(transform.FindChild(HolderName).gameObject);
        }

        Transform MapHolder = new GameObject(HolderName).transform;

        // Spawn Tiles
        MapHolder.parent = transform;
        for (int x = 0; x < CurrentMap.MapSize.x; x++)
        {
            for(int y = 0; y < CurrentMap.MapSize.y; y++)
            {
                Vector3 TilePosition = GetWorldPosition(x, y);
                Transform NewTile = Instantiate(TilePrefab, TilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                NewTile.parent = MapHolder;
                NewTile.localScale = Vector3.one * (1 - OutlinePercentage) * TileSize;
                TileMap[x, y] = NewTile;
            }
        }

        // Spawn obstacles
        bool[,] Obstacles = new bool[(int)CurrentMap.MapSize.x, (int)CurrentMap.MapSize.y];

        int ObstacleCount = (int)(CurrentMap.MapSize.x * CurrentMap.MapSize.y * CurrentMap.ObstaclePercentage);
        int CurrentObstacleCount = 0;
        List<Coordinate> OpenCoordinates = new List<Coordinate>(TileCoords);
        for(int i = 0; i < ObstacleCount; i++)
        {
            Coordinate RandomCoordinate = GetRandomCoordinate();
            Obstacles[RandomCoordinate.x, RandomCoordinate.y] = true;
            CurrentObstacleCount++;
            if (RandomCoordinate != CurrentMap.MapCenter && MapFullyAccessible(Obstacles, CurrentObstacleCount))
            {
                float ObstacleHeight = Mathf.Lerp(CurrentMap.MinimalObstacleHeight, CurrentMap.MaximumObstacleHeight, (float)PseudoRandom.NextDouble());
                Vector3 ObstaclePosition = GetWorldPosition(RandomCoordinate.x, RandomCoordinate.y);
                
                Transform NewObstacle = Instantiate(ObstaclePrefab, ObstaclePosition + Vector3.up * ObstacleHeight / 2, Quaternion.identity) as Transform;
                NewObstacle.localScale = new Vector3((1 - OutlinePercentage) * TileSize, ObstacleHeight, (1 - OutlinePercentage) * TileSize);
                NewObstacle.parent = MapHolder;

                Renderer ObstacleRenderer = NewObstacle.GetComponent<Renderer>();
                Material ObstacleMaterial = new Material(ObstacleRenderer.sharedMaterial);

                float ColourPercentage = RandomCoordinate.y / (float)CurrentMap.MapSize.y;

                ObstacleMaterial.color = Color.Lerp(CurrentMap.ForegroundColour, CurrentMap.BackgroundColour, ColourPercentage);

                ObstacleRenderer.sharedMaterial = ObstacleMaterial;

                OpenCoordinates.Remove(RandomCoordinate);
            }
            else
            {
                Obstacles[RandomCoordinate.x, RandomCoordinate.y] = false;
                CurrentObstacleCount--;
            }
        }

        ShuffledOpenTileCoords = new Queue<Coordinate>(Util.ShuffleArray(OpenCoordinates.ToArray(), CurrentMap.Seed));

        Transform MaskLeft = Instantiate(NavMeshMaskPrefab, Vector3.left * (CurrentMap.MapSize.x + MaxMapSize.x) / 4f * TileSize, Quaternion.identity) as Transform;
        MaskLeft.parent = MapHolder;
        MaskLeft.localScale = new Vector3((MaxMapSize.x - CurrentMap.MapSize.x) / 2f, 1, CurrentMap.MapSize.y) * TileSize;

        Transform MaskRight = Instantiate(NavMeshMaskPrefab, Vector3.right * (CurrentMap.MapSize.x + MaxMapSize.x) / 4f * TileSize, Quaternion.identity) as Transform;
        MaskRight.parent = MapHolder;
        MaskRight.localScale = new Vector3((MaxMapSize.x - CurrentMap.MapSize.x) / 2f, 1, CurrentMap.MapSize.y) * TileSize;

        Transform MaskTop = Instantiate(NavMeshMaskPrefab, Vector3.forward * (CurrentMap.MapSize.y + MaxMapSize.y) / 4f * TileSize, Quaternion.identity) as Transform;
        MaskTop.parent = MapHolder;
        MaskTop.localScale = new Vector3(MaxMapSize.x, 1, (MaxMapSize.y - CurrentMap.MapSize.y) / 2f)* TileSize;

        Transform MaskBottom = Instantiate(NavMeshMaskPrefab, Vector3.back * (CurrentMap.MapSize.y + MaxMapSize.y) / 4f * TileSize, Quaternion.identity) as Transform;
        MaskBottom.parent = MapHolder;
        MaskBottom.localScale = new Vector3(MaxMapSize.x, 1, (MaxMapSize.y - CurrentMap.MapSize.y) / 2f) * TileSize;

        NavMeshFloor.localScale = new Vector3(MaxMapSize.x, MaxMapSize.y) * TileSize;
        MapFloor.localScale = new Vector3(CurrentMap.MapSize.x * TileSize, CurrentMap.MapSize.y * TileSize, .05f);
    }

    bool MapFullyAccessible(bool[,] ObstacleMap, int CurrentObstacleCount)
    {
        bool[,] Visited = new bool[ObstacleMap.GetLength(0), ObstacleMap.GetLength(1)];
        Queue<Coordinate> FloodFillQueue = new Queue<Coordinate>();
        FloodFillQueue.Enqueue(CurrentMap.MapCenter);
        Visited[CurrentMap.MapCenter.x, CurrentMap.MapCenter.y] = true;

        int AccessibleTiles = 1;

        while(FloodFillQueue.Count > 0)
        {
            Coordinate Tile = FloodFillQueue.Dequeue();

            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    int NeighbourX = Tile.x + x;
                    int NeighbourY = Tile.y + y;

                    if(x == 0 || y == 0)
                    {
                        if(NeighbourX >= 0 && NeighbourX < ObstacleMap.GetLength(0) && NeighbourY >= 0 && NeighbourY < ObstacleMap.GetLength(1))
                        {
                            if(!Visited[NeighbourX, NeighbourY] && !ObstacleMap[NeighbourX, NeighbourY])
                            {
                                Visited[NeighbourX, NeighbourY] = true;
                                FloodFillQueue.Enqueue(new Coordinate(NeighbourX, NeighbourY));
                                AccessibleTiles++;
                            }
                        }
                    }
                }
            }
        }

        int TargetTileCount = (int)(CurrentMap.MapSize.x * CurrentMap.MapSize.y - CurrentObstacleCount);

        return TargetTileCount == AccessibleTiles;
    }

    Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(-CurrentMap.MapSize.x / 2f + 0.5f + x, 0, -CurrentMap.MapSize.y / 2f + 0.5f + y) * TileSize;
    }

    public Transform GetTileFromPosition(Vector3 Position)
    {
        int x = Mathf.RoundToInt(Position.x / TileSize + (CurrentMap.MapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(Position.z / TileSize + (CurrentMap.MapSize.y - 1) / 2f);

        x = Mathf.Clamp(x, 0, TileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, TileMap.GetLength(1) - 1);

        return TileMap[x, y];
    }

    public Coordinate GetRandomCoordinate()
    {
        Coordinate RandomCoord = ShuffledTileCoords.Dequeue();
        ShuffledTileCoords.Enqueue(RandomCoord);
        return RandomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coordinate RandomCoord = ShuffledOpenTileCoords.Dequeue();
        ShuffledOpenTileCoords.Enqueue(RandomCoord);

        return TileMap[RandomCoord.x, RandomCoord.y];
        
    }

    // Use this for initialization
    void Start () {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [System.Serializable]
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Coordinate Left, Coordinate Right)
        {
            return Left.x == Right.x && Left.y == Right.y;
        }

        public static bool operator !=(Coordinate Left, Coordinate Right)
        {
            return !(Left == Right);
        }


    }
    [System.Serializable]
    public class Map
    {
        public Coordinate MapSize;
        [Range(0, 1)]
        public float ObstaclePercentage;
        public int Seed;
        public float MinimalObstacleHeight;
        public float MaximumObstacleHeight;
        public Color ForegroundColour;
        public Color BackgroundColour;

        public Coordinate MapCenter
        {
            get
            {
                return new Coordinate(MapSize.x / 2, MapSize.y / 2);
            }
        }

    }
}
