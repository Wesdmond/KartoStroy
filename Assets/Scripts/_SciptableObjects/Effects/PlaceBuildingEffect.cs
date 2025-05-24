using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/PlaceBuildingEffect")]
public class PlaceBuildingEffect : EffectSO
{
    [SerializeField] private int buildingId; // ID здания для размещения
    [SerializeField] private float placementDuration = 0f; // Длительность эффекта (для анимации)
    // private BuildingSystem buildingSystem;

    private void Awake()
    {
        // Находим BuildingSystem в сцене
        // buildingSystem = FindObjectOfType<BuildingSystem>();
        // if (buildingSystem == null)
        // {s
        //     Debug.LogError("PlaceBuildingEffect: BuildingSystem not found in scene");
        // }
    }

    public override IEnumerator Perform()
    {
        // if (buildingSystem == null)
        // {
        //     Debug.LogError($"PlaceBuildingEffect ({name}): BuildingSystem is null");
        //     yield break;
        // }

        Debug.Log($"PlaceBuildingEffect: Attempting to place building with ID {buildingId}");

        // Вызываем метод размещения здания в BuildingSystem
        // bool success = buildingSystem.PlaceBuilding(buildingId);
        // bool success = PlacementSystem.Instance.StartPlacement(buildingId);
        PlacementSystem.Instance.StartPlacement(buildingId);
        yield return null;
        // if (success)
        // {
        //     Debug.Log($"PlaceBuildingEffect: Successfully placed building with ID {buildingId}");
        //     // Симулируем анимацию размещения
        //     yield return new WaitForSeconds(placementDuration);
        // }
        // else
        // {
        //     Debug.LogWarning($"PlaceBuildingEffect: Failed to place building with ID {buildingId}");
        // }
    }
}