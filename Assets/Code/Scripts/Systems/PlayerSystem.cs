using System;
using DG.Tweening;
using UnityEngine;

public class PlayerSystem : Singleton<PlayerSystem>
{
    [SerializeField] private int Energy = 3;
    [SerializeField] private int Money = 100;
    [SerializeField, Range(0.0f, 2.0f)] private float ChangingDelay = 1.0f;

    public void Start()
    {
        
    }

    public void ChangeEnergy(int amount, float delay = float.NaN)
    {
        if (float.IsNaN(delay)) delay = ChangingDelay;
        DOTween.To(() => Energy, x => Energy = x, amount, delay);
    }

    public void ChangeMoney(int amount, float delay = float.NaN)
    {
        if (float.IsNaN(delay)) delay = ChangingDelay;
        DOTween.To(() => Money, x => Money = x, amount, delay);
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