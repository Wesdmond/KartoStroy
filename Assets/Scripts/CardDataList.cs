using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class CardDataList : Singleton<CardDataList>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public List<CardData> cards;

    void Start()
    {
        
    }

    public CardData getCard(int index){
        if (index < 0 || index >= cards.Count)
        {
            Debug.LogError("No such card id: " + index);
            return null;
        }
        return cards[index];
    }
    
    public CardData getCard(CardNames name){
        int index = (int)name;
        if (index < 0 || index >= cards.Count)
        {
            Debug.LogError("No such card name: " + name.ToString());
            return null;
        }
        return cards[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}