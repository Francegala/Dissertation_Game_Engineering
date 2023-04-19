using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProceduralCity/Rule")]
public class Rule : ScriptableObject
{
    public string letter;
    [SerializeField]
    private string[] results = null;
    // another serializable field
    [SerializeField] private bool randomResult = false; // if false return 0 index
    // instead of accessing the array directly we implemented get result method 
    public string GetResult()
    {
        if (randomResult)
        {
            int randomIndex = UnityEngine.Random.Range(0, results.Length);
            //Debug.Log(results[randomIndex]);
            return results[randomIndex];
        }
        //Debug.Log(results[0]);
        return results[0];
    }
    
}
