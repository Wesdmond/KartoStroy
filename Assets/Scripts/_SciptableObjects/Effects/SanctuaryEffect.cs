using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/SanctuaryEffect")]
public class SanctuaryEffect : EffectSO
{
    [SerializeField] private float basePreventionChance = 30f; // Базовый шанс предотвращения (для 1 святилища)
    [SerializeField] private float chanceIncreasePerSanctuary = 10f; // Увеличение шанса за каждое дополнительное святилище

    public override IEnumerator Perform()
    {
        BonusSystem bonusSystem = FindObjectOfType<BonusSystem>();
        if (bonusSystem == null)
        {
            Debug.LogError("SanctuaryEffect: BonusSystem not found");
            yield break;
        }

        // Подсчитываем количество святилищ
        int sanctuaryCount = 0;
        foreach (var placedObject in bonusSystem.GetPlacedBuildings())
        {
            if (bonusSystem.IsSanctuary(placedObject.ID)) // Предполагается метод в BonusSystem
            {
                sanctuaryCount++;
            }
        }

        float preventionChance = basePreventionChance + (sanctuaryCount - 1) * chanceIncreasePerSanctuary;
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= preventionChance)
        {
            DisasterSystem disasterSystem = FindObjectOfType<DisasterSystem>();
            if (disasterSystem != null)
            {
                // disasterSystem.PreventNonMajorDisaster(); // TODO
                Debug.Log($"Sanctuary: Prevented non-major disaster with {preventionChance}% chance");
            }
        }
        else
        {
            Debug.Log($"Sanctuary: Failed to prevent disaster ({randomValue} > {preventionChance}%)");
        }

        yield return null;
    }
}