using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuilding : MonoBehaviour
{
		
    public GameObject myFloor;
    public int counterFloors = 0 ;
    public Material[] materialBuilding;
    
    public List<Vector3> coords;

    
    public int largo = 4;
    public int lungo = 4;
    public int numofFloors = 1;

    public void createBuildings(float x, float z,int lungoVar, int largoVar, int floorsVar)
    {
        coords = new List<Vector3>();
        lungo = lungoVar;
        largo = largoVar;
        numofFloors = floorsVar;
        
        counterFloors = 0;
        Material picked = GetMaterial();
        for (float f = 0; f < numofFloors; f++) { 
            float copyX = x; 
            float copyY = f; 
            float copyZ = z;
			
            var floor =Instantiate(myFloor);
            floor.name = "Floor"+(++counterFloors);
            floor.transform.parent = this.transform;

            SpawnRooms spawnRooms = floor.GetComponent<SpawnRooms>();
            spawnRooms.wall.GetComponent<Renderer>().material = picked;
            spawnRooms.floor.GetComponent<Renderer>().material = picked;
            spawnRooms.createFloor(z, copyX, copyY, copyZ);
        } 

    }
		
    public Material GetMaterial()
    {
        if (materialBuilding.Length > 1)
        {
            var random = UnityEngine.Random.Range(0, materialBuilding.Length);
            return materialBuilding[random];
        }
        return materialBuilding[0];
    }

}