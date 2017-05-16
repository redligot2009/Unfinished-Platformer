using UnityEngine;
using System.Collections;
//[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {
    [HideInInspector]
    public BoxCollider2D myCollider;
    public RaycastOrigins raycastOrigins;
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    public float skinWidth = 0.015f;
    public LayerMask collisionMask;
    public Bounds bounds;

    public virtual void Start()
    {
        if (GetComponent<BoxCollider2D>())
        {
            myCollider = GetComponent<BoxCollider2D>();
        }
        else
        {
            myCollider = transform.FindChild("collider").GetComponent<BoxCollider2D>();
        }
        CalculateRaySpacing();
    }

    public void UpdateRayCastOrigins()
    {
        bounds = myCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        bounds = myCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
