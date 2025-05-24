using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/BloodMoonEffect")]
public class BloodMoonEffect : EffectSO
{
    [SerializeField] private int maxTurns = 3;
    private BuildingSystem buildingSystem;

    public override IEnumerator Perform()
    {
        buildingSystem = BuildingSystem.Instance;
        if (buildingSystem == null)
        {
            Debug.LogError($"BloodMoonEffect ({name}): BuildingSystem is null");
            yield break;
        }

        Debug.Log("BloodMoonEffect: Activating Blood Moon");
        buildingSystem.SetBloodMoonActive(true, maxTurns);

        while (buildingSystem.IsBloodMoonActive && buildingSystem.BloodMoonTurnsRemaining > 0)
        {
            yield return WaitForBuildingTurn();
        }

        if (buildingSystem.IsBloodMoonActive)
        {
            buildingSystem.SetBloodMoonActive(false);
            Debug.Log("BloodMoonEffect: Deactivated due to turn limit");
        }
        else
        {
            Debug.Log("BloodMoonEffect: Deactivated by player action");
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