using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardData cardData1;
    [SerializeField] private CardData cardData2;
    [SerializeField] private CardData cardData3;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Card card = new(cardData1);
            AddCardToHand(card);
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Card card = new(cardData2);
            AddCardToHand(card);
        }
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Card card = new(cardData3);
            AddCardToHand(card);
        }
    }

    private void AddCardToHand(Card card)
    {
        if (handView.IsHandFull()) return;
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);
        
        StartCoroutine(handView.AddCard(cardView));
    }
}