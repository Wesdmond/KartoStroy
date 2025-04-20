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

    CardData getCard(int index){
        return cards[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
