using UnityEngine;
using System.Collections;
using Pathfinding;

public class FollowAI : MonoBehaviour {

    public Transform target;

    public float updateRate = 0.2f;
    float updateTimer = 0.2f;

    Seeker mySeeker;

    public Path path;

    public float moveSpeed = 5f;


    //waypoint variables
    [HideInInspector]
    public bool pathEnded = false;

    public bool keepFindingPaths = true;

    public float nextWayPointDistance = 3f;
    public float maximumDistanceFromTarget = 20f;
    int currentWaypoint = 0;

    [HideInInspector]
    public Vector3 dir;

    void Start () {
        mySeeker = GetComponent<Seeker>();
        target = GameObject.Find("player").GetComponent<Transform>();
        updateTimer = updateRate;
        GetNewPath();
	}


    void GetNewPath()
    {
        mySeeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
	
	public void Update () {
	    if(target == null)
        {
            return;
        }
        
        if(path == null)
        {
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count && keepFindingPaths)
        {
            if(pathEnded)
            {
                return;
            }

            pathEnded = true;
            GetNewPath();
            return;
        }
        pathEnded = false;
        if (keepFindingPaths)
        {
            float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            float distFromTarget = (transform.position - target.position).sqrMagnitude;
            if (dist < nextWayPointDistance)
            {
                currentWaypoint++;
                return;
            }
            if (distFromTarget < Mathf.Pow(maximumDistanceFromTarget, 2))
            {
                dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                dir *= moveSpeed * Time.fixedDeltaTime;
                if (updateTimer > 0)
                {
                    updateTimer -= Time.fixedDeltaTime;
                }
            }
        }

        if(updateTimer <= 0)
        {
            updateTimer = updateRate;
        }

        if(updateTimer == updateRate && keepFindingPaths)
        {
            GetNewPath();
        }
    }
}
