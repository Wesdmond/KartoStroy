using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/DevilsLuckEffect")]
public class DevilsLuckEffect : EffectSO
{
    private BuildingSystem buildingSystem;

    public override IEnumerator Perform()
    {
        buildingSystem = BuildingSystem.Instance;
        if (buildingSystem == null)
        {
            Debug.LogError($"DevilsLuckEffect ({name}): BuildingSystem is null");
            yield break;
        }

        Debug.Log("DevilsLuckEffect: Cancelling all active disasters");
        buildingSystem.SetBloodMoonActive(false);
        buildingSystem.SetZagarskAwakeningActive(false);
        yield return null;
    }
}