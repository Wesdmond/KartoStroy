using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/RandomBonusEffect")]
public class RandomBonusEffect : EffectSO
{
    [SerializeField] private float moneyChance = 50f; // Шанс получить деньги
    [SerializeField] private float cardChance = 30f; // Шанс получить карту
    [SerializeField] private int minMoney = 10; // Минимальное количество денег
    [SerializeField] private int maxMoney = 50; // Максимальное количество денег

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
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= moneyChance)
        {
            int moneyAmount = Random.Range(minMoney, maxMoney + 1);
            PlayerSystem playerSystem = PlayerSystem.Instance;
            if (playerSystem != null)
            {
                playerSystem.ChangeMoney(moneyAmount);
                Debug.Log($"Random Bonus: Added {moneyAmount} money");
            }
        }
        else if (randomValue <= moneyChance + cardChance)
        {
            HorizontalCardHolder cardHolder = FindObjectOfType<HorizontalCardHolder>();
            if (cardHolder != null)
            {
                int randomCardIndex = Random.Range(0, (int)CardNames.FreezingTime + 1);
                cardHolder.AddCard((CardNames)randomCardIndex);
                Debug.Log($"Random Bonus: Added card {(CardNames)randomCardIndex}");
            }
        }
        else
        {
            Debug.Log("Random Bonus: No bonus applied");
        }

        yield return null;
    }
}