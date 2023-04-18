using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualiser : MonoBehaviour
{

 //reference to the system generator
 public LSystemGenerator lsystem;

 //save positions the agent travelled to
 private List<Vector3> positions = new List<Vector3>();

 //list of all game objects to delete every time we crete new
 //private List<GameObject> objectsCreated = new List<GameObject>();

 public RoadHelper RoadHelper;
 public StructureHelper structureHelper;

 //lenght of the road that can change
 private int length = 15;
 public int startingLength = 15;
 public float angle = 85; //turn left or right

 //to stop invoke method generation when not completed
 public static bool pauseRepeating = false;
 // to stop clicking the button
 public bool pauseButton= false;

 private bool waitingForTheRoad = false;

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

 //pass the sequence and visualise it
 private IEnumerator VisualiseSequence(string sequence)
 {
  // last in first out
  Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
  var currentPosition = Vector3.zero; // use game object position 

  Vector3 direction = Vector3.forward; // z axes for starting firection
  Vector3
   tempPosition =
    Vector3.zero; // when drawing road set temp to current position then calculate end posiiton and draw line from temp to next position that will be current

  positions.Add(currentPosition);

  foreach (var letter in sequence)
  {
   if (waitingForTheRoad)
   {
    yield return new WaitForEndOfFrame();
   }
   SimpleVisualiser.EncodingLetters encoding = (SimpleVisualiser.EncodingLetters) letter;
   switch (encoding)
   {
    case SimpleVisualiser.EncodingLetters.save:
    {
     savePoints.Push(new AgentParameters
     {
      position = currentPosition,
      direction = direction,
      length = Length
     });
     break;
    }
    case SimpleVisualiser.EncodingLetters.load:
    {
     // there might be error in the sequence 
     if (savePoints.Count > 0)
     {
      // load position for agent
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
    case SimpleVisualiser.EncodingLetters.draw:
    {
     tempPosition = currentPosition; // save to draw a line
     currentPosition += direction * length; // where do we want to go, new point moved by lenght in direction
     // not draw line but road
    StartCoroutine(RoadHelper.PlaceStreetPositions(tempPosition, Vector3Int.RoundToInt(direction), length));
     waitingForTheRoad = true;
     yield return new WaitForEndOfFrame();
     Length -= 2; //next line shorter
     positions.Add(currentPosition);
     break;

    }
    case SimpleVisualiser.EncodingLetters.turnRight:
    {
     direction = Quaternion.AngleAxis(angle, Vector3.up) * direction; // new vector that returns angle turn to right
     break;
    }
    case SimpleVisualiser.EncodingLetters.turnLeft:
    {
     direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction; // new vector that returns angle rotate to left 
     break;
    }
   }
  }

  yield return new WaitForSeconds(0.1f);
  RoadHelper.FixRoad();
  yield return new WaitForSeconds(0.8f);
  StartCoroutine(structureHelper.PlaceStructuresAroundRoad(RoadHelper.GetRoadPosition()));

 }

 private void Start()
 {
  //informe visualiser that can continue with for loop  foreach (var letter in sequence) if (waitingForTheRoad) in its own coroutine
  RoadHelper.finisedCoroutine += () => waitingForTheRoad = false;

  InvokeRepeating("RandomGenerate", 1.0f, 60.0f);
  //RandomGenerate();
 }

 public void RandomGenerate()
 {
  //informe visualiser that can continue with for loop  foreach (var letter in sequence) if (waitingForTheRoad) in its own coroutine
  RoadHelper.finisedCoroutine += () => waitingForTheRoad = false;

  if (!pauseRepeating && !pauseButton)
  {
   StartCoroutine(MyCoroutine());
  }
 }

 IEnumerator MyCoroutine()
 {
  pauseRepeating = true;
  //This is a coroutine
  RoadHelper.Reset();
  structureHelper.Reset();
  positions.Clear();

  yield return new WaitForSeconds(0.1f); 
  
  length = startingLength;
  int choose = UnityEngine.Random.Range(0, 2);
  if (choose == 0)
  {
   angle *= (-1);
  }
  yield return new WaitForSeconds(0.1f);

  var sequence = lsystem.GenerateSentence();
  StartCoroutine(VisualiseSequence(sequence));


 }
 
 public void Pause()
 {
  if (pauseButton == false)
  {
   pauseButton = true;
   GameObject.Find("PauseButton").GetComponentInChildren<Text>().text = "In Pause";
  }
  else
  {
   pauseButton = false;
   GameObject.Find("PauseButton").GetComponentInChildren<Text>().text = "Click to Pause";

  }
 }

}