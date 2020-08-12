using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
#endif

using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneBuilder : MonoBehaviour
{

    public Grid grid;
    public Grid[] rooms;
    public int roomTotal = 5;
    public int passageGoal = 3;

    private Tilemap[] gridLayers;

    public void Awake()
    {
        gridLayers = grid.GetComponentsInChildren<Tilemap>();
    }

    public void GridBuild()
    {
        List<Grid> builtRooms = new List<Grid>();
        List<Vector3Int> builtPositions = new List<Vector3Int>();

        // initial room

        var currentRoom = rooms[0];
        Vector3Int currentRoomPosition = new Vector3Int(-1, -2, 0);
       
        AddRoom(currentRoomPosition, currentRoom);
        builtRooms.Add(currentRoom);
        builtPositions.Add(currentRoomPosition);

        int roomCount = 0;

        // BuildLoop

        while (roomCount < roomTotal)
        {
            int rIndex = Random.Range(0, builtRooms.Count);
            currentRoom = builtRooms[rIndex];
            currentRoomPosition = builtPositions[rIndex];

            var data = currentRoom.GetComponent<GridRoomData>();
            var hookPoint = data.hookPoints[Random.Range(0, data.hookPoints.Length)];
            var currentRoomLayers = currentRoom.GetComponentsInChildren<Tilemap>();
            var bounds = currentRoomLayers[1].cellBounds;
            //Debug.Log(bounds.size);

            Vector3Int hookPosition = new Vector3Int();
            hookPosition.x = currentRoomPosition.x;
            hookPosition.y = currentRoomPosition.y;

            hookPosition.x += bounds.xMin;
            hookPosition.y += bounds.yMin;
            char newHookDirection = '-';

            // Find hookPosition

            switch (hookPoint.direction)
            {
                case 'N':
                    hookPosition.x += hookPoint.position;
                    hookPosition.y += bounds.size.y - 1;
                    newHookDirection = 'S';
                    break;
                case 'S':
                    hookPosition.x += hookPoint.position;
                    newHookDirection = 'N';
                    break;
                case 'W':
                    hookPosition.y += hookPoint.position;
                    newHookDirection = 'E';
                    break;
                case 'E':
                    hookPosition.x += bounds.size.x - 1;
                    hookPosition.y += hookPoint.position;
                    newHookDirection = 'W';
                    break;
            }

            // Find potentially matching hookpoint and room

            Grid newRoom = null;
            GridRoomData newData = null;
            GridRoomData.HookPoint newHookPoint = null;

            while (newRoom == null)
            {
                newRoom = rooms[Random.Range(0, rooms.Length)];
                newData = newRoom.GetComponent<GridRoomData>();
                newHookPoint = newData.hookPoints[Random.Range(
                    0, newData.hookPoints.Length)];
                if (newHookPoint.direction != newHookDirection) newRoom = null;
            }

            var newRoomLayers = newRoom.GetComponentsInChildren<Tilemap>();

            // Check for clear area

            var newBounds = newRoomLayers[1].cellBounds;
            var newRoomPosition = new Vector3Int();
            newRoomPosition.x = currentRoomPosition.x;
            newRoomPosition.y = currentRoomPosition.y;
            newRoomPosition.z = currentRoomPosition.z;

            Vector3Int newHookPosition = new Vector3Int();
            newHookPosition.x = newRoomPosition.x;
            newHookPosition.y = newRoomPosition.y;

            newHookPosition.x += newBounds.xMin;
            newHookPosition.y += newBounds.yMin;

            switch (newHookPoint.direction)
            {
                case 'N':
                    newHookPosition.x += newHookPoint.position;
                    newHookPosition.y += newBounds.size.y - 1;
                    break;
                case 'S':
                    newHookPosition.x += newHookPoint.position;
                    newHookDirection = 'N';
                    break;
                case 'W':
                    newHookPosition.y += newHookPoint.position;
                    break;
                case 'E':
                    newHookPosition.x += newBounds.size.x - 1;
                    newHookPosition.y += newHookPoint.position;
                    break;
            }

            Vector3Int roomDiff = hookPosition - newHookPosition;
            newRoomPosition += roomDiff;

            if (!CheckRoomFit(newRoomPosition, newRoom)) continue;

            AddRoom(newRoomPosition, newRoom);

            // Set passage

            gridLayers[1].SetTile(hookPosition, null);
            gridLayers[0].SetTile(hookPosition, data.floor);

            if (hookPoint.direction == 'N' || hookPoint.direction =='S')
            {
                hookPosition.x += 1;
            }
            else
            {
                hookPosition.y += 1;
            }

            gridLayers[1].SetTile(hookPosition, null);
            gridLayers[0].SetTile(hookPosition, data.floor);

            builtRooms.Add(newRoom);
            builtPositions.Add(newRoomPosition);
            ++roomCount;
        }

        // Connecting passages

        int passageCounter = 0;

        for (int i = 0; i < 500; ++i)
        {
            if (AttemptPassage(builtRooms, builtPositions))
            {
                if (++passageCounter == passageGoal) break;
            }
        }

        Debug.Log($"Passages built: {passageCounter}");
    }

    void AddRoom(Vector3Int position, Grid room)
    {
        var tilemaps = room.GetComponentsInChildren<Tilemap>();

        // Tile layers first...

        for (int k = 0; k < 2; ++k)
        {
            var localPosition = position;

            var map = tilemaps[k];
            var bounds = map.cellBounds;
            localPosition.x += bounds.xMin;
            localPosition.y += bounds.yMin;
            var endPosition = new Vector3Int(localPosition.x + bounds.size.x,
                localPosition.y + bounds.size.y, 0);
            var tiles = map.GetTilesBlock(bounds);
            
            int tIndex = 0;
            //Debug.Log($"{localPosition},  {endPosition}");
            for (int j = localPosition.y; j < endPosition.y; ++j)
            {
                for (int i = localPosition.x; i < endPosition.x; ++i)
                {
                    TileBase tile = tiles[tIndex++];
                    if (tile != null)
                    {
                        gridLayers[k].SetTile(new Vector3Int(i, j, 0), tile);
                    }
                }
            }
        }

        var objectCollection = tilemaps[2].GetComponentsInChildren<SpriteRenderer>();
        foreach(var sr in objectCollection)
        {
            var obj = sr.gameObject;
            Instantiate(obj, obj.transform.position + position,
                Quaternion.identity);
        }  
    }

    bool CheckRoomFit(Vector3Int position, Grid room)
    {
        var tilemaps = room.GetComponentsInChildren<Tilemap>();
        var localPosition = position;
        var map = tilemaps[1];
        var bounds = map.cellBounds;
        localPosition.x += bounds.xMin;
        localPosition.y += bounds.yMin;
        var endPosition = new Vector3Int(localPosition.x + bounds.size.x,
            localPosition.y + bounds.size.y, 0);

        for (int j = localPosition.y + 1; j < endPosition.y - 1; ++j)
        {
            for (int i = localPosition.x + 1; i < endPosition.x - 1; ++i)
            {
                var tile = gridLayers[0].GetTile(new Vector3Int(i, j, 0));
                if (tile != null) return false;
                tile = gridLayers[1].GetTile(new Vector3Int(i, j, 0));
                if (tile != null) return false;
            }
        }

        return true;
    }

    struct PassageHook
    {
        public Vector2Int position;
        public char direction;
        public PassageHook(int x, int y, char _direction)
        {
            position = new Vector2Int(x, y);
            direction = _direction;
        }
    }

    bool AttemptPassage(List<Grid> builtRooms, List<Vector3Int> builtPositions)
    {
        List<PassageHook> hooks = new List<PassageHook>();
        List<Vector2Int> anchors = new List<Vector2Int>();
        List<TileBase> floors = new List<TileBase>();
        List<TileBase> walls = new List<TileBase>();

        int firstRoomIndex = -1;

        for (int i = 0; i < 2; ++i)
        {
            int rIndex = Random.Range(0, builtRooms.Count);

            if (rIndex == firstRoomIndex) return false;
            firstRoomIndex = rIndex;

            var roomA = builtRooms[rIndex];
            var roomAPosition = builtPositions[rIndex];

            var data = roomA.GetComponent<GridRoomData>();
            floors.Add(data.floor);
            walls.Add(data.wall);
            var hookPointA = data.hookPoints[Random.Range(0, data.hookPoints.Length)];
            var currentRoomLayers = roomA.GetComponentsInChildren<Tilemap>();
            var bounds = currentRoomLayers[1].cellBounds;
            //Debug.Log(bounds.size);

            Vector3Int hookPositionA = new Vector3Int();
            hookPositionA.x = roomAPosition.x;
            hookPositionA.y = roomAPosition.y;

            hookPositionA.x += bounds.xMin;
            hookPositionA.y += bounds.yMin;

            // Find hookPosition

            switch (hookPointA.direction)
            {
                case 'N':
                    hookPositionA.x += hookPointA.position;
                    hookPositionA.y += bounds.size.y - 1;
                    anchors.Add(new Vector2Int(hookPositionA.x, hookPositionA.y + 1));
                    break;
                case 'S':
                    hookPositionA.x += hookPointA.position;
                    anchors.Add(new Vector2Int(hookPositionA.x, hookPositionA.y - 2));
                    break;
                case 'W':
                    hookPositionA.y += hookPointA.position;
                    anchors.Add(new Vector2Int(hookPositionA.x - 2, hookPositionA.y));
                    break;
                case 'E':
                    hookPositionA.x += bounds.size.x - 1;
                    hookPositionA.y += hookPointA.position;
                    anchors.Add(new Vector2Int(hookPositionA.x + 1, hookPositionA.y));
                    break;
            }
            hooks.Add(new PassageHook(hookPositionA.x,
                hookPositionA.y, hookPointA.direction));
        }

        // Check anchor areas

        for (int k = 0; k < 2; ++k)
        {
            var anchor = anchors[k];
            for (int i = anchor.x; i < anchor.x + 2; ++i)
            {
                for (int j = anchor.y; j < anchor.y + 2; ++j)
                {
                    if (gridLayers[0].GetTile(new Vector3Int(i, j, 0))
                        != null ||
                        gridLayers[1].GetTile(new Vector3Int(i, j, 0))
                        != null)
                    {
                        return false;
                    }
                }
            }
        }

        Vector2Int seeker = new Vector2Int(anchors[0].x, anchors[0].y);
        
        // N/S

        while(seeker.y != anchors[1].y)
        {
            if (seeker.y < anchors[1].y) ++seeker.y;
            else --seeker.y;

            if (gridLayers[1].GetTile(new Vector3Int(seeker.x, seeker.y, 0))
                        != null ||
                       gridLayers[1].GetTile(new Vector3Int(seeker.x + 1, seeker.y, 0))
                        != null)
            {
                return false;
            }
        }

        // Turning point

        for (int i = seeker.x; i < seeker.x + 2; ++i)
        {
            for (int j = seeker.y; j < seeker.y + 2; ++j)
            {
                if (gridLayers[1].GetTile(new Vector3Int(seeker.x, seeker.y, 0))
                        != null) return false;
            }
        }

        // E/W

        while (seeker.x != anchors[1].x)
        {
            if (seeker.x < anchors[1].x) ++seeker.x;
            else --seeker.x;

            if (gridLayers[1].GetTile(new Vector3Int(seeker.x, seeker.y, 0))
                        != null ||
                        gridLayers[1].GetTile(new Vector3Int(seeker.x, seeker.y + 1, 0))
                        != null)
            {
                return false;
            }
        }

        // Success: Build Passage

        List<Vector2Int> newFloors = new List<Vector2Int>();

        // Build "landing pads" and connect

        for (int k = 0; k < 2; ++k)
        {
            var anchor = anchors[k];
            for (int i = anchor.x; i < anchor.x + 2; ++i)
            {
                for (int j = anchor.y; j < anchor.y + 2; ++j)
                {
                    gridLayers[0].SetTile(new Vector3Int(i, j, 0), floors[0]);
                    newFloors.Add(new Vector2Int(i, j));
                }
            }
        }

        seeker = new Vector2Int(anchors[0].x, anchors[0].y);

        // N/S

        while (seeker.y != anchors[1].y)
        {
            if (seeker.y < anchors[1].y) ++seeker.y;
            else --seeker.y;

            gridLayers[0].SetTile(new Vector3Int(seeker.x, seeker.y, 0), floors[0]);
            newFloors.Add(new Vector2Int(seeker.x, seeker.y));
            gridLayers[0].SetTile(new Vector3Int(seeker.x + 1, seeker.y, 0), floors[0]);
            newFloors.Add(new Vector2Int(seeker.x + 1, seeker.y));
        }

        // Turning point

        for (int i = seeker.x; i < seeker.x + 2; ++i)
        {
            for (int j = seeker.y; j < seeker.y + 2; ++j)
            {
                gridLayers[0].SetTile(new Vector3Int(i, j, 0), floors[0]);
                newFloors.Add(new Vector2Int(i, j));
            }
        }

        // E/W

        while (seeker.x != anchors[1].x)
        {
            if (seeker.x < anchors[1].x) ++seeker.x;
            else --seeker.x;

            gridLayers[0].SetTile(new Vector3Int(seeker.x, seeker.y, 0), floors[0]);
            newFloors.Add(new Vector2Int(seeker.x, seeker.y));
            gridLayers[0].SetTile(new Vector3Int(seeker.x, seeker.y + 1, 0), floors[0]);
            newFloors.Add(new Vector2Int(seeker.x, seeker.y + 1));
        }

        // WALLS

        for (int k = 0; k < newFloors.Count; ++k)
        {
            var curFloor = newFloors[k];
            for (int i = curFloor.x - 1; i < curFloor.x + 2; ++i)
            {
                for (int j = curFloor.y - 1; j < curFloor.y + 2; ++j)
                {
                    if (gridLayers[0].GetTile(new Vector3Int(i, j, 0)) == null)
                    {
                        gridLayers[1].SetTile(new Vector3Int(i, j, 0), walls[0]);
                    }
                }
            }
        }

        // Remove Hooktiles

        for (int k = 0; k < 2; ++k)
        {
            var hookPoint = hooks[k];
            var hookPosition = new Vector3Int(hooks[k].position.x,
                hooks[k].position.y, 0);

            gridLayers[1].SetTile(hookPosition, null);
            gridLayers[0].SetTile(hookPosition, floors[k]);

            if (hookPoint.direction == 'N' || hookPoint.direction == 'S')
            {
                hookPosition.x += 1;
            }
            else
            {
                hookPosition.y += 1;
            }

            gridLayers[1].SetTile(hookPosition, null);
            gridLayers[0].SetTile(hookPosition, floors[k]);
        }


        return true;
    }
}
