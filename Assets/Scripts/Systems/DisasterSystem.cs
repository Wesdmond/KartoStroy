using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterSystem : MonoBehaviour
{
    [SerializeField] private DisasterSO disasterSO;
    private List<DisasterData> disasters;
    private List<DisasterData> disastersRandom = new();
    private int forecastDays = 1;

    void Start()
    {
        if (disasterSO != null) disasters = new(disasterSO.disasterData);
        else Debug.LogError("DisasterSystem: disasterSO is null");
        Randomize();
    }

    public void Randomize()
    {
        int counter = disasters.Count;
        for (int i = 0; i < counter; ++i)
        {
            int random = Random.Range(0, disasters.Count);
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

    private IEnumerator EnemyTurnPerformer(DisasterGA disasterGA)
    {
        Debug.Log("Disaster Turn");
        yield return new WaitForSeconds(2f);
        if (disastersRandom == null)
        {
            Debug.LogError("DisasterSystem: disastersRandom is null");
            yield break;
        }

        foreach (DisasterData disaster in disastersRandom)
        {
            if (!disaster.isMajor)
            {
                bool prevented = false; // Здесь можно интегрировать логику святилища
                if (prevented) continue;
            }

            foreach (EffectSO effect in disaster.effects)
            {
                yield return effect.Perform();
            }
        }
        Debug.Log("End Disaster Turn");
    }

    public void PreventNonMajorDisaster()
    {
        if (disastersRandom.Count > 0 && !disastersRandom[0].isMajor)
        {
            disastersRandom.RemoveAt(0);
            Debug.Log("DisasterSystem: Non-major disaster prevented");
        }
    }
    
    public void SetForecastDays(int days)
    {
        forecastDays = days;
        // Здесь можно обновить UI или отправить событие для отображения
        Debug.Log($"DisasterSystem: Forecast set to {days} days");
    }

    public void ReplaceDisaster()
    {
        if (disastersRandom.Count > 0)
        {
            disastersRandom.RemoveAt(0);
            if (disasters.Count > 0)
            {
                int random = Random.Range(0, disasters.Count);
                disastersRandom.Insert(0, disasters[random]);
                disasters.RemoveAt(random);
                Debug.Log("DisasterSystem: Disaster replaced");
            }
        }
    }
}