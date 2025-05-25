using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/DevilsLuckEffect")]
public class DevilsLuckEffect : EffectSO
{
    private DisasterSystem disasterSystem;
    

    public override IEnumerator Perform()
    {
        disasterSystem = DisasterSystem.Instance;
        if (disasterSystem == null)
        {
            Debug.LogError($"DevilsLuckEffect ({name}): DisasterSystem is null");
            yield break;
        }

        Debug.Log("DevilsLuckEffect: Cancelling all active disasters");
        disasterSystem.SetBloodMoonActive(false);
        disasterSystem.SetZagarskAwakeningActive(false);
        yield return null;
    }
}