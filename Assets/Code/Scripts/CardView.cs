using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text energyCost;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private bool isHoverCard = false;
    private Collider2D col;
    private Vector3 startDragPosition;
    
    public Card Card { get; private set; }
    

    void Start()
    {
        col = GetComponent<Collider2D>();
    }
    
    public void Setup(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Descriptioon;
        energyCost.text = card.Energy.ToString();
        imageSR.sprite = card.Image;
    }

    void OnMouseEnter()
    {
        // wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x, -2, 0);
        CardViewHoverSystem.Instance.Show(Card, pos);
    }

    void OnMouseExit()
    {
        CardViewHoverSystem.Instance.Hide();
        // wrapper.SetActive(true);
    }

    private void OnMouseDown()
    {
        if (!isHoverCard) return;
        startDragPosition = transform.position;
        transform.position = GetMousePosition();
    }

    private void OnMouseDrag()
    {
        if (!isHoverCard) return;
        transform.position = GetMousePosition();
    }

    private void OnMouseUp()
    {
        if (!isHoverCard) return;
        col.enabled = false;
        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
        col.enabled = true;
        if (hitCollider != null && hitCollider.TryGetComponent(out CardView cardView))
        {
            
        }
        else
        {
            transform.position = startDragPosition;
        }
    }

    public Vector3 GetMousePosition()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }
}