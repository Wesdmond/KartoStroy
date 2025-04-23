using UnityEngine;
using UnityEngine.UI;

enum CardType
{
    Building,
    Action
}



public class Card
{
    public string Title => data.Title;
    public string Description => data.Description;
    public Image Image => null;// data.Image;
    public int Energy { get; private set; }
    public int ShopPrice { get; private set; }
    private readonly CardData data;
    public Card(CardData cardData)
    {
        data = cardData;
        Energy = cardData.Energy;
        ShopPrice = cardData.ShopPrice;
    }
}