using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureHelper : MonoBehaviour
{
    //prefab of the object to spawn
    public BuildingType procedurallyBuilding;

    // nature random instead of houses
    public GameObject[] naturePrefabs;
    public bool randomNaturePlacement = false;
    [Range(0, 1)] public float randomNaturePlacementTreshold = 0.3f;
    public Dictionary<Vector3Int, GameObject> naturesDictionary = new Dictionary<Vector3Int, GameObject>();

    // to save references for game object
    public Dictionary<Vector3Int, GameObject> structuresDictionary = new Dictionary<Vector3Int, GameObject>();

    public float animationTime = 0.01f;
    public IEnumerator PlaceStructuresAroundRoad(List<Vector3Int> roadPositions) // from road helper to structure helper
    {
      
        Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpacesAroundRoad(roadPositions);
        // check each free spot in the free estate spots dictionary
        List<Vector3Int> blockedPosition = new List<Vector3Int>();
        foreach (var freeSpot in freeEstateSpots)
        {
            // when a structure require larger space block space not to place anythign else
            if (blockedPosition.Contains(freeSpot.Key))
            {
                continue;
            }
            var rotation = Quaternion.identity;
            switch (freeSpot.Value)
            {
                case Direction.Up:
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                    break;
                }
                case Direction.Down:
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                    break;
                }
                case Direction.Right:
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                    break;
                }
            }
            while(true)
            {
                if (randomNaturePlacement)
                {
                    var random = UnityEngine.Random.value;
                    if (random < randomNaturePlacementTreshold)
                    {
                        var nature = SpawnPrefab(naturePrefabs[UnityEngine.Random.Range(0, naturePrefabs.Length)],
                            freeSpot.Key, rotation);
                        try
                        {
                            naturesDictionary.Add(freeSpot.Key,nature);
                        }catch (Exception e)
                        {
                            Reset();
                            transform.parent.gameObject.GetComponent<Visualiser>().EmergencyFailure();
                            yield break;
                        }
                        break;
                    }
                }
                //other cases
                if (procedurallyBuilding.sizeRequired > 1)
                {
                    // size bigger than 1
                    //if the size is 3 we block two cells in each direction (right and left)
                    var halfSize = Mathf.FloorToInt(procedurallyBuilding.sizeRequired / 2.0f);//1.something to int so 2
                    List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
                    // true if space to place building
                    if (VerifyIfBuildingFits(halfSize, freeEstateSpots, freeSpot, blockedPosition, ref tempPositionsBlocked))
                    {
                        blockedPosition.AddRange(tempPositionsBlocked);
                        var building = SpawnPrefab(procedurallyBuilding.GetPrefab(), freeSpot.Key, rotation);
                        // add building to building dictionary
                        try
                        {
                            structuresDictionary.Add(freeSpot.Key,building);
    
                        }catch (Exception e) {
                            Reset();
                            transform.parent.gameObject.GetComponent<Visualiser>().EmergencyFailure();
                            yield break;
                        }
                        foreach (var pos in tempPositionsBlocked)
                        {
                            // to access structure by checking if dictionary contains position we have clicked
                            try
                            {
                                structuresDictionary.Add(pos, building);

                            }catch (Exception e) {
                                Reset();
                                transform.parent.gameObject.GetComponent<Visualiser>().EmergencyFailure();
                                yield break;
                            }
                        }
                        break;
                    }
                }
                else
                {
                    var building = SpawnPrefab(procedurallyBuilding.GetPrefab(), freeSpot.Key, rotation);
                    // add building to building dictionary
                    try
                    {
                        structuresDictionary.Add(freeSpot.Key,building);
                    }catch (Exception e) {
                        Reset();
                        transform.parent.gameObject.GetComponent<Visualiser>().EmergencyFailure();
                        yield break;

                    }
                }
                break;
            }
            procedurallyBuilding.DestroyBase();
            yield return new WaitForSeconds(animationTime);
        }

        transform.parent.gameObject.GetComponent<Visualiser>().pauseRepeating = false;
       
    }

    private bool VerifyIfBuildingFits(
        int halfSize, 
        Dictionary<Vector3Int, Direction> freeEstateSpots, 
        KeyValuePair<Vector3Int, Direction> freeSpot, 
        List<Vector3Int> blockedPosition,
        ref List<Vector3Int> tempPositionsBlocked)
    {
        Vector3Int direction = Vector3Int.zero;
        if (freeSpot.Value == Direction.Down || freeSpot.Value == Direction.Up)
        {
            //check direction if road is upwards or downwards we now we can move to right or left to add position + direction or - directuon
            direction = Vector3Int.right;
        }
        else
        {
            direction = new Vector3Int(0, 0, 1);
        }

        for (int i = 1; i <= halfSize; i++)
        {
            // position of current free space
            // to the right for example
            var position1 = freeSpot.Key + direction * i;
            // to the left
            var position2 = freeSpot.Key - direction * i;
            // check if those are free positions if not free building doesn't fit
            if (!freeEstateSpots.ContainsKey(position1) || !freeEstateSpots.ContainsKey(position2) ||
                blockedPosition.Contains(position1) || blockedPosition.Contains(position2))
            {
                return false;
            }
            
            tempPositionsBlocked.Add(position1);
            tempPositionsBlocked.Add(position2);
        }
        return true;
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation)
    {
        var newStructure = Instantiate(prefab, position, rotation, transform);
        newStructure.AddComponent<FallTween>();
        return newStructure;
    }

    private Dictionary<Vector3Int, Direction> FindFreeSpacesAroundRoad(List<Vector3Int> roadPositions)// i collection so you can pass these values generic to everyone
    {
        // algo to find these spaces
        Dictionary<Vector3Int, Direction> freeSpaces = new Dictionary<Vector3Int, Direction>();
        foreach (var position in roadPositions)
        {
            var neighbourDirections = PlacementHelper.FindNeighbour(position, roadPositions);
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (neighbourDirections.Contains(direction) == false)
                {
                    var newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
                    if(freeSpaces.ContainsKey(newPosition)) continue;
                    //find what is the direction of the street to know which direction the rooad is to know which the building is facing
                    freeSpaces.Add(newPosition,PlacementHelper.GetReverseDirection(direction));
                }
            }
        }

        return freeSpaces;
    }

    public void Reset()
    {
        foreach (var item in structuresDictionary.Values)
        {
            Destroy(item);
        }
        structuresDictionary.Clear();
        foreach (var item in naturesDictionary.Values)
        {
            Destroy(item);
        }
        naturesDictionary.Clear();
        procedurallyBuilding.Reset(); 
        procedurallyBuilding.DestroyBase();

    }
}