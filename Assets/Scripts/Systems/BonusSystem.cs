using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSystem : Singleton<BonusSystem>
{
    [SerializeField] private BuildingSO buildingSO;
    [SerializeField] private GridData furnitureData;
    [SerializeField] private ObjectsDatabaseSO objectsDatabase;
    [SerializeField] private int sanctuaryBuildingID = 1; // ID здания "Святилище"
    [SerializeField] private int furryCampBuildingID = 2; // ID здания "Лагерь фурри"
    [SerializeField] private int randomPortalBuildingID = 3; // ID здания "Портал случайностей"

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<BuildingBonusGA>(BuildingBonusPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<BuildingBonusGA>();
    }

    public void ApplyBuildingBonuses()
    {
        if (buildingSO == null || furnitureData == null || objectsDatabase == null)
        {
            Debug.LogError("BonusSystem: Required references are null");
            return;
        }

        List<EffectSO> bonusesToApply = new List<EffectSO>();
        foreach (var placedObject in furnitureData.GetAllPlacedObjects())
        {
            int objectID = placedObject.ID;
            ObjectData objectData = objectsDatabase.objectsData.Find(data => data.ID == objectID);
            if (objectData != null)
            {
                BuildingData buildingData = buildingSO.buildingsData.Find(b => b.prefab == objectData.Prefab);
                if (buildingData != null && buildingData.bonuses != null)
                {
                    foreach (var bonus in buildingData.bonuses)
                    {
                        // Передаём позицию и ID для проверки соседства
                        if (bonus is DailyMoneyEffect dailyMoney)
                        {
                            // dailyMoney.SetContext(placedObject.occupiedPositions[0], objectID, this);
                        }
                        else if (bonus is RandomBonusEffect randomBonus)
                        {
                            // randomBonus.SetContext(placedObject.occupiedPositions[0], objectID, this);
                        }
                        bonusesToApply.Add(bonus);
                    }
                }
            }
        }

        BuildingBonusGA bonusAction = new BuildingBonusGA(bonusesToApply);
        ActionSystem.Instance.Perform(bonusAction);
    }

    private IEnumerator BuildingBonusPerformer(BuildingBonusGA bonusAction)
    {
        Debug.Log("Applying Building Bonuses");
        foreach (EffectSO effect in bonusAction.Bonuses)
        {
            yield return effect.Perform();
        }
        Debug.Log("End Building Bonuses");
    }

    public List<PlacementData> GetPlacedBuildings()
    {
        return furnitureData.GetAllPlacedObjects();
    }

    public bool IsSanctuary(int objectID)
    {
        return objectID == sanctuaryBuildingID;
    }

    public bool IsFurryCamp(int objectID)
    {
        return objectID == furryCampBuildingID;
    }

    public bool IsRandomPortal(int objectID)
    {
        return objectID == randomPortalBuildingID;
    }

    // Проверка соседних зданий
    public List<int> GetNeighborBuildingIDs(Vector3Int gridPosition)
    {
        List<Vector3Int> neighborPositions = furnitureData.GetNeighborPositions(gridPosition);
        List<int> neighborIDs = new List<int>();
        foreach (var pos in neighborPositions)
        {
            int index = furnitureData.GetRepresentationIndex(pos);
            if (index != -1)
            {
                var placedObject = furnitureData.GetAllPlacedObjects().Find(p => p.PlacedObjectIndex == index);
                if (placedObject != null)
                {
                    neighborIDs.Add(placedObject.ID);
                }
            }
        }
        return neighborIDs;
    }
}

public class BuildingBonusGA : GameAction
{
    public List<EffectSO> Bonuses { get; private set; }

    public BuildingBonusGA(List<EffectSO> bonuses)
    {
        Bonuses = bonuses;
    }
}