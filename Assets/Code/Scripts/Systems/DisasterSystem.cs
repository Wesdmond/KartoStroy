using System.Collections;
using UnityEngine;

public class DisasterSystem : MonoBehaviour
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<DisasterGA>(EnemyTurnPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DisasterGA>();
    }
    
    // Performers
    private IEnumerator EnemyTurnPerformer(DisasterGA disasterGA)
    {
        Debug.Log("Disaster Turn");
        yield return new WaitForSeconds(2f);
        Debug.Log("End Disaster Turn");
    }
}