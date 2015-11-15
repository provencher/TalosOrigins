using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject mTalos;
    [SerializeField]
    GameObject mExit;

    [SerializeField]
    GameObject tempEnemy;

    [SerializeField]
    GameObject enemyCo;

    [SerializeField]
    GameObject asteroid;
    int numAsteroids;

    GameObject[] spaceProps;
    int propSpawnIndex = 0;

    List<GameObject> asteroids;
    List<GameObject> enemies;

    public int width;
    public int height;
    public int currentLevel;

    private int startWidth, startHeight;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    [Range(1, 10)]
    public int hallWayRadius;

    [Range(1, 100)]
    public int wallThresholdSize;

    [Range(1, 100)]
    public int roomThresholdSize;


    [Range(1, 50)]
    public int enemyModifier;

    [Range(50, 500)]
    public int numEnemiesToSpawn;

    [Range(50, 500)]
    public int numAsteroidsToSpawn;

    Vector3 mTalosPos;
    Coord mTalosCoord;

    Vector3 mExitPos;
    Coord mExitCoord;

    private Room startRoom, endRoom;
    private List<Room> allRooms;

    Dictionary<Coord, int> mapPointOccupied;


    int[,] map;

    void Start()
    {
        asteroids = new List<GameObject>();
        enemies = new List<GameObject>();
        mapPointOccupied = new Dictionary<Coord, int>(); 
        startWidth = width;
        startHeight = height; 
        spaceProps = Resources.LoadAll<GameObject>("SpacePrefabs");
        GenerateMap();
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("MapGeneration"))
        {
            ResetLevel();
            GenerateMap();
        }        
    }

    void ResetLevel()
    {
        GameObject.Find("Talos").GetComponent<Grapple>().grapplehooked = false;
        GameObject.Find("Talos").GetComponent<Trail>().ResetTrail();
        mapPointOccupied = new Dictionary<Coord, int>();
    }

    void NextLevel()
    {
        currentLevel++;
        ResetLevel();
        GenerateMap();
    }

    void ResetGame()
    {
        Application.LoadLevel(0);
    }

    void SetLevelParameters()
    {
        if (currentLevel == 0)
        {
            currentLevel = 1;
        }

        width = ++startWidth;
        height = ++startHeight;

        width = startWidth + 3 * currentLevel;
        height = startHeight + 3 * currentLevel;

        numAsteroidsToSpawn = width/2 + 2 * UnityEngine.Random.Range(1, enemyModifier + currentLevel);
        numEnemiesToSpawn = width/2 + 2 * UnityEngine.Random.Range(1, enemyModifier + currentLevel);

        propSpawnIndex = 0;

        // set max values for map size
        width = Math.Min(1000, width);
        height = Math.Min(1000, height);
    }


    void GenerateMap()
    {
        SetLevelParameters();
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        ProcessMap();

        int borderSize = 1;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);

        SpawnEverything();
        MessageHandling();
    }

    void PlaceTalosInRoom()
    {
        bool foundSpot = false;
        foreach (Room room in allRooms)
        {
            List<Coord> coordsInRoom = room.tiles;
            Coord center = new Coord();

            foreach (Coord pos in coordsInRoom)
            {
                if (CheckForFit(pos, 1, 1))
                {
                    center = pos;
                    foundSpot = true;
                    break;
                }
            }

            if (foundSpot)
            {
                mTalosCoord = center;
                mTalosPos = CoordToWorldPoint(center);
                mapPointOccupied.Add(center, 1);
                break;
            }
        }
    }

    void PlaceExitInRoom()
    {
        bool exitFound = false;
        int startIndex = allRooms.Count - 1;

        while (startIndex > 1)
        {
            endRoom = allRooms[startIndex];

            List<Coord> coordsInRoom = endRoom.tiles;
            Coord center = new Coord();     

            foreach (Coord pos in coordsInRoom)
            {
                if (CheckForFit(pos, 1, 1))
                {
                    center = pos;
                    exitFound = true;
                    break;
                }
            }

            if (exitFound)
            {
                mExitPos = CoordToWorldPoint(center);
                mExitCoord = center;
                mapPointOccupied.Add(center, 2);
                break;
            }
            else
            {
                startIndex--;
            }
        }
    }

    void SpawnEverything()
    {
        PlaceTalosInRoom();
        PlaceExitInRoom();
        //SpawnAllEnemies(numEnemiesToSpawn);
        //SpawnAllAsteroids(numAsteroidsToSpawn);      


        int enemiesSpawned = 0;
        int asteroidsSpawned = 0;

        ClearAllEnemies();
        ClearAllAsteroids();
        Coord tempCoord;       
        int sentry = 3;

        while (numEnemiesToSpawn > 0 && numAsteroidsToSpawn > 0 && sentry > 0)
        {
            for (int i = 1; i < height; i++)
            {
                for (int j = 1; j < width; j++)
                {
                    tempCoord = new Coord(j, i);

                    //Spawn Asteroid
                    if (UnityEngine.Random.Range(1, 30) == 15 && numEnemiesToSpawn > 0)
                    {
                        if (CheckForFit(tempCoord, 1, 1))
                        {
                            SpawnEnemyAtPosition(enemiesSpawned, tempCoord);
                            mapPointOccupied.Add(tempCoord, 3);
                            enemiesSpawned++;
                            numEnemiesToSpawn--;
                        }
                    }
                    //Spawn Enemy
                    if (UnityEngine.Random.Range(1, 30) == 15 && numAsteroidsToSpawn > 0)
                    {
                        if (CheckForFit(tempCoord, 1, 1))
                        {
                            SpawnAsteroidAtPosition(asteroidsSpawned, tempCoord);
                            mapPointOccupied.Add(tempCoord, 4);
                            asteroidsSpawned++;
                            numAsteroidsToSpawn--;
                        }
                    }

                    if(UnityEngine.Random.Range(1, 50) == 15 && propSpawnIndex < spaceProps.Length)
                    {
                        Instantiate(spaceProps[propSpawnIndex], CoordToWorldPoint(tempCoord), Quaternion.identity);
                        propSpawnIndex++;

                    }

                }
            }
            sentry--;
        }       
    }


    /// ENEMY PROCESSING
    ////////////////////////////////////////////////////////////////////////////////////
    void ClearAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }
        enemies.Clear();
    }

    void SpawnEnemyAtPosition(int index, Coord position)
    {
        Vector3 WorldPos = CoordToWorldPoint(position);

        if (UnityEngine.Random.Range(0, 3) == 1)
        {
            enemies.Add((GameObject)Instantiate(enemyCo, WorldPos, Quaternion.identity));
        }
        else
        {
            enemies.Add((GameObject)Instantiate(tempEnemy, WorldPos, Quaternion.identity));
        }

        enemies[index].SendMessage("UpdateEnemyIndex", index);
        //enemies[index].SendMessage("UpdateLevel", currentLevel);
    }

    void SpawnAllEnemies(int numToSpawn)
    {
        ClearAllEnemies();
        Coord tempCoord;
        int enemiesSpawned = 0;
        int sentry = 3;

        while (numToSpawn > 0 && sentry > 0)
        {
            for (int i = 1; i < height; i++)
            {
                for (int j = 1; j < width; j++)
                {
                    tempCoord = new Coord(j, i);

                    if (UnityEngine.Random.Range(0, numEnemiesToSpawn) == numToSpawn % enemyModifier)
                    {
                        if (CheckForFit(tempCoord, 1, 1))
                        {
                            SpawnEnemyAtPosition(enemiesSpawned, tempCoord);
                            enemiesSpawned++;
                            numToSpawn--;
                        }
                    }
                }
            }
        }
        sentry--;
    }

    


    void KilledEnemy(int index)
    {
        if (index < enemies.Count - 1)
        {
            Destroy(enemies[index]);
        }
    }

    //Asteroid Processing
    ////////////////////////////////////////////////////////////////////////////////////
    void SpawnAllAsteroids(int numToSpawn)
    {
        ClearAllAsteroids();
        Coord tempCoord;
        numAsteroids = 0;
        int sentry = 3;

        while (numToSpawn > 0 && sentry > 0)
        {
            for (int i = 1; i < height; i++)
            {
                for (int j = 1; j < width; j++)
                {
                    tempCoord = new Coord(j, i);

                    if (UnityEngine.Random.Range(0, numEnemiesToSpawn) == numToSpawn % enemyModifier)
                    {
                        if (CheckForFit(tempCoord, 1, 1))
                        {
                            SpawnAsteroidAtPosition(numAsteroids, tempCoord);
                            numAsteroids++;
                            numToSpawn--;
                        }
                    }
                }
            }
        }
        sentry--;
    }

    void SpawnAsteroidAtPosition(int index, Coord position)
    {
        Vector3 WorldPos = CoordToWorldPoint(position);

        asteroids.Add((GameObject)Instantiate(asteroid, WorldPos, Quaternion.identity));

        /*
        if (UnityEngine.Random.Range(0, 3) == 1)
        {
            enemies.Add((GameObject)Instantiate(enemyCo, WorldPos, Quaternion.identity));
        }
        else
        {
            enemies.Add((GameObject)Instantiate(tempEnemy, WorldPos, Quaternion.identity));
        }
        */

        asteroids[index].SendMessage("UpdateAsteroidIndex", index);
        asteroids[index].SendMessage("UpdateLevel", currentLevel);
    }


    void ClearAllAsteroids()
    {
        for (int i = 0; i < asteroids.Count; i++)
        {
            Destroy(asteroids[i]);
        }
        numAsteroids = 0;
        asteroids.Clear();
    }

    void DestroyAsteroid(int index)
    {
        if (index < asteroids.Count - 1)
        {
            Destroy(asteroids[index]);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////



    void MessageHandling()
    {
        Debug.Log("Talos Start Position: " + mTalosPos.ToString());
        mTalos.SendMessage("StartPos", mTalosPos);
        mTalos.SendMessage("TotalEnemies", enemies.Count);
        mTalos.SendMessage("CurrentLevel", currentLevel);
        mTalos.SendMessage("ExitPos", mExitPos);
        mExit.SendMessage("NewExit", mExitPos);
    }


    ////////////////////////////////////////////////////////////////////////////////////  

    // Check if each corner of the sprite fits in the tile
    bool CheckForFit(Coord pos, int offSetX, int offSetY)
    {     
        bool clear = true;       

        int value;
        mapPointOccupied.TryGetValue(pos, out value);
        if(value > 0)
        {
            clear = false;
        }       

        return
            clear
            //In map Range
            && ((pos.tileX - offSetX) > 0) && ((pos.tileX + offSetX) < width)
            && ((pos.tileY - offSetY) > 0) && ((pos.tileY + offSetY) < height)
            //Top Left
            && (map[pos.tileX - offSetX, pos.tileY + offSetY] == 0)
            //Top Right
            && (map[pos.tileX + offSetX, pos.tileY + offSetY] == 0)
            //Lower Left
            && (map[pos.tileX - offSetX, pos.tileY - offSetY] == 0)
            //Lower Right
            && (map[pos.tileX + offSetX, pos.tileY - offSetY] == 0);
    }


    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        //int wallThresholdSize = 50;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        //int roomThresholdSize = 50;
        allRooms = new List<Room>();  
            
        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                allRooms.Add(new Room(roomRegion, map));                
            }
        }
        allRooms.Sort();
        allRooms[0].isMainRoom = true;
        allRooms[0].isAccessibleFromMainRoom = true;             

        ConnectClosestRooms(allRooms);
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, hallWayRadius);
        }
    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, -height / 2 + .5f + tile.tileY, 0);
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }


    void RandomFillMap()
    {
        //disable non-random seed
        if (useRandomSeed)
        {           
            long rng = TimeZoneInfo.GetSystemTimeZones().ToString().GetHashCode() + DateTime.Now.Millisecond;
            rng = Math.Abs(rng);                     
          
            for(long i = currentLevel; i < UnityEngine.Random.Range(currentLevel + 1, currentLevel + UnityEngine.Random.Range(0, 10)); i++)
            {
                rng += Math.Abs(i* TimeZoneInfo.GetSystemTimeZones().ToString().GetHashCode() * DateTime.Now.Millisecond);
            }
            
            seed = Math.Abs(rng).ToString();
        }        

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    class Coord : IComparable<Coord>
    {
        public int tileX;
        public int tileY;

        public Coord()
        {
            tileX = 0;
            tileY = 0;
        }

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }

        public int Value()
        {
            return Math.Abs(tileX) + Math.Abs(tileY);
        }

        public int CompareTo(Coord otherTile)
        {
            return otherTile.Value().CompareTo(Value());
        }
    }


    class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}