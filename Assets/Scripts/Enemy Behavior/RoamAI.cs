using UnityEngine;
using System.Collections;
using Pathfinding;

public class RoamAI : MonoBehaviour {
    //public Transform target;

    public Vector3 randomPos;
    Vector3 currPos;
    /*Seeker mySeeker;
    public Path path;

    public bool pathEnded = false;

    public float nextWayPointDistance = 3f;
    public float maximumDistanceFromTarget = 20f;
    int currentWaypoint = 0;*/

    public float moveSpeed = 5f;

    public float randtimer;
    public float randInterval = 2f;
    public float randomRadius = 5f;

    //[HideInInspector]
    public Vector3 dir;
    public float dist;

    GridGraph gridGraph;

    // Use this for initialization
    void Start () {
        currPos = transform.position;
    }

	/*
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }*/

    public void GetPath()
    {
        if (currPos != transform.position)
        {
            currPos = transform.position;
        }
        randomPos.x = Random.Range(-randomRadius, randomRadius);
        randomPos.y = Random.Range(-randomRadius, randomRadius);
        randomPos = Vector3.ClampMagnitude(randomPos, randomRadius);
        Debug.DrawLine(transform.position, transform.position + randomPos);
        if (!Physics2D.Raycast((Vector2)transform.position, (Vector2)randomPos, randomRadius*1.5f, 1 << LayerMask.NameToLayer("ground"))&&(Mathf.Abs(randomPos.x) >= 1&&Mathf.Abs(randomPos.y) >= 1))
        {
            randtimer = randInterval + Random.Range(0,1);
            return;
        }
        else
        {
            GetPath();
        }
    }
	
	void Update () {
        if (randtimer <= 0)
        {
            GetPath();
        }

        if (randomPos != null)
        {
            dist = Vector3.Distance(transform.position, (currPos + randomPos));
        }

        if (randtimer > 0&&dist <= 3)
        {
            randtimer -= Time.deltaTime;
        }

        if (randomPos != null)
        {
            if (dist > 3)
            {
                dir = ((currPos + randomPos) - transform.position).normalized;
                dir *= moveSpeed * Time.deltaTime;
            }
        }
	}
}
