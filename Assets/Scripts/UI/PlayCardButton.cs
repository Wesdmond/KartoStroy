using UnityEngine;
using UnityEngine.UI;

public class PlayCardButton : MonoBehaviour
{
    [Header("References")]
    [SerializeReference] private HorizontalCardHolder HandScript;
    
    private Button ButtonRef;
    
    void Start()
    {
        ButtonRef = gameObject.GetComponentInChildren<Button>();
        HandScript.newItemSelected.AddListener(SetButtonState);
        ButtonRef.interactable = false;
    }

    void SetButtonState(CardView cardView)
    {
        if (HandScript.GetSelectedCard() != null)
        {
            ButtonRef.interactable = true;
        }
        else
        {
            ButtonRef.interactable = false;
        }
    }

    public void PlayCard()
    {
        if (HandScript.GetSelectedCard() != null)
            HandScript.GetSelectedCard().PerformEffect();
        else
        {
            Debug.LogWarning(gameObject.name + "card is not selected for 'Play Button'");
        }
    }
    
}