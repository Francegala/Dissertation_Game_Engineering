using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{

 public void Info()
 {
  
 }

 public void closeApp()
 {
  Application.Quit();

 }


 void Update()
 {
  if (Input.GetKey("escape"))
  {
   Application.Quit();
  }
  
  // moving camera
  if (Input.GetKey("right"))GameObject.Find("Main Camera").transform.Translate(Vector3.right* Time.deltaTime);

  if (Input.GetKey("left")) GameObject.Find("Main Camera").transform.Translate(Vector3.left* Time.deltaTime);


    if (Input.GetKey("up"))   GameObject.Find("Main Camera").transform.Translate(Vector3.forward* Time.deltaTime);

     if (Input.GetKey("down"))   GameObject.Find("Main Camera").transform.Translate(-Vector3.forward* Time.deltaTime);

 }
    
}
