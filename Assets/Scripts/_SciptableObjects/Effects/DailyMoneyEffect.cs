using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/DailyMoneyEffect")]
public class DailyMoneyEffect : EffectSO
{
    [SerializeField] private int baseMoneyAmount = 20;
    [SerializeField] private int bonusPerNeighborCamp = 10;
    [SerializeField] private int portalBonus = 15;

    private Vector3Int gridPosition;
    private int objectID;
    private BuildingSystem bonusSystem;

    public void SetContext(Vector3Int position, int id, BuildingSystem system)
    {
        gridPosition = position;
        objectID = id;
        bonusSystem = system;
    }

    public override IEnumerator Perform()
    {
        if (bonusSystem == null)
        {
            Debug.LogError("DailyMoneyEffect: BonusSystem not set");
            yield break;
        }

        if (bonusSystem.IsBloodMoonActive)
        {
            Debug.Log($"DailyMoneyEffect: Blocked by Blood Moon for object ID {objectID}");
            yield break;
        }

        int totalMoney = baseMoneyAmount;

        List<int> neighborIDs = bonusSystem.GetNeighborBuildingIDs(gridPosition);
        int neighborCamps = 0;
        bool hasPortal = false;

        foreach (int neighborID in neighborIDs)
        {
            if (bonusSystem.IsFurryCamp(neighborID))
            {
                neighborCamps++;
            }
            if (bonusSystem.IsRandomPortal(neighborID))
            {
                hasPortal = true;
            }
        }

        totalMoney += neighborCamps * bonusPerNeighborCamp;

        if (hasPortal)
        {
            totalMoney += portalBonus;
        }

        PlayerSystem playerSystem = PlayerSystem.Instance;
        if (playerSystem != null)
        {
            playerSystem.ChangeMoney(totalMoney);
            Debug.Log($"Daily Money: Added {totalMoney} money (base: {baseMoneyAmount}, camps: {neighborCamps}, portal: {(hasPortal ? portalBonus : 0)})");
        }

        yield return null;
    }
}