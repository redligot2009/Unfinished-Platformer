using UnityEngine;
using System.Collections;

public class GoInvisibleOnStart : MonoBehaviour {

    public bool goInvisibleOnStart = true;
	// Use this for initialization
	void Start () {
	    if(goInvisibleOnStart)
        {
            GetComponent<Renderer>().enabled = false;
        }
	}
}
