using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[ExecuteInEditMode]
[RequireComponent(typeof(Controller2D))]
public class PathFollow : MonoBehaviour {

    public List<GameObject> path;
    public float speed = 5.0f;
    public float reachDistance = 1.0f;
    public int currentPoint = 0;
    [HideInInspector]
    public Vector3 newPos;
    Vector3 dir;
    Controller2D myController;
    public LayerMask passengerLayers;
    OneWay myOneWay;
	// Use this for initialization
	void Start () {
        myController = GetComponent<Controller2D>();
        myOneWay = GetComponent<OneWay>();
	}

	// Update is called once per frame
	void Update () {
        //path = GameObject.FindGameObjectsWithTag("ExitPoint");
        foreach(Transform child in transform.parent)
        {
            if(path.Contains(child.gameObject))
            {
                path.Remove(child.gameObject);
            }
            if(child != transform)
            {
                if(!path.Contains(child.gameObject))
                {
                    path.Add(child.gameObject);
                }
            }
        }
        path.RemoveAll(GameObject => GameObject == null);
        path.Sort(delegate(GameObject g1,GameObject g2){
            if (g1 != null && g2 != null)
            {
                return g1.name.CompareTo(g2.name);
            }
            else
            {
                return 1;
            }
            });
        //Vector3 dir = path[currentPoint].position - transform.position;
        
        if(Application.isPlaying)
        {
            float dist = Vector3.Distance(path[currentPoint].transform.position, transform.position);
            //newPos = Vector3.MoveTowards(transform.position, path[currentPoint].transform.position, Time.deltaTime * speed);
            //transform.position = newPos;
            dir = path[currentPoint].transform.position - transform.position;
            newPos = dir.normalized * speed * Time.deltaTime;
            if(newPos.magnitude > dir.magnitude)
            {
                newPos = dir;
            }
            MovePassengers();
            myController.Move(newPos);
            if(dist <= reachDistance)
            {
                if (currentPoint < path.Count)
                {
                    currentPoint++;
                }
            }
    
            if(currentPoint >= path.Count)
            {
                currentPoint = 0;
            }
        }
	}


    List<Controller2D> myEntities = new List<Controller2D>();

    void MovePassengers()
    {
        List<Transform> entityAbove = myController.CheckVertical(passengerLayers, 1,0.025f);
        //Transform entitySide = myController.CheckHorizontal(passengerLayers, (int)Mathf.Sign(newPos.x));
        if(entityAbove != null)
        {
            foreach(Transform trans in entityAbove)
            {
                if(trans.GetComponent<Controller2D>()&&!myEntities.Contains(trans.GetComponent<Controller2D>()))
                {
                    myEntities.Add(trans.GetComponent<Controller2D>());
                    //passengerList.Add(entityAbove, entityAbove.GetComponent<Controller2D>());
                }
            }
        }
        /*if(entitySide)
        {
            if(entitySide.GetComponent<Controller2D>()&&!myEntities.Contains(entitySide.GetComponent<Controller2D>()))
            {
                //myEntities.Add(entitySide.GetComponent<Controller2D>());
            }
        }*/
        
        for(int i = 0; i < myEntities.Count; i ++)
        {
            if(myEntities[i].CheckVertical(1<<gameObject.layer, -1) == null)
            {
                myEntities.Remove(myEntities[i]);
                Debug.Log("removed 1");
                break;
            }
        }
        for(int i = 0; i < myEntities.Count; i ++)
        {
            if(myEntities[i].CheckVertical(1<<gameObject.layer, -1).Contains(transform) && myEntities[i].raycastOrigins.bottomLeft.y >= GetComponent<BoxCollider2D>().bounds.max.y - 0.05f)
            {
                myEntities[i].transform.Translate(newPos);
            }
        }
    }  

    void OnDrawGizmos()
    {
        if(path == null)
        {
            return;
        }
        for (int i = 0; i < path.Count; i++)
        {
            if (path[i] != null)
            {
                Gizmos.DrawCube(path[i].transform.position, Vector3.one);
            }
        }
    }
}
