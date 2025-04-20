using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Energy { get; private set; }
    [field: SerializeField] public int ShopPrice { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }    
}