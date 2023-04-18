using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVisualiser : MonoBehaviour
{
 
 //reference to the system generator
 public LSystemGenerator lsystem;
 
 //save positions the agent travelled to
 private List<Vector3> positions = new List<Vector3>();
 
 //list of all game objects to delete every time we crete new
 private List<GameObject> objectsCreated = new List<GameObject>();

 //place prefab where agent has visited
 public GameObject prefab;
 // line renderer clas from unity to render line to visualise path using lines
 public Material lineMaterial;

 private int length = 15;
 public int changeLength = 15;
 public float angle = 85; //turn left or right

 public int Length
 {
  get
  {
   if (length > 0)
   {
    return length;
   }
   else
   {
    return 1;
   }
  }
  set => length = value;
 }
 
//map the sequence tp concrete commands to the agent
 public enum EncodingLetters
 {
  unknown = '1',
  save = '[',
  load = ']',
  draw = 'F',
  turnRight = '+',
  turnLeft = '-'
 }
 
 //pass the sequence and visualise it
 private void VisualiseSequence(string sequence)
 {
  // last in first out
  Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
  var currentPosition = Vector3.zero; // use game object position 
  
  Vector3 direction = Vector3.forward; // z axes for starting firection
  Vector3 tempPosition = Vector3.zero; // when drawing road set temp to current position then calculate end posiiton and draw line from temp to next position that will be current
  
  positions.Add(currentPosition);

  foreach (var letter in sequence)
  {
   EncodingLetters encoding = (EncodingLetters)letter;
   switch (encoding)
   {
    case EncodingLetters.save:
    {
     savePoints.Push(new AgentParameters
     {
      position = currentPosition,
      direction = direction,
      length = Length
     });
     break;
    }
    case EncodingLetters.load:
    {
     // there might be error in the sequence 
     if (savePoints.Count > 0)
     { // load position for agent
      var agentParameter = savePoints.Pop();
      currentPosition = agentParameter.position;
      direction = agentParameter.direction;
      Length = agentParameter.length;
     }
     else
     {
      throw new System.Exception("Don't have saved point in our stack");
     }
     break;
    }
    case EncodingLetters.draw:{
     tempPosition = currentPosition; // save to draw a line
     currentPosition += direction * length; // where do we want to go, new point moved by lenght in direction
     DrawLine(tempPosition, currentPosition, Color.red);
     Length -= 2;//next line shorter
     positions.Add(currentPosition);
     break;
     
    }
    case EncodingLetters.turnRight:
    {
     direction = Quaternion.AngleAxis(angle,Vector3.up)*direction;// new vector that returns angle turn to right
     break;
    }
    case EncodingLetters.turnLeft:
    {
     direction = Quaternion.AngleAxis(-angle,Vector3.up)*direction;// new vector that returns angle rotate to left 
     break;
    }
   }
  }
  
  //spawn points where agent hs visited
  foreach (var position in positions)
  {
   objectsCreated.Add(Instantiate(prefab, position, Quaternion.identity));
  }
  
 }

 private void DrawLine(Vector3 start, Vector3 end, Color color)
 {
  GameObject line = new GameObject("line");
  objectsCreated.Add(line);
  line.transform.position = start;
  var lineRenderer = line.AddComponent<LineRenderer>(); //render a line between two points
  lineRenderer.material = lineMaterial;
  lineRenderer.startColor = color;
  lineRenderer.endColor = color;
  lineRenderer.startWidth = 0.1f;
  lineRenderer.endWidth = 0.1f;
  // line drawn between start point 0 and point number 1 that is end point
  lineRenderer.SetPosition(0,start);
  lineRenderer.SetPosition(1,end);

 }

 private void Start()
 {
  InvokeRepeating("RandomGenerate", 1.0f, 10.0f);
 }
 
 void RandomGenerate()
 {
  if (objectsCreated.Count>0)
  {
   length = changeLength;
   int choose = UnityEngine.Random.Range(0, 2);
   if (choose == 0)
   {
    angle *= (-1);
   }
   foreach (var obj in objectsCreated)
   {
    Destroy(obj);
   }
  }
  positions = new List<Vector3>();
  var seqeunce = lsystem.GenerateSentence();
  VisualiseSequence(seqeunce);
 }
}
