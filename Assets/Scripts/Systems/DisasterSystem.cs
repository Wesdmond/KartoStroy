using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterSystem : Singleton<DisasterSystem>
{
    [SerializeField] private DisasterSO disasterSO;
    [SerializeField] private int mayorOfficeID = 1;
    [SerializeField] private List<int> houseBuildingIDs = new List<int> { 2 };
    [SerializeField] private float destructionThresholdPercent = 50f;
    [SerializeField] private float noDisasterChance = 20f; // Шанс отсутствия бедствия (в процентах, 0-100)
    [SerializeField] private int gameEndTurn = 5;
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
    }

    public void Randomize()
    {
        disastersRandom.Clear(); // Очищаем список для нового хода

        if (disasters == null || disasters.Count == 0)
        {
            Debug.LogWarning("DisasterSystem: No disasters available to randomize");
            return;
        }

        // Суммируем веса всех бедствий (magnitude) и добавляем шанс отсутствия бедствия
        float totalWeight = noDisasterChance / 100f; // Переводим процент в диапазон 0-1
        foreach (var disaster in disasters)
        {
            totalWeight += disaster.magnitude;
        }

        if (totalWeight <= 0f)
        {
            Debug.Log("DisasterSystem: No disaster selected (total weight is 0)");
            NotifyUIAboutDisaster(null);
            return;
        }

        // Взвешенный случайный выбор
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        // Проверяем шанс отсутствия бедствия
        cumulativeWeight += noDisasterChance / 100f;
        if (randomValue <= cumulativeWeight)
        {
            Debug.Log("DisasterSystem: No disaster selected for this turn");
            NotifyUIAboutDisaster(null);
            return;
        }

        // Выбираем бедствие на основе magnitude
        foreach (var disaster in disasters)
        {
            cumulativeWeight += disaster.magnitude;
            if (randomValue <= cumulativeWeight)
            {
                disastersRandom.Add(disaster);
                Debug.Log($"DisasterSystem: Selected disaster '{disaster.disasterName}' with magnitude {disaster.magnitude}");
                NotifyUIAboutDisaster(disaster);
                return;
            }
        }

        // Если выбор не произошел (на случай ошибок округления), ничего не выбираем
        Debug.LogWarning("DisasterSystem: No disaster selected due to weight calculation error");
        NotifyUIAboutDisaster(null);
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
        if (turnCount >= gameEndTurn)
        {
            EndGame(true);
            yield break;
        }

        // Выбираем новое бедствие для этого хода
        Randomize();

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

        // Обрабатываем выбранное бедствие (если есть)
        if (disastersRandom.Count > 0)
        {
            DisasterData disaster = disastersRandom[0];
            if (!disaster.isMajor)
            {
                bool prevented = false; // Логика предотвращения (например, через SanctuaryEffect)
                if (prevented)
                {
                    Debug.Log($"DisasterSystem: Non-major disaster '{disaster.disasterName}' prevented");
                    disastersRandom.Clear();
                }
            }

            if (disastersRandom.Count > 0) // Проверяем, не было ли предотвращено
            {
                foreach (EffectSO effect in disaster.effects)
                {
                    Debug.Log($"Performing disaster effect: {effect.name}");
                    yield return effect.Perform();
                }
            }
        }
        else
        {
            Debug.Log("DisasterSystem: No disaster effects to perform this turn");
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
            Debug.Log($"DisasterSystem: Non-major disaster '{disastersRandom[0].disasterName}' prevented");
            disastersRandom.Clear();
        }
    }

    public void SetForecastDays(int days)
    {
        forecastDays = days;
        Debug.Log($"DisasterSystem: Forecast set to {days} days");
    }

    public void ReplaceDisaster()
    {
        disastersRandom.Clear(); // Очищаем текущее бедствие
        Randomize(); // Выбираем новое
        Debug.Log("DisasterSystem: Disaster replaced");
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

    private void NotifyUIAboutDisaster(DisasterData disaster)
    {
        if (disaster == null)
        {
            DisasterViewer.Instance.ChangeSprite(null);
            Debug.Log("DisasterSystem: UI notified - No disaster this turn");
        }
        else
        {
            DisasterViewer.Instance.ChangeSprite(disaster);
            Debug.Log($"DisasterSystem: UI notified - New disaster: {disaster.disasterName} (Type: {disaster.type}, Level: {disaster.level}, Magnitude: {disaster.magnitude})");
        }
        // TODO: Реализовать уведомление UI
        // Например: UIManager.Instance.ShowDisasterNotification(disaster?.disasterName ?? "No Disaster", disaster?.description);
    }

    private void EndGame(bool isVictory, string reason = "")
    {
        gameEnded = true;
        if (isVictory)
        {
            GameEnderHandler.Instance.ShowGood();
            Debug.Log("DisasterSystem: Victory! Survived 7 turns");
        }
        else
        {
            GameEnderHandler.Instance.ShowBad();
            Debug.Log($"DisasterSystem: Game Over! Reason: {reason}");
        }
        // Реализуйте логику завершения игры, например, вызов UI или GameManager
        // GameManager.Instance.EndGame(isVictory);
    }
}