using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitPointFinder : MonoBehaviour {

    public List<Transform> ExitPoints = new List<Transform>();
	// Use this for initialization
	void Start () {
        foreach (Transform childTransform in transform)
        {
            if(childTransform.gameObject.tag == "ExitPoint")
            {
                ExitPoints.Add(childTransform);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
