using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ZagarskAwakeningEffect")]
public class ZagarskAwakeningEffect : EffectSO
{
    [SerializeField] private int maxTurns = 3;
    private BuildingSystem buildingSystem;

    public override IEnumerator Perform()
    {
        buildingSystem = BuildingSystem.Instance;
        if (buildingSystem == null)
        {
            Debug.LogError($"ZagarskAwakeningEffect ({name}): BuildingSystem is null");
            yield break;
        }

        Debug.Log("ZagarskAwakeningEffect: Activating Zagarsk Awakening");
        buildingSystem.SetZagarskAwakeningActive(true, maxTurns);

        while (buildingSystem.IsZagarskAwakeningActive && buildingSystem.ZagarskTurnsRemaining > 0)
        {
            yield return WaitForBuildingTurn();
        }

        if (!buildingSystem.IsZagarskAwakeningActive)
        {
            Debug.Log("ZagarskAwakeningEffect: Deactivated by player action");
        }
    }

    private IEnumerator WaitForBuildingTurn()
    {
        bool buildingTurnCompleted = false;
        ActionSystem.Instance.Perform(new BuildingGA(), () => buildingTurnCompleted = true);
        while (!buildingTurnCompleted)
        {
            yield return null;
        }
    }
}