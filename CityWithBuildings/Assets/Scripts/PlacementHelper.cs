using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementHelper
{
   //road and builder helper
   //one static method to find neighbour roads

   //pass position for which we want to find neighbour
   // pass interface instead list dictonary to use both helpers and use different collection types 
   public static List<Direction> FindNeighbour(Vector3Int position, ICollection<Vector3Int> collection)
   {
       List<Direction> neighbourDirections = new List<Direction>();
       // road neighbours to left to right or down so use a prefab like 3 way if 4 neighbour 4 ways
       if (collection.Contains(position + Vector3Int.right))
       {
           neighbourDirections.Add(Direction.Right);
       }
       
       if (collection.Contains(position - Vector3Int.right))
       {
           neighbourDirections.Add(Direction.Left);
       }
       
       if (collection.Contains(position + new Vector3Int(0,0,1)))//forward
       {
           neighbourDirections.Add(Direction.Up);
       }
       
       if (collection.Contains(position - new Vector3Int(0,0,1)))
       {
           neighbourDirections.Add(Direction.Down);
       }
       
       
       return neighbourDirections;
   }


   public static Vector3Int GetOffsetFromDirection(Direction direction)
   {
       switch (direction)
       {
           case Direction.Up:
           {
               return new Vector3Int(0, 0, 1);
           }
           case Direction.Down:
           {
               return new Vector3Int(0, 0, -1);
           }
           case Direction.Left:
           {
               return Vector3Int.left;
           }
           case Direction.Right:
           {
               return Vector3Int.right;
           }
       }
       throw new System.Exception("No direction such as "+direction);
   }

   public static Direction GetReverseDirection(Direction direction)
   {
       switch (direction)
       {
           case Direction.Up:
           {
               return Direction.Down;
           }
           case Direction.Down:
           {
               return Direction.Up;
           }
           case Direction.Left:
           {
               return Direction.Right;
           }
           case Direction.Right:
           {
               return Direction.Left;
           }
       }
       throw new System.NotImplementedException();
   }
}
