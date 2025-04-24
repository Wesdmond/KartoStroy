using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Building", fileName = "BuildingData")]
public class BuildingSO : ScriptableObject
{
    public List<BuildingData> buildingsData;
}

[Serializable]
public class BuildingData
{
    // Building name for visualization
    [field: SerializeField] public string displayName;
    [field: SerializeField] public string description;
    // [field: SerializeField] public int playCost; // neeed to do this in cards
    // [field: SerializeField] public int butingCost;
    // Building Icon (for windows or something like that)
    [field: SerializeField] public Sprite icon;
    // Prefab of building for vizualization
    [field: SerializeField] public GameObject prefab;
}