using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/SanctuaryEffect")]
public class SanctuaryEffect : EffectSO
{
    [SerializeField] private float basePreventionChance = 30f; // Базовый шанс предотвращения (для 1 святилища)
    [SerializeField] private float chanceIncreasePerSanctuary = 10f; // Увеличение шанса за каждое дополнительное святилище

    private BuildingSystem buildingSystem;
    private DisasterSystem disasterSystem;

    public override IEnumerator Perform()
    {
        buildingSystem = BuildingSystem.Instance;
        disasterSystem = DisasterSystem.Instance;
        if (buildingSystem == null || disasterSystem == null)
        {
            Debug.LogError($"SanctuaryEffect ({name}): BuildingSystem or DisasterSystem is null");
            yield break;
        }

        // Подсчитываем количество святилищ
        int sanctuaryCount = 0;
        foreach (var placedObject in PlacementSystem.Instance.furnitureData.GetAllPlacedObjects())
        {
            if (buildingSystem.IsSanctuary(placedObject.ID) && !PlacementSystem.Instance.furnitureData.IsBuildingBroken(placedObject.occupiedPositions[0]))
            {
                sanctuaryCount++;
            }
        }

        if (sanctuaryCount == 0)
        {
            Debug.Log("SanctuaryEffect: No active sanctuaries found");
            yield break;
        }

        float preventionChance = basePreventionChance + (sanctuaryCount - 1) * chanceIncreasePerSanctuary;
        float randomValue = Random.Range(0f, 100f);
    
        if (randomValue <= preventionChance)
        {
            disasterSystem.PreventNonMajorDisaster();
            Debug.Log($"SanctuaryEffect: Prevented non-major disaster with {preventionChance}% chance (sanctuaries: {sanctuaryCount})");
        }
        else
        {
            Debug.Log($"SanctuaryEffect: Failed to prevent disaster ({randomValue:F1} > {preventionChance}%)");
        }

        yield return null;
    }
}