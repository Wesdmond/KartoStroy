using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
public class ShopSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [field: SerializeField] public int Coin;
    public int price = 50;
    public int Cards;

    public Text Coin_text;
    public Text Cards_text;
    void Start()
    {
        Coin = PlayerPrefs.GetInt("Coin");
        Cards = PlayerPrefs.GetInt("Cards");
        Coin_text.text = Coin.ToString();
        Cards_text.text = Cards.ToString();
    }

    public void BuyCard(int CardType)
    {
        if ((Coin >= price) && (Cards < 5))
        {
            Coin -= price;
            Coin_text.text = Coin.ToString();

            Cards += 1;
            switch (CardType)
            {
                case 0:
                    // add CardType0 here
                    break;
                case 1:
                    // add CardType1 here
                    break;
                case 2:
                    // add CardType2 here
                    break;
                default:
                    break;
            }

            Cards_text.text = Cards.ToString();
            PlayerPrefs.SetInt("Coin", Coin);
            PlayerPrefs.SetInt("Cards", Cards);

        }
        else
        {
            //not enough money
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
