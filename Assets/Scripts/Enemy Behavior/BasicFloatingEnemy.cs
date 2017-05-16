using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Controller2D))]
public class BasicFloatingEnemy : EnemyBase {
    
    float dist;

    public FollowAI followAI;
    public RoamAI roamAI;

    public enum EnemyState
    {
        Roam = 0,
        Follow = 1,
        Stay = 2,
    }

    public EnemyState myState = EnemyState.Stay;

    public override void Start()
    {
        base.Start();
        followAI = transform.GetComponent<FollowAI>();
        roamAI = transform.GetComponent<RoamAI>();
    }

    public override void resetEnemy()
    {
        base.resetEnemy();
        velocity = Vector2.zero;
        followAI.keepFindingPaths = true;
    }
	
    void UpdateStates()
    {
        if(dist < followAI.maximumDistanceFromTarget)
        {
            if (dist > followAI.nextWayPointDistance)
            {
                myState = EnemyState.Follow;
            }
            else
            {
                myState = EnemyState.Stay;
            }
        }
        else
        {
            myState = EnemyState.Roam;
        }
    }
	
	void Update () {
        //reset
        if (GlobalVars.resetTimer > 0)
        {
            resetEnemy();
        }
        if (!GlobalVars.paused)
        {
            //Move
            myController.Move(velocity * Time.deltaTime);
            //check if dead or not
            if (healthComponent.health <= 0)
            {
                dead = true;
            }

            //what to do if dead
            if (dead)
            {
                followAI.keepFindingPaths = false;
                velocity.y -= 1f;
            }

            //update enemy state
            if (followAI != null && roamAI != null && !dead)
            {
                UpdateStates();

                //distance from enemy to target
                dist = Vector3.Distance(transform.position, followAI.target.position);

                //if the distance between the two is less than maximum distance from target
                if (myState == EnemyState.Follow)
                {
                    followAI.keepFindingPaths = true;
                    if (followAI.target != GameObject.Find("player").GetComponent<Transform>())
                    {
                        followAI.target = GameObject.Find("player").GetComponent<Transform>();
                    }
                    velocity += followAI.dir;
                }

                if (myState == EnemyState.Roam)
                {
                    if (followAI.keepFindingPaths)
                    {
                        roamAI.GetPath();
                        followAI.keepFindingPaths = false;
                    }
                    if (roamAI.dist > 3)
                    {
                        velocity += roamAI.dir;
                    }
                }
            }

            //friction
            if (!dead)
            {
                if (Mathf.Abs(velocity.x) > 0)
                {
                    velocity.x = Mathf.Lerp(velocity.x, 0f, Time.deltaTime);
                }
                if (Mathf.Abs(velocity.y) > 0)
                {
                    velocity.y = Mathf.Lerp(velocity.y, 0f, Time.deltaTime);
                }
            }
            else
            {
                if (Mathf.Abs(velocity.x) > 0)
                {
                    velocity.x = Mathf.Lerp(velocity.x, 0f, Time.deltaTime * 2f);
                }
            }

            //bounce off walls
            if (myController.collisions.left)
            {
                velocity.x += 10f;
            }
            if (myController.collisions.right)
            {
                velocity.x -= 10f;
            }
            if (myController.collisions.above)
            {
                velocity.y -= 10;
            }
            if (myController.collisions.below)
            {
                if (!dead)
                {
                    velocity.y += 10;
                }
                else
                {
                    if (Mathf.Abs(velocity.y) < 12)
                    {
                        velocity.y = 0;
                    }
                    else if (Mathf.Abs(velocity.y) > 12)
                    {
                        velocity.y = -velocity.y * 0.75f;
                    }
                }
            }

            //clamp velocities
            velocity.x = Mathf.Clamp(velocity.x, -maxVelocity.x, maxVelocity.x);
            velocity.y = Mathf.Clamp(velocity.y, -maxVelocity.y, maxVelocity.y);

            //prevent screwing around on z axis
            velocity.z = 0;
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.red;
        if (followAI != null)
        {
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, 2), transform.forward, followAI.maximumDistanceFromTarget);
        }
#endif
    }
}
