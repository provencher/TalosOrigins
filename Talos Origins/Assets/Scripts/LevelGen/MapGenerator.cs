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

    List<GameObject> asteroids;
    List<GameObject> enemies;

    public int width;
    public int height;
    public int currentLevel;

    private int startWidth, startHeight;

    public string seed;
    public bool useRandomSeed;

    public bool cycleLevel = false;

    public bool resetLevel = false;

    [Range(0, 100)]
    public int randomFillPercent;
    int bossFillPercent = 35;
    int standardFillPercent;

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

    public bool bossRound = false;

    public float levelScale = 1;

	int portalLevelUpgrade;

    int[,] map;

    public bool victory = false;
    public int upgradeNumber = 0;

    void Awake()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
    }

    void Start()
    {
        mTalos = GameObject.Find("Talos");
        asteroids = new List<GameObject>();
        enemies = new List<GameObject>();
        mapPointOccupied = new Dictionary<Coord, int>();
        startWidth = width;
        startHeight = height;
        standardFillPercent = randomFillPercent;
        GenerateMap();
    }

    void FixedUpdate()
    {
        //		Debug.Log ("Reset : " + resetLevel);
        if (cycleLevel)
        {
            cycleLevel = false;
			//portalLevelUpgrade = PlayerPrefs.GetInt("Portal Distance", 0);
            //currentLevel = currentLevel + portalLevelUpgrade;//UnityEngine.Random.Range(currentLevel / 2 + 1, currentLevel + (portalLevelUpgrade * 10) + 5);
            GenerateMap();
        }
        //		if(resetLevel)
        //		{
        //			resetLevel = false;
        //			ResetLevel();
        //			GenerateMap();            
        //		}
    }

    void NextLevel()
    {
        currentLevel++;
        if (currentLevel <= 0)
        {
            currentLevel = 1;
        }       

        GenerateMap();
    }

    void ResetGame()
    {
        Cursor.visible = true;
        Application.LoadLevel(0);
    }

    void SetLevelParameters()
    {
        if (currentLevel <= 1)
        {
            currentLevel = 1;
        }

        if(victory)
        {
            currentLevel = -1;
        }
        //Define Number of upgrades to distribut in map -> ONLY 1 working right now
        upgradeNumber = 1;

        // Save Current Level
        PlayerPrefs.SetInt("currentLevel", currentLevel);

        if (!victory)
        {
            victory = false;
            levelScale = UnityEngine.Random.Range(1, 3);
            mTalos.GetComponentInChildren<Shield>().rechargeDeployed = true;

            
            width = currentLevel + (startWidth);
            height = currentLevel + (startHeight);

            int rand = UnityEngine.Random.Range(1, 3);
            if (rand == 2)
            {               
                height = height / 2;
            }
            else
            {
                width = width / 2;
            }
           

            int densityModifer = 3;// UnityEngine.Random.Range((currentLevel / levelScale) + 1, 2*(currentLevel / levelScale) + 1);

            //width = (int)Mathf.Round((startWidth + levelScale * currentLevel / densityModifer) * 0.65f);


            randomFillPercent = Mathf.CeilToInt(UnityEngine.Random.Range(42, 52)); //* //Mathf.Clamp(UnityEngine.Random.Range(1, levelScale / densityModifer) + 1

          
            if (randomFillPercent < 48)
            {
                width = Mathf.CeilToInt(width * 0.85f);
                height = Mathf.CeilToInt(height * 0.85f);
                levelScale /= 2;
            }

            /*
            // set max values for map size
            if (Math.Min(height, width) == height)
            {
                
                
            }
            else
            {
                height *= 2;
                height  = Math.Min(250, height);
                width   = height / 2;                
            }
          */


            //Reset Level Parameters
            mapPointOccupied = new Dictionary<Coord, int>();
            GameObject.Find("Talos").GetComponent<Grapple>().grapplehooked = false;
            GameObject.Find("Talos").GetComponent<Trail>().ResetTrail();

            //enemies = new List<GameObject>();
            //asteroids = new List<GameObject>();


            numAsteroidsToSpawn = Mathf.CeilToInt(densityModifer * (width / levelScale + currentLevel / Mathf.Max((densityModifer * UnityEngine.Random.Range(1, densityModifer + currentLevel)), 1)));

            numEnemiesToSpawn = Mathf.CeilToInt(densityModifer * (width / levelScale + currentLevel / Mathf.Max((densityModifer * UnityEngine.Random.Range(1, densityModifer + currentLevel)), 1)) / 3);
            bossRound = false;

            numAsteroidsToSpawn = Mathf.Clamp(numAsteroidsToSpawn, 1, 300);
            numEnemiesToSpawn = Mathf.Clamp(numEnemiesToSpawn, 1, 300);


            //DISABLED BOSSES
            if (false && currentLevel % 10 == 0)
            {
                bossRound = true;
                randomFillPercent = bossFillPercent;
                numEnemiesToSpawn = currentLevel / 10;
                width = Mathf.CeilToInt(5 * levelScale * densityModifer);
                height = 25;
            }
        }
        else
        {
            width = 300;
            height = 50;
            numAsteroidsToSpawn = 0;
            numEnemiesToSpawn = 0;
            randomFillPercent = 10;
        }

        
        


       


       
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

        int borderSize = 5;
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
        meshGen.GenerateMesh(borderedMap, 1.01f);

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
                if (CheckForFit(pos, 1, 1, true))
                {
                    center = pos;
                    foundSpot = true;
                    break;
                }
            }

            if (foundSpot)
            {
                GameObject.FindObjectOfType<Grapple>().unHook();
                mTalosCoord = center;
                mTalosPos = CoordToWorldPoint(center);
                //mapPointOccupied.Add(center, 1);
                break;
            }
        }

    }

    void PlaceExitInRoom()
    {
        bool exitFound = false;
        int startIndex = allRooms.Count - 1;
        List<Coord> exits = new List<Coord>();
        float largestDistance = 0;
        int largestIndex = 0;

        Coord tempCoord = new Coord(0, 0);        
        for (int i = 1; i < height; i++)
        {
            for (int j = 1; j < width; j++)
            {
                tempCoord = new Coord(j, i);
                if (CheckForFit(tempCoord, 2, 2, false))
                {                    
                    if (tempCoord.sqrtdistance(mTalosCoord) > largestDistance)
                    {
                        exits.Add(tempCoord);
                        exitFound = true;
                        largestDistance = tempCoord.sqrtdistance(mTalosCoord);
                        largestIndex = exits.Count - 1;                        
                    }                                   
                }                   
            }        
        }
     

        if (exitFound)
        {
            Coord exit = exits[UnityEngine.Random.Range(Mathf.CeilToInt((exits.Count - 1)/2), (exits.Count - 1))];           
            mExitPos = CoordToWorldPoint(exit);
            mExitCoord = exit;
        }
    }
    int enemiesSpawned = 0;
    int asteroidsSpawned = 0;
    void SpawnEverything()
    {
        PlaceTalosInRoom();
        PlaceExitInRoom();
        //SpawnAllEnemies(numEnemiesToSpawn);
        //SpawnAllAsteroids(numAsteroidsToSpawn);      


        enemiesSpawned = 0;
        asteroidsSpawned = 0;

        ClearAllEnemies();
        ClearAllAsteroids();
        Coord tempCoord;
        int sentry = 3;

        float maxScale = Mathf.Clamp(currentLevel / 16 + 1, 0.75f, 4);

        while (numEnemiesToSpawn > 0 && numAsteroidsToSpawn > 0 && sentry > 0)
        {
            for (int i = 1; i < height; i++)
            {
                for (int j = 1; j < width; j++)
                {
                    tempCoord = new Coord(j, i);

                    //Spawn Enemy
                    if (UnityEngine.Random.Range(1, 30) == 15 && numEnemiesToSpawn > 0)
                    {
                        float scaleModifier = UnityEngine.Random.Range(0.6f, maxScale + 1);
                        int roundedScale = Math.Max(Mathf.CeilToInt(Mathf.Clamp(scaleModifier,0, 4)) - 1, 1);

                        if (CheckForFit(tempCoord, roundedScale, roundedScale, true))
                        {
                            SpawnEnemyAtPosition(enemies.Count - 1, tempCoord, scaleModifier);
                            //mapPointOccupied.Add(tempCoord, 3);
                            enemiesSpawned++;
                            numEnemiesToSpawn--;
                        }
                    }
                    //Spawn Asteroid
                    if (UnityEngine.Random.Range(1, 30) == 15 && numAsteroidsToSpawn > 0)
                    {
                        float scaleModifier = UnityEngine.Random.Range(0.6f, maxScale + 1);
                        int roundedScale = Math.Max(Mathf.CeilToInt(Mathf.Clamp(scaleModifier, 0, 4)) - 1, 1);

                        if (CheckForFit(tempCoord, roundedScale, roundedScale, true))
                        {
                            SpawnAsteroidAtPosition(asteroids.Count - 1, tempCoord, scaleModifier);
                            //mapPointOccupied.Add(tempCoord, 4);
                            asteroidsSpawned++;
                            numAsteroidsToSpawn--;
                        }
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
        enemies = new List<GameObject>();
    }

    void SpawnEnemyAtPosition(int index, Coord position, float scale)
    {
        Vector3 WorldPos = CoordToWorldPoint(position);

        if (UnityEngine.Random.Range(0, 3) == 1)
        {
            enemies.Add((GameObject)Instantiate(enemyCo, WorldPos, Quaternion.identity));       
            enemies[enemies.Count - 1].transform.localScale *= Mathf.Clamp(scale, 0.5f, 1.0f);
        }
        else
        {
            enemies.Add((GameObject)Instantiate(tempEnemy, WorldPos, Quaternion.identity));
            enemies[enemies.Count - 1].transform.localScale *= scale;
        }

        if (bossRound)
        {
            enemies[enemies.Count - 1].GetComponent<Enemy>().isBoss = true;
        }
        enemies[enemies.Count - 1].GetComponent<Enemy>().mScaleValue = scale;
        enemies[enemies.Count - 1].GetComponent<Enemy>().upgradeNumber = -1;//UnityEngine.Random.Range(0, 10);

        enemies[enemies.Count - 1].SendMessage("UpdateEnemyIndex", enemies.Count - 1);
        //enemies[index].SendMessage("UpdateLevel", currentLevel);
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
    

    void SpawnAsteroidAtPosition(int index, Coord position, float scale)
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
        asteroids[asteroids.Count - 1].GetComponent<Asteroid_Script>().mScaleValue = scale;
        asteroids[asteroids.Count - 1].transform.localScale *= scale;
        asteroids[asteroids.Count - 1].SendMessage("UpdateAsteroidIndex", asteroids.Count - 1);
        asteroids[asteroids.Count - 1].SendMessage("UpdateLevel", currentLevel);        

        if (upgradeNumber > 0 && asteroids.Count > numAsteroidsToSpawn/2)
        {
            upgradeNumber = 0;
            asteroids[asteroids.Count - 1].GetComponent<Asteroid_Script>().upgradeNumber = UnityEngine.Random.Range(0, 10);
            asteroids[asteroids.Count - 1].AddComponent<Light>();
            asteroids[asteroids.Count - 1].GetComponent<Light>().range = 20;
            asteroids[asteroids.Count - 1].GetComponent<Light>().color = Color.yellow;
            Debug.Log("UPGRADES INDEX" + (asteroids.Count - 1).ToString());
        }
    }


    void ClearAllAsteroids()
    {
        for (int i = 0; i < asteroids.Count; i++)
        {
            Destroy(asteroids[i]);
        }
        numAsteroids = 0;
        asteroids = new List<GameObject>();
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
        mTalos.SendMessage("StartPos", mTalosPos);
        mTalos.SendMessage("TotalEnemies", enemies.Count);
        mTalos.SendMessage("CurrentLevel", currentLevel);

        if (bossRound)
        {
            Vector3 TempExit = mExitPos;
            TempExit.x += width;
            mTalos.SendMessage("ExitPos", TempExit);
            mExit.SendMessage("NewExit", TempExit);
        }
        else
        {
            mTalos.SendMessage("ExitPos", mExitPos);
            mExit.SendMessage("NewExit", mExitPos);
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////  

    // Check if each corner of the sprite fits in the tile
    bool CheckForFit(Coord pos, int offSetX, int offSetY, bool markAsOccupied)
    {
        Coord tempCord = new Coord();

        // Check for obstacles
        for (int i = -offSetX; i <= offSetX; i++)
        {
            tempCord.tileX = pos.tileX + i;

            for (int j = -offSetY; j <= offSetY; j++)
            {
                tempCord.tileY = pos.tileY + j;
               
                if (!IsInMapRange(tempCord.tileX, tempCord.tileY)
                    || (map[tempCord.tileX, tempCord.tileY] != 0) 
                    || mapPointOccupied.ContainsKey(tempCord))
                {                    
                    return false;                                  
                }               
            }
        }

        if (markAsOccupied)
        {
            for (int i = -offSetX; i <= offSetX; i++)
            {
                for (int j = -offSetY; j <= offSetY; j++)
                {
                    tempCord.tileX = pos.tileX + i;
                    tempCord.tileY = pos.tileY + j;
                    if (!mapPointOccupied.ContainsKey(tempCord))
                    {
                        mapPointOccupied.Add(tempCord, 2);
                    }
                }

            }
        }

        return true;
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

            for (long i = currentLevel; i < UnityEngine.Random.Range(currentLevel + 1, currentLevel + UnityEngine.Random.Range(0, 10)); i++)
            {
                rng += Math.Abs(i * TimeZoneInfo.GetSystemTimeZones().ToString().GetHashCode() * DateTime.Now.Millisecond);
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

        public float sqrtdistance(Coord otherTile)
        {
            return Mathf.Sqrt(Mathf.Pow((tileX - otherTile.tileX), 2) + Mathf.Pow((tileY - otherTile.tileY), 2));
        }

        public int CompareTo(Coord otherTile)
        {
            // equal
            if (tileX == otherTile.tileX && tileY == otherTile.tileY)
            {
                return 0;
            }
            // larger
            else if ((tileX > otherTile.tileX && tileY > otherTile.tileY))
            {

                return 1;
            }
            else
            {
                return -1;
            }
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