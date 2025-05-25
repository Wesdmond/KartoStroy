using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterSystem : Singleton<DisasterSystem>
{
    [SerializeField] private DisasterSO disasterSO;
    [SerializeField] private int mayorOfficeID = 1;
    [SerializeField] private List<int> houseBuildingIDs = new List<int> { 2 };
    [SerializeField] private float destructionThresholdPercent = 50f;
    private List<DisasterData> disasters;
    private List<DisasterData> disastersRandom = new();
    private int forecastDays = 1;
    private int turnCount = 0;
    private bool gameEnded = false;
    private bool isBloodMoonActive = false;
    private int lastBloodMoonTurn = -1;
    private bool isZagarskAwakeningActive = false;
    private int lastZagarskAwakeningTurn = -1;
    private const int MAX_DISASTER_TURNS = 3;

    public bool IsBloodMoonActive => isBloodMoonActive;
    public bool IsZagarskAwakeningActive => isZagarskAwakeningActive;

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
        if (gameEnded) yield break;

        turnCount++;
        Debug.Log($"Disaster Turn: Turn {turnCount}");

        // Проверяем условие победы
        if (turnCount >= 7)
        {
            EndGame(true);
            yield break;
        }

        // Получаем состояние зданий до катастроф
        BuildingSystem buildingSystem = BuildingSystem.Instance;
        if (buildingSystem == null)
        {
            Debug.LogError("DisasterSystem: BuildingSystem is null");
            yield break;
        }

        var placedObjects = PlacementSystem.Instance.furnitureData.GetAllPlacedObjects();
        int initialHouseCount = 0;
        foreach (var obj in placedObjects)
        {
            if (houseBuildingIDs.Contains(obj.ID) && !PlacementSystem.Instance.furnitureData.IsBuildingBroken(obj.occupiedPositions[0]))
            {
                initialHouseCount++;
            }
        }

        Debug.Log("Disaster Turn: Starting effect processing");
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
                bool prevented = false;
                if (prevented) continue;
            }

            foreach (EffectSO effect in disaster.effects)
            {
                Debug.Log($"Performing disaster effect: {effect.name}");
                yield return effect.Perform();
            }
        }

        Debug.Log("End Disaster Turn");

        // Проверяем состояние мэрии
        bool mayorOfficeBroken = false;
        foreach (var obj in placedObjects)
        {
            if (obj.ID == mayorOfficeID && PlacementSystem.Instance.furnitureData.IsBuildingBroken(obj.occupiedPositions[0]))
            {
                mayorOfficeBroken = true;
                break;
            }
        }

        if (mayorOfficeBroken)
        {
            EndGame(false, "Mayor Office is broken");
            yield break;
        }

        // Проверяем процент разрушенных домов
        int finalHouseCount = 0;
        foreach (var obj in placedObjects)
        {
            if (houseBuildingIDs.Contains(obj.ID) && !PlacementSystem.Instance.furnitureData.IsBuildingBroken(obj.occupiedPositions[0]))
            {
                finalHouseCount++;
            }
        }

        int housesDestroyed = initialHouseCount - finalHouseCount;
        float destructionPercent = initialHouseCount > 0 ? (housesDestroyed / (float)initialHouseCount) * 100f : 0f;
        if (destructionPercent >= destructionThresholdPercent)
        {
            EndGame(false, $"{destructionPercent:F1}% of houses destroyed in one turn");
            yield break;
        }

        // Проверяем длительность катастроф
        if (isBloodMoonActive && turnCount >= lastBloodMoonTurn + MAX_DISASTER_TURNS)
        {
            SetBloodMoonActive(false);
            Debug.Log("DisasterSystem: Blood Moon deactivated due to turn limit");
        }

        if (isZagarskAwakeningActive && turnCount >= lastZagarskAwakeningTurn + MAX_DISASTER_TURNS)
        {
            EndGame(false, "Zagarsk Awakening completed");
            yield break;
        }
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

    public void SetBloodMoonActive(bool active)
    {
        isBloodMoonActive = active;
        lastBloodMoonTurn = active ? turnCount : -1;
        Debug.Log($"DisasterSystem: Blood Moon {(active ? $"activated on turn {turnCount}" : "deactivated")}");
    }

    public void SetZagarskAwakeningActive(bool active)
    {
        isZagarskAwakeningActive = active;
        lastZagarskAwakeningTurn = active ? turnCount : -1;
        Debug.Log($"DisasterSystem: Zagarsk Awakening {(active ? $"activated on turn {turnCount}" : "deactivated")}");
    }

    private void EndGame(bool isVictory, string reason = "")
    {
        gameEnded = true;
        if (isVictory)
        {
            Debug.Log("DisasterSystem: Victory! Survived 7 turns");
        }
        else
        {
            Debug.Log($"DisasterSystem: Game Over! Reason: {reason}");
        }
        // Реализуйте логику завершения игры, например, вызов UI или GameManager
        // GameManager.Instance.EndGame(isVictory);
    }
}