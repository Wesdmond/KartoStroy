using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

// Вспомогательный класс PlacementState для обработки размещения
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    BuildingSO buildingSO; // Добавляем BuildingSO
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          BuildingSO buildingSO,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.buildingSO = buildingSO;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            // Проверяем, есть ли соответствующая запись в BuildingSO
            var objectData = database.objectsData[selectedObjectIndex];
            var buildingData = buildingSO.buildingsData.Find(b => b.prefab == objectData.Prefab);
            if (buildingData == null)
            {
                Debug.LogWarning($"No BuildingData found for prefab {objectData.Prefab.name} in BuildingSO");
            }
            else
            {
                Debug.Log($"Found BuildingData: {buildingData.displayName} for prefab {objectData.Prefab.name}");
            }

            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD} in ObjectsDatabaseSO");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            // soundFeedback.PlaySound(SoundType.wrongPlacement);
            Debug.Log("Cannot place: Invalid position");
            return;
        }
        // soundFeedback.PlaySound(SoundType.Place);
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
            grid.GetCellCenterWorld(gridPosition));

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);
        selectedData.SetBuildingBroken(gridPosition, false); // Устанавливаем здание как не поломанное

        Debug.Log($"Placed building with ID {database.objectsData[selectedObjectIndex].ID} at {gridPosition}");

        previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;

        return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition), placementValidity);
    }
}