using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BuildingType
{
    // size that the structure required to be placed
    public int sizeRequired;
    public int quantityAlreadyPlaced; // count of how many structure we already placed in the map;

    // this method returns us the prefab
    public GameObject GetPrefab()
    {
        quantityAlreadyPlaced++;
        GameObject.Find("Ground").GetComponent<spawnHouse>().Create(Random.Range(2,5),Random.Range(2,4),Random.Range(3,6));
        return GameObject.Find("Building");
    }

    public void DestroyBase()
    { 
        GameObject.Find("Ground").GetComponent<spawnHouse>().DestroyBase();
    }
    
    
    // to implement button to recreate town
    public void Reset()
    {
        quantityAlreadyPlaced = 0;
    }
}