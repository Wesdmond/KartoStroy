using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class HorizontalCardHolder : MonoBehaviour
{

    [SerializeField] private CardView selectedCard;
    [SerializeReference] private CardView hoveredCard;

    [SerializeField] private GameObject slotPrefab;
    private RectTransform rect;

    [Header("Spawn Settings")]
    [SerializeField] private int muxHandCard = 7;
    public List<CardView> cards;


    bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;

    [Header("Our Custom Settings")] 

    [SerializeField] private bool isHandFillAtStart = true;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (isHandFillAtStart)
            FillHand();
    }

    private void BeginDrag(CardView card)
    {
        selectedCard = card;
    }


    void EndDrag(CardView card)
    {
        if (selectedCard == null)
            return;

        selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0,selectedCard.selectionOffset,0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;

    }

    void CardPointerEnter(CardView card)
    {
        hoveredCard = card;
    }

    void CardPointerExit(CardView card)
    {
        hoveredCard = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (hoveredCard != null)
            {
                Destroy(hoveredCard.transform.parent.gameObject);
                cards.Remove(hoveredCard);

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (CardView card in cards)
            {
                card.Deselect();
            }
        }

        if (selectedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {

            if (selectedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (selectedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = selectedCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cards[index].cardVisual == null)
            return;

        bool swapIsRight = cards[index].ParentIndex() > selectedCard.ParentIndex();
        cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (CardView card in cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }
    }

    public void DeleteCard(int index)
    {
        Destroy(cards[index].transform.parent.gameObject);
        cards.Remove(hoveredCard);
    }

    public void DeleteAllCards()
    {
        foreach (CardView card in cards)
        {
            Destroy(card.transform.parent.gameObject);
        }
        cards.Clear();
    }
    
    public bool IsHandFull()
    {
        return cards.Count >= muxHandCard;
    }

    public void AddCard(CardNames cardName)
    {
        if (IsHandFull())
        {
            Debug.LogWarning("Hand is full");
            return;
        }
        CardView card = Instantiate(slotPrefab, transform).GetComponentInChildren<CardView>();
        if (card == null)
        {
            Debug.LogError("Card could not be instantiated");
            return;
        }
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);
        card.name = cards.Count.ToString();
        
        CardData cardData = CardDataList.Instance.getCard(cardName);
        if (cardData == null)
        {
            Debug.LogError("Card is null");
            return;
        }
        card.title = cardData.Title;
        card.description = cardData.Description;
        card.energyCost = cardData.Energy.ToString();
        card.imageSR = cardData.Sprite;
        cards.Add(card);
    }

    public void FillHand(int cardsCount = int.MinValue)
    {
        if (cards.Count != 0)
        {
            DeleteAllCards();
        }
        if (cardsCount == int.MinValue) cardsCount = muxHandCard;
        for (int i = 0; i < cardsCount; i++)
        {
            Instantiate(slotPrefab, transform);
        }
        
        cards = GetComponentsInChildren<CardView>().ToList();

        int cardCount = 0;

        CardDataList cardDataList = CardDataList.Instance;
        CardData cardData = null;
        foreach (CardView card in cards)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.name = cardCount.ToString();
            
            int random = UnityEngine.Random.Range(0, cardDataList.cards.Count);
            cardData = cardDataList.getCard(random);
            if (cardData == null)
            {
                Debug.LogError("Card is null");
                return;
            }
            card.title = cardData.Title;
            card.description = cardData.Description;
            card.energyCost = cardData.Energy.ToString();
            card.imageSR = cardData.Sprite;
            cardCount++;
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].cardVisual != null)
                    cards[i].cardVisual.UpdateIndex(transform.childCount);
            }
        }
    }

}