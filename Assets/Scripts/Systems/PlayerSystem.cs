using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : Singleton<PlayerSystem>
{
    [Header("Player Parameters")]
    [SerializeField] private int Energy = 3;
    [SerializeField] private int Money = 100;
    [SerializeField] private int MaxEnergyLimit = 10;
    [SerializeField] private float ChangingDelay = 1.0f;
    
    [Header("Links to objects")]
    [SerializeField] public TMP_Text MoneyText;
    [SerializeField] public TMP_Text EnergyText;

    [HideInInspector] public AudioSource mainSource;

    public void Start()
    {
        EnergyText.SetText(Energy.ToString());
        MoneyText.SetText(Money.ToString());
        mainSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        
    }

    public void ChangeEnergy(int amount, float delay = float.NaN)
    {
        
        if (float.IsNaN(delay)) delay = ChangingDelay;
        DOTween.To(() => Energy, x => Energy = x, amount, delay).OnUpdate(() => EnergyText.SetText(Energy.ToString()));
    }

    public void ChangeMoney(int amount, float delay = float.NaN)
    {
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
}