using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadHelper : MonoBehaviour
{
 //pass information that we finished creating the road using a delegate type action
 public Action finisedCoroutine;
 
 public GameObject roadStraight, roadCorner, road3way, road4way, roadEnd;

 // at some point issue replace prefabs
 // if place a road and place another road we need to change into corner or 3 ways
 // after create whole road have proper prefab
 public Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
 public HashSet<Vector3Int> fixRoadCandidates = new HashSet<Vector3Int>();

 // how many seconds to wait
 public float animationTime = 0.01f;
 public List<Vector3Int> GetRoadPosition()
 {
  return roadDictionary.Keys.ToList();
 }
 
 // instantiate prefab where we need road
 public IEnumerator PlaceStreetPositions(Vector3 startPosition, Vector3Int direction, int length)
 {
  var rotation = Quaternion.identity;
  //rotate prefab to merge direction of road
  if (direction.x == 0)
  {
   rotation = Quaternion.Euler(0, 90, 0);
  }

  for (int i = 0; i < length; i++)
  {
   var position = Vector3Int.RoundToInt(startPosition + direction * i);
   //do not place another prefab if already there
   if (roadDictionary.ContainsKey(position))
   {
    continue;
   }

   
   var road = Instantiate(roadStraight, position, rotation,transform);
   roadDictionary.Add(position,road);
  // if need to be changed starting position or ending position
   if (i == 0 || i == length - 1)
   {
    fixRoadCandidates.Add(position);
   }

   yield return new WaitForSeconds(animationTime);
  }
  // our delegate if it is equal send info to visualiser that we have finished placing a road and we can place another road.
finisedCoroutine?.Invoke();
 }

 // method to go for each position in candidates (hash set) doesn't take duplicates so only once 
 public void FixRoad()
 {
  foreach (var position in fixRoadCandidates)
  {
   // list of neighbours near this position roadDictionary.Keys is type icollection 
   List<Direction> neighbourDirections = PlacementHelper.FindNeighbour(position, roadDictionary.Keys);
   Quaternion rotation = Quaternion.identity;

   if (neighbourDirections.Count == 1)
   {
    Destroy(roadDictionary[position]); // remove actual road piece
    // check if contains
    if (neighbourDirections.Contains(Direction.Down))
    {
     rotation = Quaternion.Euler(0,90,0);
     //rotate end tale of 90 degree
    }else if (neighbourDirections.Contains(Direction.Left))
    {
     rotation = Quaternion.Euler(0,180,0);
    }else if (neighbourDirections.Contains(Direction.Up))
    {
     rotation = Quaternion.Euler(0,-90,0);
    }
    roadDictionary[position] = Instantiate(roadEnd, position, rotation, transform);

   }else if (neighbourDirections.Count == 2)
   {
    
    if (
     (neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Down))
     || (neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Left))
     )
    {
     continue;
    }
    
    Destroy(roadDictionary[position]);
    if (neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Right))
    {
     rotation = Quaternion.Euler(0,90,0);
     //rotate end tale of 90 degree
    }else if (neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Down))
    {
     rotation = Quaternion.Euler(0,180,0);
    }else if (neighbourDirections.Contains(Direction.Down) && neighbourDirections.Contains(Direction.Left))
    {
     rotation = Quaternion.Euler(0,-90,0);
    }
    roadDictionary[position] = Instantiate(roadCorner, position, rotation, transform);

    
   }else if (neighbourDirections.Count == 3)
   {
    // 3 way
    
    Destroy(roadDictionary[position]);
    if (neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Down)&& neighbourDirections.Contains(Direction.Left))
    {
     rotation = Quaternion.Euler(0,90,0);
     //rotate end tale of 90 degree
    }else if (neighbourDirections.Contains(Direction.Down) && neighbourDirections.Contains(Direction.Left)&& neighbourDirections.Contains(Direction.Up))
    {
     rotation = Quaternion.Euler(0,180,0);
    }else if (neighbourDirections.Contains(Direction.Left) && neighbourDirections.Contains(Direction.Up)&& neighbourDirections.Contains(Direction.Right))
    {
     rotation = Quaternion.Euler(0,-90,0);
    }
    roadDictionary[position] = Instantiate(road3way, position, rotation, transform);

    
   }else if (neighbourDirections.Count == 4)
   {
    // 4 way
    Destroy(roadDictionary[position]);
    roadDictionary[position] = Instantiate(road4way, position, rotation, transform);
   }

  }
 }

 //destroy all roads in map
 public void Reset()
 {
  foreach (var item in roadDictionary.Values)
  {
   Destroy(item);
  }
  roadDictionary.Clear();
  fixRoadCandidates = new HashSet<Vector3Int>();
 }
}
