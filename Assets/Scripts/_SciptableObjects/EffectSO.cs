using UnityEngine;

[CreateAssetMenu(fileName = "EffectSO", menuName = "Scriptable Objects/EffectSO")]
public abstract class EffectSO : ScriptableObject
{
    public abstract void Perform();
}