using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterSystem : MonoBehaviour
{
    [SerializeField] private DisasterSO disasterSO;

    private List<DisasterData> disasters;

    private List<DisasterData> disastersRandom = new(); // ingame meow meow

    void Start()
    {
        if (disasterSO != null) disasters = new(disasterSO.disasterData);
        else Debug.LogError("DisasterSystem: disasterSO is null");
        Randomize();
    }
    public void Randomize()
    {
        int counter = disasters.Count;
        for (int i = 0; i < counter; ++i) {
            int random = UnityEngine.Random.Range(0, disasters.Count);
            disastersRandom.Add(disasters[random]);
            disasters.RemoveAt(random);
        }
        disasters = new(disasterSO.disasterData);
    }

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