﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController {

    public LayerMask passengerMask;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public bool returnToOriginalPoint = false;
    public float waitTime;
    public float easeAmount = 0;
    [Range(0,2)]
    int fromWayPointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

	public override void Start ()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

	void Update ()
    {
        UpdateRayCastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    Vector3 CalculatePlatformMovement()
    {
        if(Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }
        fromWayPointIndex %= globalWaypoints.Length;
        int toWayPointIndex = (fromWayPointIndex + 1)%globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWayPointIndex], globalWaypoints[toWayPointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWayPoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWayPointIndex], globalWaypoints[toWayPointIndex], easedPercentBetweenWayPoints);

        if(percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWayPointIndex++;
            if (!returnToOriginalPoint)
            {
                if (fromWayPointIndex >= globalWaypoints.Length-1)
                {
                    fromWayPointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach(PassengerMovement passenger in passengerMovement)
        {
            if(!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }
            if(passenger.moveBeforePlatform == beforeMovePlatform)
            {
                if (!passengerDictionary[passenger.transform].collisions.above)
                {
                    passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
                }
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (hit&&hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1)?velocity.x:0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                        //hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }

        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                        //hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }

        //move down
        if(directionY == -1||velocity.y == 0&&velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit && hit.distance != 0&&!Physics2D.Raycast(hit.transform.position,Vector2.up,hit.collider.bounds.size.y/2, gameObject.layer))
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                        //hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }

    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                //Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                //Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                Gizmos.DrawCube(globalWaypointPos - Vector3.up * 0.5f, new Vector3(1.0f, 1.0f, 1.0f));
            }
        }
    }
}
