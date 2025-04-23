using System;
using System.Collections.Generic;
using UnityEngine;

public enum DisasterType { Destruction, Debuff, Spawn, Other }
public enum DisasterLevel { Low, Medium, High }

[CreateAssetMenu(fileName = "DisasterSO", menuName = "Data/DisasterData")]
public class DisasterSO : ScriptableObject
{
    public List<DisasterData> disasterData;
}

[Serializable]
public class DisasterData
{
    // Disaster name for vizualization
    [field: SerializeField] public string disasterName;
    [field: SerializeField] public DisasterType type;
    [field: SerializeField] public DisasterLevel level;
    // Вероятность или сила эффекта (для типа уничтожения можно трактовать как вероятность разрушения в диапазоне 0-1)
    [field: SerializeField][Range(0f, 1f)] public float magnitude;
    // Описание бедствия (текстовое объяснение эффекта)
    [field: SerializeField] public string description;
    [field: SerializeField] public List<EffectSO> effects;
}

/* Нужно добавить в эффектах
// Радиус действия бедствия (0 = только одна клетка, 1 = затрагивает соседей и т.д.)
int radius;

// Множитель дебаффа (для типа Debuff, например 0.5 = снижение дохода до 50% от нормального)
float debuffMultiplier;

// Длительность дебаффа в ходах
int debuffDuration;

// Префаб сущности, спавнимой бедствием (для типа Spawn)
GameObject spawnPrefab;
*/