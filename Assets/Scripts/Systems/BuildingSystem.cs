using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : Singleton<BuildingSystem>
{
    [SerializeField] private BuildingSO buildingSO;
    [SerializeField] private ObjectsDatabaseSO objectsDatabase;
    [SerializeField] private GridData furnitureData; // Данные о размещённых зданиях
    [SerializeField] private int furryCampBuildingID = 3; // ID "Лагерь фурри"
    [SerializeField] private int randomPortalBuildingID = 4; // ID "Портал случайностей"

    void Awake()
    {
        // furnitureData = PlacementSystem.Instance.furnitureData;
    }
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<BuildingGA>(BuildingPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<BuildingGA>();
    }

    // Явный метод для вызова эффектов
    public void ApplyBuildingEffects()
    {
        Debug.Log("Applying building effects");
        ActionSystem.Instance.Perform(new BuildingGA());
    }

    private IEnumerator BuildingPerformer(BuildingGA buildingGA)
    {
        Debug.Log("Building Turn: Starting effect processing");

        if (buildingSO == null || objectsDatabase == null || PlacementSystem.Instance.furnitureData == null)
        {
            Debug.LogError("BuildingSystem: Required references are null");
            if (buildingSO == null) print("buildingSO");
            if (objectsDatabase == null) print("objectsDatabase");
            if (furnitureData == null) print("furnitureData");
            yield break;
        }

        // Собираем эффекты от всех неполоманных зданий
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

            // BuildingData buildingData = buildingSO.buildingsData.;
            // if (buildingData == null)
            // {
            //     Debug.LogWarning($"No BuildingData found for prefab {objectData.Prefab.name} in BuildingSO");
            //     continue;
            // }

            if (objectData.bonuses == null || objectData.bonuses.Count == 0)
            {
                Debug.Log($"No bonuses for building {objectData.Name}");
                continue;
            }

            Debug.Log($"Found {objectData.bonuses.Count} bonuses for building {objectData.Name}");
            foreach (var bonus in objectData.bonuses)
            {
                print("test2");
                // Передаём контекст для эффектов, зависящих от соседства
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

        // Выполняем все эффекты
        Debug.Log($"Applying {bonusesToApply.Count} effects");
        foreach (var effect in bonusesToApply)
        {
            Debug.Log($"Performing effect: {effect.name}");
            yield return effect.Perform();
        }

        Debug.Log("Building Turn: Effect processing completed");
    }

    // Проверка, является ли здание лагерем фурри
    public bool IsFurryCamp(int objectID)
    {
        return objectID == furryCampBuildingID;
    }

    // Проверка, является ли здание порталом случайностей
    public bool IsRandomPortal(int objectID)
    {
        return objectID == randomPortalBuildingID;
    }

    // Получение ID соседних зданий
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
}