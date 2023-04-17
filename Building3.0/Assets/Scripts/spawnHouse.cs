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
    
    [SerializeField]
    public bool refresh;

    private void Start()
    {
        refresh = true;
    }

    private void LaunchProjectile()
    {
        refresh = true;
    }
    private void Update()
    {
        if (refresh==true)
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
                building.GetComponent<SpawnBuilding>().createBuildings(0,0,Random.Range(2,5),  Random.Range(2,4),  Random.Range(3,6));

                refresh = false;
        }
    }
    

}