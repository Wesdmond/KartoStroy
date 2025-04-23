using UnityEngine;

[CreateAssetMenu(menuName = "Effect/GiveEnergy")]
public class GiveEnergyEffectSO : EffectSO
{
    public int amount = 3;
    
    public override void Perform()
    {
        Debug.Log("Give Energy to player");
        PlayerSystem.Instance.ChangeEnergy(PlayerSystem.Instance.GetEnergy() + amount);
    }
}