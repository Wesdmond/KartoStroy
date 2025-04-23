using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

public class HorizontalCardHolder : MonoBehaviour
{

    [SerializeField] private CardView draggingCard;
    [SerializeReference] private CardView selectedCard;
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
    [SerializeField] private Vector2 hideCardOffset = new Vector2(0f, 500f);
    [HideInInspector] public UnityEvent<CardView> newItemSelected = new UnityEvent<CardView>();

    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (isHandFillAtStart)
            FillHand();
    }

    private void BeginDrag(CardView card)
    {
        draggingCard = card;
    }


    void EndDrag(CardView card)
    {
        if (draggingCard == null)
            return;

        draggingCard.transform.DOLocalMove(draggingCard.selected ? new Vector3(0,draggingCard.selectionOffset,0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        draggingCard = null;

    }

    void CardPointerEnter(CardView card)
    {
        hoveredCard = card;
    }

    void CardPointerExit(CardView card)
    {
        hoveredCard = null;
    }

    void HideCards()
    {
        foreach (CardView card in cards)
        {
            card.isHiding = true;
        }
        rect.DOAnchorPos(rect.anchoredPosition - hideCardOffset, .2f).SetEase(Ease.InBack);
    }

    void ShowCards()
    {
        rect.DOAnchorPos(rect.anchoredPosition + hideCardOffset, .2f).SetEase(Ease.InBack).OnComplete(SetCardsFlag);
        void SetCardsFlag()
        {
            foreach (CardView card in cards)
            {
                card.isHiding = false;
            }
        }
    }

    public CardView GetSelectedCard()
    {
        return selectedCard;
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
            selectedCard = null;
            newItemSelected.Invoke(selectedCard);
            foreach (CardView card in cards)
            {
                card.Deselect();
            }
        }

        if (draggingCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {

            if (draggingCard.transform.position.x > cards[i].transform.position.x)
            {
                if (draggingCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (draggingCard.transform.position.x < cards[i].transform.position.x)
            {
                if (draggingCard.ParentIndex() > cards[i].ParentIndex())
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

        Transform focusedParent = draggingCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        draggingCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cards[index].cardVisual == null)
            return;

        bool swapIsRight = cards[index].ParentIndex() > draggingCard.ParentIndex();
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
        card.data = cardData;
        cards.Add(card);
    }

    private void OnNewCardSelected(CardView cardView, bool isSelected)
    {
        if (!isSelected)
        {
            if (selectedCard == cardView) selectedCard = null;
        }
        else
        {
            if (selectedCard != null)
            {
                selectedCard.Deselect();
            }
            selectedCard = cardView;
        }
        
        newItemSelected.Invoke(selectedCard);
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
            card.SelectEvent.AddListener(OnNewCardSelected);
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
            card.data = cardData;
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