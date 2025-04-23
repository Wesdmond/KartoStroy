using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public static class ExtensionMethods
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }

public class CardView : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    #region Old Realization
    [SerializeField] public string title;
    [SerializeField] public string description;
    [SerializeField] public string energyCost;
    [SerializeField] public Sprite imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private bool isHoverCard = false;
    private Collider2D col;
    private Vector3 startDragPosition;
    
    public Card Card { get; private set; }
    
    public void Setup(Card card)
    {
        Card = card;
        title = card.Title;
        description = card.Description;
        energyCost = card.Energy.ToString();
        imageSR = card.Image.sprite;
    }
    
    #endregion
    
    private Canvas canvas;
    private Image imageComponent;
    [SerializeField] private bool instantiateVisual = true;
    private VisualCardsHandler visualHandler;
    private Vector3 offset;
    public CardData data;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50;

    [Header("Selection")]
    public bool selected;
    public float selectionOffset = 50;
    private float pointerDownTime;
    private float pointerUpTime;

    [Header("Visual")]
    [SerializeField] private GameObject cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;

    [Header("States")]
    public bool isHovering;
    public bool isDragging;
    public bool isHiding;
    [HideInInspector] public bool wasDragged;

    [Header("Events")]
    [HideInInspector] public UnityEvent<CardView> PointerEnterEvent; // cardview
    [HideInInspector] public UnityEvent<CardView> PointerExitEvent;
    [HideInInspector] public UnityEvent<CardView, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<CardView> PointerDownEvent;
    [HideInInspector] public UnityEvent<CardView> BeginDragEvent;
    [HideInInspector] public UnityEvent<CardView> EndDragEvent;
    [HideInInspector] public UnityEvent<CardView, bool> SelectEvent;
    
    private int CardCount = 4;

    
    
    void Start()
    {
        col = GetComponent<Collider2D>();

        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();

        if (!instantiateVisual)
            return;
        visualHandler = FindObjectOfType<VisualCardsHandler>();
        // Setup
            // CardDataList cardDataList = CardDataList.Instance;
            //
            // System.Random rnd = new System.Random();
            // int random = Random.Range(0, CardCount);
            // if (cardDataList.getCard(random) == null) print("Card isnt null");
            // if (cardDataList.getCard(random).Title != null) print(cardDataList.getCard(random).Title);
            // title = cardDataList.getCard(random).Title;
            // description = cardDataList.getCard(random).Description;
            // energyCost = cardDataList.getCard(random).Energy.ToString();
            // imageSR = cardDataList.getCard(random).Sprite;
        //
        cardVisual = Instantiate(cardVisualPrefab, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
        cardVisual.title.SetText(title);
        cardVisual.description.SetText(description);
        cardVisual.energyCost.SetText(energyCost);
        cardVisual.imageSR.sprite = imageSR;
        cardVisual.Initialize(this);
    }
    
    void Update()
    {
        if (!isHiding)
        {
            ClampPosition();
        }
        if (isDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent.Invoke(this);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = mousePosition - (Vector2)transform.position;
        isDragging = true;
        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        imageComponent.raycastTarget = false;

        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    private bool CheckIsCanPlayCard()
    {
        float percentageOfScreenHeight = 0.40f;
        print(percentageOfScreenHeight * Screen.height);
        if (Input.mousePosition.y > percentageOfScreenHeight * Screen.height)
        {
            return true;
        }

        return false;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;
        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        imageComponent.raycastTarget = true;

        print(CheckIsCanPlayCard());
        if (CheckIsCanPlayCard())
        {
            PerformEffect();
        }
        
        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
            wasDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEvent.Invoke(this);
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;

        PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

        if (pointerUpTime - pointerDownTime > .2f)
            return;

        if (wasDragged)
            return;

        selected = !selected;
        SelectEvent.Invoke(this, selected);

        if (selected)
            transform.localPosition += cardVisual.transform.up * selectionOffset;
        else
            transform.localPosition = Vector3.zero;
    }

    public void Deselect()
    {
        if (selected)
        {
            selected = false;
            if (selected)
                transform.localPosition += cardVisual.transform.up * 50;
            else
                transform.localPosition = Vector3.zero;
        }
    }

    public void PerformEffect()
    {
        Debug.Log(gameObject.name + " is performing effects:    ");
        foreach (var effect in data.effects)
        {
            effect.Perform();
        }
    }

    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }
    public float NormalizedPosition()
    {
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    private void OnDestroy()
    {
        if(cardVisual != null)
            Destroy(cardVisual.gameObject);
    }
}