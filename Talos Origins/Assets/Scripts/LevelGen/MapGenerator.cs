﻿using UnityEngine;
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


    [Range(10, 20)]
    public int enemyModifier;

    int numEnemies;


    private Room startRoom, endRoom;
    private List<Room> allRooms;


    int[,] map;

    void Start()
    {
        enemies = new List<GameObject>();
        startWidth = width;
        startHeight = height;
        GenerateMap();     
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    void SetLevelParameters()
    {
        if (currentLevel == 0)
        {
            currentLevel = 1;
        }
        width = startWidth;
        height = startHeight;

        width = startWidth + 3 * currentLevel;
        height = startHeight + 3 * currentLevel;

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

        PlaceTalosInRoom();
        PlaceExitInRoom();
        SpawnAllEnemies();
    }

    void PlaceTalosInRoom()
    {
        startRoom = allRooms[0];

        List<Coord> coordsInRoom = startRoom.tiles;
        Coord center = new Coord();
        bool foundSpot = false;

        foreach (Coord pos in coordsInRoom)
        {
            if (CheckForFit(pos, 1, 1))
            {
                center = pos;
                foundSpot = true;
                break;
            }
        }

        //if spot not found choose random spot
        if (!foundSpot)
        {
            center = coordsInRoom[UnityEngine.Random.Range(((int)coordsInRoom.Count / 3), coordsInRoom.Count - 1)];
        }

        Vector3 position = CoordToWorldPoint(center);

        Debug.Log("Talos Start Position: " + position.ToString());
        mTalos.SendMessage("StartPos", position);
    }

    /// ENEMY PROCESSING
    ////////////////////////////////////////////////////////////////////////////////////
    void ClearAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }
    }

    void SpawnEnemiesAtPosition(int numEnemies, Coord position)
    {
        Vector3 WorldPos = CoordToWorldPoint(position);

        //Add Spacing code to spread enemies out       
        //Unecessary
        Vector3 offSetVector = Vector3.zero;//new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0);

        for (int i = 0; i < numEnemies; i++)
        {          
            enemies.Add((GameObject)Instantiate(tempEnemy, WorldPos+ offSetVector, Quaternion.identity));
            enemies[i].SendMessage("UpdateEnemyIndex", i);            
            enemies[i].SendMessage("UpdateLevel", currentLevel);
        }
    }

    void SpawnAllEnemies()
    {
        int numEnemiesToSpawn;
        ClearAllEnemies();
        Coord tempCoord;

        numEnemies = currentLevel * 10;

        for (int i = 1; i < width; i++)
        {
            for (int j = 1; j < height; j++)
            {
                tempCoord = new Coord(j, i);
                if(CheckForFit(tempCoord, 1, 1) && numEnemies > 0)
                {
                    numEnemiesToSpawn = UnityEngine.Random.Range(1, 1);
                    SpawnEnemiesAtPosition(numEnemiesToSpawn, tempCoord);
                    numEnemies -= numEnemiesToSpawn;
                }
            }     
        }
    }
        
    void KilledEnemy(int index)
    {
        if (index < enemies.Count - 1)
        {
            Destroy(enemies[index]);
            enemies.RemoveAt(index);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////  


    void PlaceExitInRoom()
    {
        endRoom = allRooms[allRooms.Count - 1];

        List<Coord> coordsInRoom = endRoom.tiles;
        Coord center = new Coord();
        bool foundSpot = false;

        foreach (Coord pos in coordsInRoom)
        {
            if (CheckForFit(pos, 1, 1))
            {
                center = pos;
                foundSpot = true;
                break;
            }
        }

        //if spot not found choose random spot
        if (!foundSpot)
        {
            center = coordsInRoom[UnityEngine.Random.Range(((int)coordsInRoom.Count / 3), coordsInRoom.Count - 1)];
        }

        Vector3 position = CoordToWorldPoint(center);

        Destroy(mExit);
        mExit = (GameObject)Instantiate(mExit, position, Quaternion.identity);
    }    


    // Check if each corner of the sprite fits in the tile
    bool CheckForFit(Coord pos, int offSetX, int offSetY)
    {
        return
            //Top Left
            (map[pos.tileX - offSetX, pos.tileY + offSetY] == 0)
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

        int roomCount = 0;        
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
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
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