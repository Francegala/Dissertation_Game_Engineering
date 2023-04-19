using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class changeSlider : MonoBehaviour
{
    public void AddX()
    {
        this.transform.position = new Vector3(
            this.transform.position.x+0.3f, 
            this.transform.position.y,
            this.transform.position.z);
    }
    public void SubX()
    {
        this.transform.position = new Vector3(
            this.transform.position.x-0.3f, 
            this.transform.position.y,
            this.transform.position.z);
    }  
    
    public void AddZ()
    {
        this.transform.position = new Vector3(
            this.transform.position.x, 
            this.transform.position.y,
            this.transform.position.z+0.3f);
    }
    public void SubZ()
    {
        this.transform.position = new Vector3(
            this.transform.position.x, 
            this.transform.position.y,
            this.transform.position.z-0.3f);
    }

    public void Rotate()
    {
        this.transform.Rotate(new Vector3(0,15,0));
    }
    
    public void GoTo1()
    {
        this.transform.rotation=(new Quaternion(0,0,0,0));
        this.transform.position = new Vector3(
            GameObject.Find("Visualiser 1").transform.position.x, 
            this.transform.position.y,
            GameObject.Find("Visualiser 1").transform.position.z
        );

    }
    
    public void GoTo2()
    {
        this.transform.rotation=(new Quaternion(0,0,0,0));
        this.transform.position = new Vector3(
            GameObject.Find("Visualiser 2").transform.position.x, 
            this.transform.position.y,
            GameObject.Find("Visualiser 2").transform.position.z
        );
    }
    
    public void GoTo3()
    {
        this.transform.rotation=(new Quaternion(0,0,0,0));
        this.transform.position = new Vector3(
            GameObject.Find("Visualiser 3").transform.position.x, 
            this.transform.position.y,
            GameObject.Find("Visualiser 3").transform.position.z
        );
    }
    
    public void GoTo4()
    {
        this.transform.rotation=(new Quaternion(0,0,0,0));
        this.transform.position = new Vector3(
            GameObject.Find("Visualiser 4").transform.position.x, 
            this.transform.position.y,
            GameObject.Find("Visualiser 4").transform.position.z
        );
    }
    
}