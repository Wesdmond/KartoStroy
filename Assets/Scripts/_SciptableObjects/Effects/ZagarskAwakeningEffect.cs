using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ZagarskAwakeningEffect")]
public class ZagarskAwakeningEffect : EffectSO
{
    private DisasterSystem disasterSystem;
    

    public override IEnumerator Perform()
    {
        disasterSystem = DisasterSystem.Instance;
        if (disasterSystem == null)
        {
            Debug.LogError($"ZagarskAwakeningEffect ({name}): DisasterSystem is null");
            yield break;
        }

        Debug.Log("ZagarskAwakeningEffect: Activating Zagarsk Awakening");
        disasterSystem.SetZagarskAwakeningActive(true);
        yield return null;
    }
}