using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private PlacementSystem placementSystem;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;
    private int idForPlace = 0;

    private void Update()
    {
        var input = Input.inputString;
        
        if (!string.IsNullOrEmpty(input))
        {
            switch (input)
            {
                case "e": placementSystem.StartPlacement(1); break;
                case "q": placementSystem.StartRemoving(); break;
                case "1": ChangePlacementID(1); break;
                case "2": ChangePlacementID(2); break;
                case "3": ChangePlacementID(3); break;
                case "4": ChangePlacementID(4); break;
                case "R": break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }
           
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Tab))
        {
            OnExit?.Invoke();
        }
        
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    private void ChangePlacementID(int newID)
    {
        placementSystem.StartPlacement(newID);
    }
}