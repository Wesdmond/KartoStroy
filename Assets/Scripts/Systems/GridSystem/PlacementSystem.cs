using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioClip correctPlacementClip, wrongPlacementClip;
    [SerializeField]
    private AudioSource source;

    private GridData floorData, furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    [SerializeField]
    private SoundFeedback soundFeedback;
    
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        gridVisualization.SetActive(false);
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           furnitureData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true) ;
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            print("Pointer over UI");
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);

    }

    private void StopPlacement()
    {
        // soundFeedback.PlaySound(SoundType.Click);
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }
    
    private void Update()
    {
        if (buildingState == null)
            return;
        // Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = mainCamera.nearClipPlane;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
        
    }
}