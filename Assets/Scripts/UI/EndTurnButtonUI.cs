using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonUI : MonoBehaviour
{
    Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        ActionSystem.SubscribeReaction<DisasterGA>(HideButtonReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<DisasterGA>(ShowButtonReaction, ReactionTiming.POST);
    }

    private void HideButtonReaction(DisasterGA disasterGA)
    {
        _button.interactable = false;
    }

    private void ShowButtonReaction(DisasterGA disasterGA)
    {
        _button.interactable = true;
    }

    private void StartBuildingTurn()
    {
        // Start building turn at the end of disaster turn
        BuildingGA buildingGA = new();
        if (ActionSystem.Instance != null)
        {
            ActionSystem.Instance.Perform(buildingGA);
        } else
        {
            Debug.LogError("ActionSystem not connected to scene");
        }
    }
    
    public void OnClick()
    {
        DisasterGA disasterGA = new();
        if (ActionSystem.Instance != null)
        {
            ActionSystem.Instance.Perform(disasterGA, StartBuildingTurn);
        } else
        {
            Debug.LogError("ActionSystem not connected to scene");
        }
    }
}   