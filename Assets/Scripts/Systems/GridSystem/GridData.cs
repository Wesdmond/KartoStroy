using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObejctAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    public List<PlacementData> GetAllPlacedObjects()
    {
        HashSet<PlacementData> uniqueObjects = new HashSet<PlacementData>(placedObjects.Values);
        return new List<PlacementData>(uniqueObjects);
    }

    public List<Vector3Int> GetNeighborPositions(Vector3Int gridPosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            gridPosition + new Vector3Int(1, 0, 0),  // Право
            gridPosition + new Vector3Int(-1, 0, 0), // Лево
            gridPosition + new Vector3Int(0, 0, 1),  // Верх
            gridPosition + new Vector3Int(0, 0, -1)  // Низ
        };
        return neighbors;
    }

    // Установить состояние здания
    public void SetBuildingBroken(Vector3Int gridPosition, bool isBroken)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            placedObjects[gridPosition].IsBroken = isBroken;
        }
    }

    // Получить состояние здания
    public bool IsBuildingBroken(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition].IsBroken;
        }
        return false;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public bool IsBroken { get; set; } // Состояние здания (поломано или нет)

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        IsBroken = false; // По умолчанию здание не поломано
    }
}