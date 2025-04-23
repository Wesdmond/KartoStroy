using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum CardNames
{
    DevilLuck = 0,
    Redevelopment = 1,
    CleansingRitual = 2,
    Redbull = 3,
    FreezingTime = 4
}

[CreateAssetMenu(fileName = "Data/Card", menuName = "CardData")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Energy { get; private set; }
    [field: SerializeField] public int ShopPrice { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    public List<EffectSO> effects;
}