using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : Singleton<BuildingSystem>
{
    [SerializeField] private ObjectsDatabaseSO objectsDatabase;
    [SerializeField] private GridData furnitureData;
    [SerializeField] private int furryCampBuildingID = 4;
    [SerializeField] private int randomPortalBuildingID = 5;

    private bool isBloodMoonActive = false;
    private bool isZagarskAwakeningActive = false;
    private int bloodMoonTurnsRemaining = 0;
    private int zagarskTurnsRemaining = 0;
    private bool gameEnded = false;

    public bool IsBloodMoonActive => isBloodMoonActive;
    public bool IsZagarskAwakeningActive => isZagarskAwakeningActive;
    public int BloodMoonTurnsRemaining => bloodMoonTurnsRemaining;
    public int ZagarskTurnsRemaining => zagarskTurnsRemaining;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<BuildingGA>(BuildingPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<BuildingGA>();
    }

    public void ApplyBuildingEffects()
    {
        Debug.Log("Applying building effects");
        ActionSystem.Instance.Perform(new BuildingGA());
    }

    private IEnumerator BuildingPerformer(BuildingGA buildingGA)
    {
        Debug.Log("Starting Building Turn");
        Debug.Log("Building Turn: Starting effect processing");

        if (objectsDatabase == null || PlacementSystem.Instance.furnitureData == null)
        {
            Debug.LogError("BuildingSystem: Required references are null");
            if (objectsDatabase == null) print("objectsDatabase");
            if (furnitureData == null) print("furnitureData");
            yield break;
        }

        List<EffectSO> bonusesToApply = new List<EffectSO>();
        var placedObjects = PlacementSystem.Instance.furnitureData.GetAllPlacedObjects();
        Debug.Log($"Found {placedObjects.Count} placed objects");

        foreach (var placedObject in placedObjects)
        {
            if (PlacementSystem.Instance.furnitureData.IsBuildingBroken(placedObject.occupiedPositions[0]))
            {
                Debug.Log($"Skipping building at {placedObject.occupiedPositions[0]}: It is broken");
                continue;
            }

            int objectID = placedObject.ID;
            ObjectData objectData = objectsDatabase.objectsData.Find(data => data.ID == objectID);
            if (objectData == null)
            {
                Debug.LogWarning($"No ObjectData found for ID {objectID}");
                continue;
            }

            Debug.Log($"Processing building with ID {objectID}, Prefab: {objectData.Prefab.name}");

            if (objectData.bonuses == null || objectData.bonuses.Count == 0)
            {
                Debug.Log($"No bonuses for building {objectData.Name}");
                continue;
            }

            Debug.Log($"Found {objectData.bonuses.Count} bonuses for building {objectData.Name}");
            foreach (var bonus in objectData.bonuses)
            {
                print("test2");
                if (bonus is DailyMoneyEffect dailyMoney)
                {
                    dailyMoney.SetContext(placedObject.occupiedPositions[0], objectID, this);
                    Debug.Log($"Set context for DailyMoneyEffect at {placedObject.occupiedPositions[0]}");
                }
                else if (bonus is RandomBonusEffect randomBonus)
                {
                    randomBonus.SetContext(placedObject.occupiedPositions[0], objectID, this);
                    Debug.Log($"Set context for RandomBonusEffect at {placedObject.occupiedPositions[0]}");
                }
                bonusesToApply.Add(bonus);
            }
        }

        Debug.Log($"Applying {bonusesToApply.Count} effects");
        foreach (var effect in bonusesToApply)
        {
            Debug.Log($"Performing effect: {effect.name}");
            yield return effect.Perform();
        }

        // Уменьшаем счетчики ходов для катастроф
        if (isBloodMoonActive)
        {
            bloodMoonTurnsRemaining--;
            Debug.Log($"BloodMoon: Turns remaining: {bloodMoonTurnsRemaining}");
            if (bloodMoonTurnsRemaining <= 0)
            {
                SetBloodMoonActive(false);
            }
        }

        if (isZagarskAwakeningActive && !gameEnded)
        {
            zagarskTurnsRemaining--;
            Debug.Log($"ZagarskAwakening: Turns remaining: {zagarskTurnsRemaining}");
            if (zagarskTurnsRemaining <= 0)
            {
                EndGame();
            }
        }

        Debug.Log("Building Turn: Effect processing completed");
        Debug.Log("Start Player Turn");
    }

    public bool IsFurryCamp(int objectID)
    {
        return objectID == furryCampBuildingID;
    }

    public bool IsRandomPortal(int objectID)
    {
        return objectID == randomPortalBuildingID;
    }

    public List<int> GetNeighborBuildingIDs(Vector3Int gridPosition)
    {
        List<Vector3Int> neighborPositions = PlacementSystem.Instance.furnitureData.GetNeighborPositions(gridPosition);
        List<int> neighborIDs = new List<int>();
        foreach (var pos in neighborPositions)
        {
            int index = PlacementSystem.Instance.furnitureData.GetRepresentationIndex(pos);
            if (index != -1)
            {
                var placedObject = PlacementSystem.Instance.furnitureData.GetAllPlacedObjects().Find(p => p.PlacedObjectIndex == index);
                if (placedObject != null)
                {
                    neighborIDs.Add(placedObject.ID);
                }
            }
        }
        Debug.Log($"Found {neighborIDs.Count} neighbors at {gridPosition}");
        return neighborIDs;
    }

    public void SetBloodMoonActive(bool active, int turns = 3)
    {
        isBloodMoonActive = active;
        bloodMoonTurnsRemaining = active ? turns : 0;
        Debug.Log($"BuildingSystem: Blood Moon {(active ? $"activated with {turns} turns" : "deactivated")}");
    }

    public void SetZagarskAwakeningActive(bool active, int turns = 3)
    {
        isZagarskAwakeningActive = active;
        zagarskTurnsRemaining = active ? turns : 0;
        Debug.Log($"BuildingSystem: Zagarsk Awakening {(active ? $"activated with {turns} turns" : "deactivated")}");
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log("BuildingSystem: Game Over due to Zagarsk Awakening!");
        // Реализуйте логику завершения игры, например, вызов UI или GameManager
    }
}