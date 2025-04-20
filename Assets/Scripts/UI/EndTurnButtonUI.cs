using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{
    public void OnClick()
    {
        DisasterGA disasterGA = new();
        ActionSystem.Instance.Perform(disasterGA);
    }
}   