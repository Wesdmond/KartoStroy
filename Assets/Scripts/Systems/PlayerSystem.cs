using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : Singleton<PlayerSystem>
{
    [Header("Player Parameters")]
    [SerializeField] private int Energy = 10;
    [SerializeField] private int Money = 100;
    [SerializeField] private int MaxEnergyLimit = 10;
    [SerializeField] private float ChangingDelay = 1.0f;
    
    [Header("Links to objects")]
    [SerializeField] public TMP_Text MoneyText;
    [SerializeField] public TMP_Text EnergyText;

    [HideInInspector] public AudioSource mainSource;
    [SerializeField]  public HorizontalCardHolder horizontalCardHolder;

    public void Start()
    {
        EnergyText.SetText(Energy.ToString());
        MoneyText.SetText(Money.ToString());
        mainSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        
    }

    public void TakeNewCards()
    {
        horizontalCardHolder.FillHand();
    }

    public void HideCards()
    {
        horizontalCardHolder.HideCards();
    }

    public IEnumerator ShowCards()
    {
        yield return horizontalCardHolder.ShowCards();
    }
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<PlayerCardGA>(PerformCard);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<PlayerCardGA>();
    }

    public void ChangeEnergy(int amount, float delay = float.NaN)
    {
        Energy += amount;
        EnergyText.SetText(Energy.ToString());
        return;
        if (float.IsNaN(delay)) delay = ChangingDelay;
        DOTween.To(() => Energy, x => Energy = x, amount, delay).OnUpdate(() => EnergyText.SetText(Energy.ToString()));
    }

    public void ChangeMoney(int amount, float delay = float.NaN)
    {
        Money += amount;
        MoneyText.SetText(Money.ToString());
        return;
        if (float.IsNaN(delay)) delay = ChangingDelay;
        DOTween.To(() => Money, x => Money = x, amount, delay).OnUpdate(() => MoneyText.SetText(Money.ToString()));;
    }

    public int GetEnergy()
    {
        return Energy;
    }

    public int GetMoney()
    {
        return Money;
    }

    public void ResetEnergy()
    {
        Energy = MaxEnergyLimit;
    }
    
    private IEnumerator PerformCard(PlayerCardGA cardAction)
    {
        List<EffectSO> effects = cardAction.cardData.effects;
        if (effects != null)
        {
            foreach (var effect in effects)
            {
                Debug.Log($"Performing card effect: {effect.name}");
                yield return effect.Perform();
            }
        }
        else
        {
            Debug.LogError("PlayerCardPerformer: CardEffect is null");
        }
    }
}

public class PlayerCardGA : GameAction
{
    public CardData cardData { get; private set; }

    public PlayerCardGA(CardData cardData)
    {
        this.cardData = cardData;
    }
}