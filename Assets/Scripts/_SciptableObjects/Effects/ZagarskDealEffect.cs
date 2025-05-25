using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ZagarskDealEffect")]
public class ZagarskDealEffect : EffectSO
{
    private DisasterSystem disasterSystem;


    public override IEnumerator Perform()
    {
        disasterSystem = DisasterSystem.Instance;
        if (disasterSystem == null)
        {
            Debug.LogError($"ZagarskDealEffect ({name}): DisasterSystem is null");
            yield break;
        }

        if (disasterSystem.IsZagarskAwakeningActive)
        {
            Debug.Log("ZagarskDealEffect: Cancelling Zagarsk Awakening");
            disasterSystem.SetZagarskAwakeningActive(false);
        }
        else
        {
            Debug.Log("ZagarskDealEffect: No Zagarsk Awakening active");
        }
        yield return null;
    }
}