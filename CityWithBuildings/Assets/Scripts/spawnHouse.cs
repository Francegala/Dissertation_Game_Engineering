using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class spawnHouse : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject myBuilding;
    private List<GameObject> buildingsList  = new List<GameObject>();

    public void Create(int lungoVar, int largoVar, int floorsVar)
    {
        if (buildingsList.Count > 0)
        {
            foreach (var buildingCreated in buildingsList) {
                Destroy(buildingCreated);
            }
        }
        var building =Instantiate(myBuilding);
            building.name = "Building";
            buildingsList.Add(building);
            building.GetComponent<SpawnBuilding>().createBuildings(0,0, lungoVar,  largoVar,  floorsVar);
            building.transform.localScale /= 6;
        
    }

    public void DestroyBase()
    {
        if (buildingsList.Count > 0)
        {
            foreach (var buildingCreated in buildingsList) {
                Destroy(buildingCreated);
            }
        }   
    }
}