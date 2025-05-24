using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/PurifyingRitualEffect")]
public class PurifyingRitualEffect : EffectSO
{
    private BuildingSystem buildingSystem;

    public override IEnumerator Perform()
    {
        buildingSystem = BuildingSystem.Instance;
        if (buildingSystem == null)
        {
            Debug.LogError($"PurifyingRitualEffect ({name}): BuildingSystem is null");
            yield break;
        }

        if (buildingSystem.IsBloodMoonActive)
        {
            Debug.Log("PurifyingRitualEffect: Cancelling Blood Moon");
            buildingSystem.SetBloodMoonActive(false);
        }
        else
        {
            Debug.Log("PurifyingRitualEffect: No Blood Moon active");
        }
        yield return null;
    }
}