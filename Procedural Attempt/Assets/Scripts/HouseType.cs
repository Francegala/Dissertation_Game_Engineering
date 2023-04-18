using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingType
{
    [SerializeField]
    private GameObject[] prefabs;

    // sizze that the structure required to be placed
    public int sizeRequired;

    public int quantity;
    public int quantityAlreadyPlaced; // count of how many structure we already placed in the map;

    // this method returns us the prefab
    public GameObject GetPrefab()
    {
        quantityAlreadyPlaced++;
        if (prefabs.Length > 1)
        {
            var random = UnityEngine.Random.Range(0, prefabs.Length);
            return prefabs[random];
        }
        return prefabs[0];
    }

    // if we have already palced buildings and they are enough, it will skip to next house type otherwise place some more of this type
    public bool isBuildingAvailable()
    {
        return quantityAlreadyPlaced < quantity;
    }

    // to implement button to recreate town
    public void Reset()
    {
        quantityAlreadyPlaced = 0;
    }
}
