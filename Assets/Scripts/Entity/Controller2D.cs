using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController {

	public float maxClimbAngle = 80f;
	public float maxDescendAngle = 70f;
	public float maxCeilingAngle1 = 45;
	public float maxCeilingAngle2 = 225;
	public CollisionInfo collisions;
	public LayerMask upPlatform;
	[HideInInspector]
	public Vector3 myVelocity;


	public override void Start()
	{
		base.Start();
		collisions.directionFacing = 1;
		collisions.originalPosition = transform.position;
	}

	public bool AABBCheck(Bounds r1, Bounds r2)
	{
		if ((r2.min.x > r1.max.x || r2.max.x < r1.min.x || r2.max.y < r1.min.y || r2.min.y > r1.max.y))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Move(Vector3 velocity, bool standingOnPlatform = false)
	{
		UpdateRayCastOrigins();
		collisions.Reset();
		collisions.velocityOld = velocity;
		collisions.positionOld = transform.position;

		if(velocity.x != 0)
		{
			collisions.directionFacing = (int)Mathf.Sign(velocity.x);
		}
		if(velocity.y < 0)
		{
			DescendSlope(ref velocity);
		}
		HorizontalCollisions(ref velocity);
		if (velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}

		if(standingOnPlatform)
		{
			collisions.below = collisions.canJump = true;
		}
		myVelocity = velocity;
		transform.Translate(velocity);
	}

	public List<Transform> CheckVertical(LayerMask myLayer, int directionY = 1, float collisionPadding = 0)
	{
		List<Transform> arr = new List<Transform>();
		float rayLength = myCollider.bounds.size.y/2f + skinWidth;
		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength + collisionPadding, myLayer);
			if(hit)
			{
				if(!arr.Contains(hit.transform))
				{
					arr.Add(hit.transform);
				}
			}
		}
		return arr;
	}

	public Transform CheckHorizontal(LayerMask myLayer, int directionX = 1)
	{
		float rayLength = skinWidth*4;
		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, myLayer);
			if(hit)
			{
				return hit.transform;
			}
		}
		return null;
	}

	

	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			RaycastHit2D upPlatformHit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, upPlatform);
			RaycastHit2D downHit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);
			RaycastHit2D jumpHit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength+0.1f, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
			
				/*if(upPlatformHit)
				{
					if(myCollider.bounds.min.y >= upPlatformHit.transform.GetComponent<BoxCollider2D>().bounds.max.y - 0.05f)
					{
						velocity.y = (upPlatformHit.distance - skinWidth) * directionY;
						rayLength = upPlatformHit.distance;
						collisions.below = true;
						collisions.above = false;
						collisions.canJump = true;
						/*if(upPlatformHit.transform.GetComponent<PathFollow>())
						{
							transform.Translate(new Vector3(upPlatformHit.transform.GetComponent<PathFollow>().newPos.x,upPlatformHit.transform.GetComponent<PathFollow>().newPos.y));
						}
					}
				}*/

			if(hit)
			{
                if (!hit.collider.gameObject.GetComponent<OneWay>())
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    float moveDistance = Mathf.Abs(velocity.x);
                    if (collisions.climbingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }
                    velocity.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;
                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                    collisions.canJump = true;
                }
                else
                {
                    if (directionY <= 0)
                    {
                        if (myCollider.bounds.min.y >= hit.transform.GetComponent<BoxCollider2D>().bounds.max.y - 0.05f)
                        {
                            velocity.y = (hit.distance - skinWidth) * -1f;
                            rayLength = hit.distance;
                            collisions.below = true;
                            collisions.canJump = true;
                            collisions.above = false;
                            collisions.ceilingAbove = false;
                        }
                    }
                }
			}

			/*if(downHit)
			{
				velocity.y = (downHit.distance - skinWidth) * -1f;
				rayLength = hit.distance;
				collisions.below = true;
				collisions.canJump = true;
			}*/

			if (jumpHit)
			{
                if (!jumpHit.collider.gameObject.GetComponent<OneWay>())
                {
                    collisions.canJump = true;
                }
			}
		}
		RaycastHit2D ceilingHit = Physics2D.Raycast(transform.position, new Vector2(0, 1f), 1f, collisionMask);
		if (ceilingHit)
		{
            if (!ceilingHit.collider.gameObject.GetComponent<OneWay>())
            {
                collisions.ceilingAbove = true;
            }
		}

		if (collisions.climbingSlope)
		{
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if(hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if(slopeAngle != collisions.slopeAngle)
				{
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = collisions.directionFacing;
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		float rayLength2 = Mathf.Abs(velocity.y) + skinWidth;

		if(Mathf.Abs(velocity.x) < skinWidth)
		{
			rayLength = skinWidth * 2;
		}

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (hit.distance == 0)
				{
					continue;
				}

				if (i == 0 && slopeAngle <= maxClimbAngle&&directionY == -1)
				{
					if (collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if(slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					if (velocity.y != 0)
					{
						ClimbSlope(ref velocity, slopeAngle);
					}
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (hit.collider.bounds.max.y - bounds.min.y > 0.5f||hit.collider.bounds.min.y - bounds.max.y > 0.5f)
				{
                    if (!hit.collider.gameObject.GetComponent<OneWay>())
                    {
                        if ((!collisions.climbingSlope && velocity.y <= 0 && velocity.x == 0 || slopeAngle > maxClimbAngle) && !(slopeAngle > 100 && slopeAngle < 180))
                        {
                            velocity.x = (hit.distance - skinWidth) * directionX;
                            rayLength = hit.distance;
                            //Debug.Log("yes");
                            if (slopeAngle >= maxClimbAngle)
                            {
                                collisions.left = directionX == -1;
                                collisions.right = directionX == 1;
                            }
                        }
                        else
                        {
                            ClimbSlope(ref velocity, slopeAngle);
                        }
                    }
				}
				if (collisions.climbingSlope)
				{
					//velocity.y = 0;
					//velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
				}
			}
		}
	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs(velocity.x);
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
		if (velocity.y <= climbVelocityY)
		{
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = collisions.canJump = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

		if(hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if(Mathf.Sign(hit.normal.x) == directionX)
				{
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x) + 0.25f)
					{
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;
						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	public void ResetPos()
	{
		transform.position = collisions.originalPosition;
	}

	public struct CollisionInfo
	{
		public bool above, below, ceilingAbove;
		public bool left, right;

		public bool canJump, climbingSlope, descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld, positionOld, originalPosition;
		public int directionFacing;

		public void Reset()
		{
			above = ceilingAbove = below = false;
			left = right = false;
			descendingSlope = climbingSlope = canJump = false;
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
