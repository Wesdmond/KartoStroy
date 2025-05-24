using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ZagarskDealEffect")]
public class ZagarskDealEffect : EffectSO
{
    private BuildingSystem buildingSystem;

    public override IEnumerator Perform()
    {
        buildingSystem = BuildingSystem.Instance;
        if (buildingSystem == null)
        {
            Debug.LogError($"ZagarskDealEffect ({name}): BuildingSystem is null");
            yield break;
        }

        if (buildingSystem.IsZagarskAwakeningActive)
        {
            Debug.Log("ZagarskDealEffect: Cancelling Zagarsk Awakening");
            buildingSystem.SetZagarskAwakeningActive(false);
        }
        else
        {
            Debug.Log("ZagarskDealEffect: No Zagarsk Awakening active");
        }
        yield return null;
    }
}