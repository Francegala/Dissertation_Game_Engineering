using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnRooms : MonoBehaviour
{
    private int counterRooms = 0 ;
    public GameObject wall;
    public GameObject[] doors;
    public GameObject door;    
    public GameObject[] windows;
    public GameObject windowPrefab;    
    public GameObject floor;
    private bool instantiateFloor = false;
    public Material Roof;

    private SpawnBuilding spawnB;
    private bool noExit = true;
    private string[] notEmptyWall;

    public void createFloor(float z, float copyX, float copyY, float copyZ)
    {

        /* decide a door for each floor to have a variety but still consistency */
        door = GetDoor(); 
        spawnB = gameObject.GetComponentInParent<global::SpawnBuilding>();
        Vector3 position;
        counterRooms = 0;

        for (int i = 1; i <= (spawnB.largo * spawnB.lungo); i++)
        {
            if (copyZ == z+spawnB.largo)
            {
                copyX +=1;
                copyZ = z;
            }
            position = new Vector3(copyX, copyY, copyZ);
            createRoom(position,this.gameObject);
            copyZ++;

        }
        
    }
    
    public GameObject GetDoor()
    {
        if (doors.Length > 1)
        {
            var random = UnityEngine.Random.Range(0, doors.Length);
            return doors[random];
        }
        return doors[0];
    }
    
    public GameObject GetWindow()
    {
        if (windows.Length > 1)
        {
            var random = UnityEngine.Random.Range(0, windows.Length);
            return windows[random];
        }
        return windows[0];
    }
    
    
    private bool checkExist(Vector3 newPosition)
    {
        foreach (var coordinata in spawnB.coords)
        {
            if (coordinata.x == newPosition.x && coordinata.y == newPosition.y && coordinata.z == newPosition.z) return false;
        }

        return true;
    }
    
    private GameObject randomiser ()
    {
        if (spawnB.counterFloors == 1)
        {
            float num = Random.Range(0f, 1f);
            float num2 = 0.6f;
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
    
    private void createRoom(Vector3 position,GameObject Floor)
    {
        windowPrefab = GetWindow();
        instantiateFloor = false;
        notEmptyWall = new string[4];
        Vector3 newPosition;
        var Room = new GameObject("Room"+spawnB.counterFloors+"."+(++counterRooms));
        Room.transform.parent = Floor.transform;

        Vector3 positionEast = new Vector3(position.x, (position.y), position.z);
        bool eastInstantiate = checkExist(positionEast);
        GameObject myPrefabEast = null;
        if(eastInstantiate){
            myPrefabEast = randomiser();
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
        bool westInstantiate = checkExist(positionWest);
        GameObject myPrefabWest = null;
        if (westInstantiate)
        {
            myPrefabWest = randomiser();
            if (myPrefabWest == door)
            {
                notEmptyWall[1]="door";
            }
            else
            {
                notEmptyWall[1]="wall";
            }
                
        }

        Vector3 positionSouth = new Vector3((position.x - .5f), (position.y), (position.z + .5f));
        bool southInstantiate = checkExist(positionSouth);
        GameObject myPrefabSouth = null;
        if (southInstantiate)
        {
            myPrefabSouth = randomiser();
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
        bool northInstantiate = checkExist(positionNorth);
        GameObject myPrefabNorth = null;
        if (northInstantiate)
        {
            myPrefabNorth = randomiser();
            if (counterRooms == ((spawnB.lungo * spawnB.largo) - 1) && noExit && spawnB.counterFloors == 1) 
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

        // at least an entrnace per room
        if (!notEmptyWall.Contains("door"))
        {
            // if it is a wall
            if (!string.IsNullOrEmpty(notEmptyWall[0]))
            {
                if (
                    (counterRooms - 1) % spawnB.largo != 0 // if it is not the external to the right 
                )
                {
                    myPrefabEast = door;
                }
                else
                {
                
                    if (!string.IsNullOrEmpty(notEmptyWall[2]))
                    {
                        myPrefabSouth = door;
                    }
                    if (!string.IsNullOrEmpty(notEmptyWall[1])) // not else if otherwise an isolated room
                    {
                        if (
                            counterRooms % spawnB.largo != 0
                        )
                        {
                            myPrefabWest = door;
                        }
                    }
                    else if (!string.IsNullOrEmpty(notEmptyWall[3]))
                    {
                        myPrefabNorth = door;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(notEmptyWall[1]))
            {
                if (
                    counterRooms % spawnB.largo != 0
                )
                {
                    myPrefabWest = door;
                }
                else
                {
                    if (!string.IsNullOrEmpty(notEmptyWall[2]))
                    {
                        if (
                            counterRooms > spawnB.largo
                        )
                        {
                            myPrefabSouth = door;
                        }
                    }
                    if (!string.IsNullOrEmpty(notEmptyWall[3])) // not else if otherwise last line closed
                    {
                        {
                            myPrefabNorth = door;
                        }
                    }
                }
            }
        }
        if(eastInstantiate){
            spawnB.coords.Add(positionEast);
            var num = Random.Range(0f, 1f);
            if (
                (counterRooms-1)%spawnB.largo!=0  &&
                ((myPrefabEast==door && num < 0.6)||(myPrefabEast!=door && num < 0.2))
            ) {
                   
            }
            else
            {
                Quaternion rotationEast = Quaternion.identity;

                if (instantiateFloor == false)
                {
                    var eastFloor = Instantiate(floor, positionEast+new Vector3(0,-0.5f,0.5f), Quaternion.Euler(new Vector3(90, 0, 0)));
                    instantiateFloor=true;
                    eastFloor.name = "Floor East";
                    eastFloor.transform.parent = Room.transform;
                }
             
                if (spawnB.counterFloors > 1 && spawnB.counterFloors<spawnB.numofFloors && (counterRooms) % spawnB.largo == 1){
                    var window = Random.Range(0f, 1f);
                    if(window < 0.3){
                        positionEast += new Vector3(0, -0.5f, 0);
                        myPrefabEast = windowPrefab;
                    }
                }
             
                // this to have doors on this side
                if (myPrefabEast != door && spawnB.counterFloors == 1 && (counterRooms) % spawnB.largo == 1){
                    var doorRandom = Random.Range(0f, 1f);
                    if(doorRandom < 0.3) myPrefabEast = door;
                }
                var eastWall = Instantiate(myPrefabEast, positionEast, rotationEast);
                if (myPrefabEast == door)
                {
                    eastWall.transform.position = new Vector3(eastWall.transform.position.x,
                        eastWall.transform.position.y - 0.3f, eastWall.transform.position.z);
                    eastWall.transform.Rotate(0,90,0);
                }
                eastWall.name = "Wall East";
                eastWall.transform.parent = Room.transform;

            }
        } 
            
        if (westInstantiate)
        {
            spawnB.coords.Add(positionWest);
            var num = Random.Range(0f, 1f);
            if (
                counterRooms%spawnB.largo!=0  &&
                ((myPrefabWest==door && num < 0.6)||(myPrefabWest!=door && num < 0.2))
            ) {
                   
            }
            else
            {
                Quaternion rotationWest = Quaternion.identity;
             
                if (instantiateFloor == false)
                {
                    var westFloor = Instantiate(floor, positionWest+new Vector3(0,-0.5f,-0.5f), Quaternion.Euler(new Vector3(90, 0, 0)));
                    instantiateFloor=true;
                    westFloor.name = "Floor West";
                    westFloor.transform.parent = Room.transform;
                }
             
                if (spawnB.counterFloors > 1 && spawnB.counterFloors<spawnB.numofFloors && (counterRooms) % spawnB.largo == 0){
                    var window = Random.Range(0f, 1f);
                    if(window < 0.3){
                        positionWest += new Vector3(0, -0.5f, 0);
                        myPrefabWest = windowPrefab;
                    }
                }
             
                // this to have doors on this side
                if (myPrefabWest != door && spawnB.counterFloors == 1 && (counterRooms) % spawnB.largo == 0){
                    var doorRandom = Random.Range(0f, 1f);
                    if(doorRandom < 0.3) myPrefabWest = door;
                }
                var westWall = Instantiate(myPrefabWest, positionWest, rotationWest);
                if (myPrefabWest == door)
                {
                    westWall.transform.position = new Vector3(westWall.transform.position.x,
                        westWall.transform.position.y - 0.3f, westWall.transform.position.z);
                    westWall.transform.Rotate(0,90,0);
                }
                westWall.name = "Wall West";
                westWall.transform.parent = Room.transform;
             
            }
        }
                
            
        if (southInstantiate)
        {
            spawnB.coords.Add(positionSouth);
            var num = Random.Range(0f, 1f);
            if (
                counterRooms>spawnB.largo  &&
                ((myPrefabSouth==door && num < 0.6)||(myPrefabSouth!=door && num < 0.2))
            ) {
                   
            }
            else
            {
                Quaternion rotationSouth = Quaternion.Euler(new Vector3(0, 90, 0));
             
                if (instantiateFloor == false)
                {
                    var southFloor = Instantiate(floor, positionSouth+new Vector3(+0.5f,-0.5f,0), Quaternion.Euler(new Vector3(90, 0, 0)));
                    instantiateFloor=true;
                    southFloor.name = "South Floor";
                    southFloor.transform.parent = Room.transform;
                }
             
                if (spawnB.counterFloors > 1 && spawnB.counterFloors<spawnB.numofFloors && counterRooms<= spawnB.largo){
                    var window = Random.Range(0f, 1f);
                    if(window < 0.5){
                        rotationSouth = Quaternion.Euler(new Vector3(0, 90, 0));
                        positionSouth += new Vector3(0, -0.5f, 0);
                        myPrefabSouth = windowPrefab;
                    }
                } 
                if (myPrefabSouth == door && counterRooms <= spawnB.largo)
                {
                    myPrefabSouth = wall;
                }

                // to have doors on this street
                if (myPrefabSouth != door && spawnB.counterFloors == 1 && counterRooms<= spawnB.largo){
                    var doorRandom = Random.Range(0f, 1f);
                    if(doorRandom < 0.3) myPrefabSouth = door;
                }
                var southWall = Instantiate(myPrefabSouth, positionSouth, rotationSouth);
                if (myPrefabSouth == door)
                {
                    southWall.transform.position = new Vector3(southWall.transform.position.x,
                        southWall.transform.position.y - 0.3f, southWall.transform.position.z);
                    southWall.transform.Rotate(0,90,0);
                }
                southWall.name = "Wall South";
                southWall.transform.parent = Room.transform;
            
            }
                
        }
     
        // if not yet created
        if (instantiateFloor == false)
        { 
            var northFloor = Instantiate(floor, positionNorth+new Vector3(-0.5f,-0.5f,0), Quaternion.Euler(new Vector3(90, 0, 0)));
            instantiateFloor=true;
            northFloor.name = "Floor North";
            northFloor.transform.parent = Room.transform;
        }
     
        if (spawnB.counterFloors==spawnB.numofFloors ){
            var soffitto = Instantiate(floor, positionNorth+new Vector3(-0.5f,0.5f,0), Quaternion.Euler(new Vector3(90, 0, 0)));
            soffitto.GetComponent<Renderer>().material = Roof;
            soffitto.name = "Soffitto";
            soffitto.transform.parent = Room.transform;
        } 
     
        if (northInstantiate)
        {
            spawnB.coords.Add(positionNorth);
            var num = Random.Range(0f, 1f);
            if (
                counterRooms<(spawnB.largo*(spawnB.lungo-1))  &&
                ((myPrefabNorth==door && num < 0.6)||(myPrefabNorth!=door && num < 0.2))
            )
            {
           
            }
            else
            {
                Quaternion rotationNorth = Quaternion.Euler(new Vector3(0, 90, 0));

                if (spawnB.counterFloors > 1 && spawnB.counterFloors<spawnB.numofFloors && counterRooms>=((spawnB.largo * spawnB.lungo)-spawnB.largo)){
                    var window = Random.Range(0f, 1f);
                    if(window < 0.5){
                        rotationNorth = Quaternion.Euler(new Vector3(0, 90, 0));
                        positionNorth += new Vector3(0, -0.5f, 0);
                        myPrefabNorth = windowPrefab;
                    }
                } 
                if (myPrefabNorth == door && counterRooms>=((spawnB.largo * spawnB.lungo)-spawnB.largo))
                {
                    myPrefabNorth = wall;
                }

                // to have doors on this street
                if (myPrefabNorth != door && spawnB.counterFloors == 1 && counterRooms>=((spawnB.largo * spawnB.lungo)-spawnB.largo))
                {
                    var doorRandom = Random.Range(0f, 1f);
                    if(doorRandom < 0.3) myPrefabNorth = door;
                }
                var northWall = Instantiate(myPrefabNorth, positionNorth, rotationNorth);
                if (myPrefabNorth == door)
                {
                    northWall.transform.position = new Vector3(northWall.transform.position.x,
                        northWall.transform.position.y - 0.3f, northWall.transform.position.z);
                    northWall.transform.Rotate(0,90,0);
                }
                northWall.name = "Wall North";
                northWall.transform.parent = Room.transform;
            
            }
        }
    }
}