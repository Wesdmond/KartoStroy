using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : Singleton<PlayerSystem>
{
    [SerializeField] private int Energy = 3;
    [SerializeField] private int Money = 100;
    [SerializeField, Range(0.0f, 2.0f)] private float ChangingDelay = 1.0f;

    [SerializeField] public TMP_Text MoneyText;
    [SerializeField] public TMP_Text EnergyText;

    public void Start()
    {
        EnergyText.SetText(Energy.ToString());
        MoneyText.SetText(Money.ToString());
    }

    void Update()
    {
        Money++;
        MoneyText.SetText(Money.ToString());
    }

    public void ChangeEnergy(int amount, float delay = float.NaN)
    {
        if (float.IsNaN(delay)) delay = ChangingDelay;
        DOTween.To(() => Energy, x => Energy = x, amount, delay);
        EnergyText.SetText(Energy.ToString());
    }

    public void ChangeMoney(int amount, float delay = float.NaN)
    {
        if (float.IsNaN(delay)) delay = ChangingDelay;
        DOTween.To(() => Money, x => Money = x, amount, delay);
        MoneyText.SetText(Money.ToString()); 
    }

    public int GetEnergy()
    {
        return Energy;
    }

    public int GetMoney()
    {
        return Money;
    }
}