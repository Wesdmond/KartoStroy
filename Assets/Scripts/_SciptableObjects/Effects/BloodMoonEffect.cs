using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/BloodMoonEffect")]
public class BloodMoonEffect : EffectSO
{
    private DisasterSystem disasterSystem;


    public override IEnumerator Perform()
    {
        disasterSystem = DisasterSystem.Instance;
        if (disasterSystem == null)
        {
            Debug.LogError($"BloodMoonEffect ({name}): DisasterSystem is null");
            yield break;
        }

        Debug.Log("BloodMoonEffect: Activating Blood Moon");
        disasterSystem.SetBloodMoonActive(true);
        yield return null;
    }
}