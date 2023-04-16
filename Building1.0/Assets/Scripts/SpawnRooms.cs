using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnRooms : MonoBehaviour
{
    private int counterRooms = 0 ;
    public GameObject wall;
    public GameObject door;
    private SpawnBuilding spawnB;
    private bool noExit = true;
    private string[] notEmptyWall;

    public void createFloor(spawnHouse spawnH, float z, float copyX, float copyY, float copyZ)
    {

        spawnB = gameObject.GetComponentInParent<global::SpawnBuilding>();
        Vector3 position;
        counterRooms = 0;

        for (int i = 1; i <= (spawnH.largo * spawnH.lungo); i++)
        {
            if (copyZ == z+spawnH.largo)
            {
                copyX +=1;
                copyZ = z;
            }
            position = new Vector3(copyX, copyY, copyZ);
            createRoom(position,this.gameObject,spawnH);
            copyZ++;

        }
        
    }
    
    
    private bool checkExist(Vector3 newPosition, spawnHouse spawnH)
    {
        foreach (var coordinata in spawnH.coords)
        {
            if (coordinata.x == newPosition.x && coordinata.y == newPosition.y && coordinata.z == newPosition.z) return false;
        }

        return true;
    }
    
    private GameObject randomiser (spawnHouse spawnH)
    {
        if (spawnB.counterFloors == 1)
        {
            var num = Random.Range(0f, 1f);
            var num2 = ((float)spawnH.mySlider)/100;
            if (num>num2)
            {
                return wall;
            }
            else
            {
                return door;
            }
            
        }else
        {
            return wall;
        }
      
    }
    
    private void createRoom(Vector3 position,GameObject Floor,spawnHouse spawnH)
    { 
        notEmptyWall = new string[4];
    Vector3 newPosition;
     var Room = new GameObject("Room"+spawnH.counterBuildings+"."+spawnB.counterFloors+"."+(++counterRooms));
     Room.transform.parent = Floor.transform;

     Vector3 positionEast = new Vector3(position.x, (position.y), position.z);
     bool eastInstantiate = checkExist(positionEast, spawnH);
     GameObject myPrefabEast = null;
            if(eastInstantiate){
                myPrefabEast = randomiser(spawnH);
                if (myPrefabEast == door)
                {
                    noExit = false;
                    notEmptyWall[0]="door";
                }
                else
                {
                    notEmptyWall[0]="wall";
                }
            }
            
     Vector3 positionWest = new Vector3(position.x, (position.y), (position.z + 1f));
     bool westInstantiate = checkExist(positionWest, spawnH);
     GameObject myPrefabWest = null;
            if (westInstantiate)
            {
                myPrefabWest = randomiser(spawnH);
                if (myPrefabWest == door)
                {
                    notEmptyWall[1]="door";
                }
                else
                {
                    notEmptyWall[1]="wall";
                }            }

     Vector3 positionSouth = new Vector3((position.x - .5f), (position.y), (position.z + .5f));
     bool southInstantiate = checkExist(positionSouth, spawnH);
     GameObject myPrefabSouth = null;
            if (southInstantiate)
            {
                myPrefabSouth = randomiser(spawnH);
                if (myPrefabSouth == door)
                {
                    noExit = false;
                    notEmptyWall[2]="door";
                }
                else
                {
                    notEmptyWall[2]="wall";
                }
                
            }

     Vector3 positionNorth = new Vector3((position.x + .5f), (position.y), (position.z + .5f));
     bool northInstantiate = checkExist(positionNorth, spawnH);
     GameObject myPrefabNorth = null;
     if (northInstantiate)
     {
         myPrefabNorth = randomiser(spawnH);
         if (counterRooms == ((spawnH.lungo * spawnH.largo) - 1) && noExit && spawnB.counterFloors == 1)
         {
             myPrefabNorth = door;
         }
         if (myPrefabNorth == door)
         {
             notEmptyWall[3]="door";
         }
         else
         {
             notEmptyWall[3]="wall";
         }
     }

     if (!notEmptyWall.Contains("door"))
     {
        if (!string.IsNullOrEmpty(notEmptyWall[0]))
        {
            if (
                (counterRooms - 1) % spawnH.largo != 0
            )
            {
                myPrefabEast = door;
            }
            else
            {
                if (!string.IsNullOrEmpty(notEmptyWall[1]))
                {
                    if (
                        counterRooms % spawnH.largo != 0
                    )
                    {
                        myPrefabWest = door;
                    }
                }
                if (!string.IsNullOrEmpty(notEmptyWall[2]))
                {
                   
                        myPrefabSouth = door;
                    
                }
                if (!string.IsNullOrEmpty(notEmptyWall[3]))
                {
                   
                        myPrefabNorth = door;
                    
                }
            }
        }
        else if (!string.IsNullOrEmpty(notEmptyWall[1]))
        {
            if (
                counterRooms % spawnH.largo != 0
            )
            {
                myPrefabWest = door;
            }
            else
            {
                if (!string.IsNullOrEmpty(notEmptyWall[2]))
                {
                    if (
                        counterRooms > spawnH.largo
                    )
                    {
                        myPrefabSouth = door;
                    }
                }
                if (!string.IsNullOrEmpty(notEmptyWall[3]))
                {
                    {
                        myPrefabNorth = door;
                    }
                }
            }
        }
        else if (!string.IsNullOrEmpty(notEmptyWall[2]))
        {
            myPrefabSouth = door;
          
                if (!string.IsNullOrEmpty(notEmptyWall[3]))
                {
                    myPrefabNorth = door;
                }
            
        }
        
     }
/*
     if (notEmptyWall.Contains("door"))
     {
         int count = 0;
         if (!string.IsNullOrEmpty(notEmptyWall[0])) count++;
         if (!string.IsNullOrEmpty(notEmptyWall[1])) count++;
         if (!string.IsNullOrEmpty(notEmptyWall[2])) count++;
         if (!string.IsNullOrEmpty(notEmptyWall[3])) count++;

         if (count < 2)
         {
             if (!string.IsNullOrEmpty(notEmptyWall[0])) myPrefabEast = door;
             if (!string.IsNullOrEmpty(notEmptyWall[1])) myPrefabWest = door;
             if (!string.IsNullOrEmpty(notEmptyWall[2])) myPrefabSouth = door;
             if (!string.IsNullOrEmpty(notEmptyWall[3])) myPrefabNorth = door;
         }
     }
*/
     if(eastInstantiate){
         spawnH.coords.Add(positionEast);
         var num = Random.Range(0f, 1f);
         if (
             (counterRooms-1)%spawnH.largo!=0  &&
             (myPrefabEast==door && num < 0.5)
         ) {
                   
         }
         else
         {
             var eastWall = Instantiate(myPrefabEast, positionEast, Quaternion.identity);
             eastWall.name = "Wall East";
             eastWall.transform.parent = Room.transform;
         }
     } 
            
     if (westInstantiate)
     {
         spawnH.coords.Add(positionWest);
         var num = Random.Range(0f, 1f);
         if (
             counterRooms%spawnH.largo!=0  &&
             (myPrefabWest==door && num < 0.5)
         ) {
                   
         }
         else
         {
             var westWall = Instantiate(myPrefabWest, positionWest, Quaternion.identity);
             westWall.name = "Wall West";
             westWall.transform.parent = Room.transform;
         }
     }
                
            
     if (southInstantiate)
     {
         spawnH.coords.Add(positionSouth);
         var num = Random.Range(0f, 1f);
         if (
             counterRooms>spawnH.largo  &&
             (myPrefabSouth==door && num < 0.5)
         ) {
                   
         }
         else
         {
             var southWall = Instantiate(myPrefabSouth, positionSouth, Quaternion.Euler(new Vector3(0, 90, 0)));
             southWall.name = "Wall South";
             southWall.transform.parent = Room.transform;
         }
                
     }
            
     if (northInstantiate)
     {
         spawnH.coords.Add(positionNorth);
         var num = Random.Range(0f, 1f);
         if (
             counterRooms<(spawnH.largo*(spawnH.lungo-1))  &&
             ((myPrefabNorth!=door && num < 0.2)||(myPrefabNorth==door && num < 0.5))
         ) {
                   
         }
         else
         {
             var northWall = Instantiate(myPrefabNorth, positionNorth, Quaternion.Euler(new Vector3(0, 90, 0)));
             northWall.name = "Wall North";
             northWall.transform.parent = Room.transform;
         }
     }
    }
}