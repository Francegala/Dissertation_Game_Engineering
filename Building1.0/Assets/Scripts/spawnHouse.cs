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
    
    public int largo = 4;
    public int lungo = 4;
    public int numofFloors = 1;
    public int numofBuildings = 1;
    
    [SerializeField]
    public float[] startingXOfBuildings;
    [SerializeField]
    public bool refresh;
    public int counterBuildings = 0;

    private List<GameObject> buildingsList;
    public List<Vector3> coords;


    [Range(1, 100)] public int mySlider = 1;
 

    
    private void Start()
    {
        buildingsList = new List<GameObject>();
        coords = new List<Vector3>();
        startingXOfBuildings = new float[2*numofBuildings];
        //Start Block for AR
        // InvokeRepeating("LaunchProjectile", 2.0f, 3.0f);
        // End Block for AR

    }

    private void LaunchProjectile()
    {
        refresh = true;
    }
    private void Update()
    {
      
        if(numofBuildings!=(startingXOfBuildings.Length/2)){startingXOfBuildings = new float[2*numofBuildings];}
        if (refresh==true)
        {
            
            //Start Block for AR
           // lungo = (int)Random.Range(1, 5);
           // largo = (int)Random.Range(1, 5);
           // numofFloors = (int)Random.Range(1, 5);
            // End Block for AR
            
            foreach (var buildingCreated in buildingsList) {
                coords = new List<Vector3>();
                Destroy(buildingCreated);
                counterBuildings = 0;
            }
            
            for (int aIndex = 0; aIndex < (startingXOfBuildings.Length); aIndex=aIndex+2)
            {
                var building =Instantiate(myBuilding);
                    building.name = "Building"+(++counterBuildings);
                    //building.transform.parent = this.transform;
                    buildingsList.Add(building);

                SpawnBuilding spawnBuilding = building.GetComponent<SpawnBuilding>();
                spawnBuilding.createBuildings(startingXOfBuildings[aIndex],startingXOfBuildings[aIndex+1]);

            }
            refresh = false;

        }

    }

   
    


}