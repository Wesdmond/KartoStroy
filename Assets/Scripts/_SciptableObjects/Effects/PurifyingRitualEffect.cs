using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/PurifyingRitualEffect")]
public class PurifyingRitualEffect : EffectSO
{
    private DisasterSystem disasterSystem;

    public override IEnumerator Perform()
    {
        disasterSystem = DisasterSystem.Instance;
        if (disasterSystem == null)
        {
            Debug.LogError($"PurifyingRitualEffect ({name}): DisasterSystem is null");
            yield break;
        }

        if (disasterSystem.IsBloodMoonActive)
        {
            Debug.Log("PurifyingRitualEffect: Cancelling Blood Moon");
            disasterSystem.SetBloodMoonActive(false);
        }
        else
        {
            Debug.Log("PurifyingRitualEffect: No Blood Moon active");
        }
        yield return null;
    }
}