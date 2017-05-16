using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {
    public float dampTime = 0.15f;
    public float circleSize = 2f;
    public float cameraZoom = 1f;
    float originalZoom;
    public Transform target;
    public GameObject boundary;
    Transform boundaryTransform;
    [HideInInspector]
    public Transform cutSceneBoundaryTransform;
    Vector3 myPosition;

    public bool withinBounds = true;

    public Vector3 minCamera;
    public Vector3 maxCamera;

    void Start()
    {
        boundary = GameObject.Find("Boundary");
        boundaryTransform = boundary.GetComponent<Transform>();
        myPosition = GetComponent<Transform>().position;
        originalZoom = cameraZoom = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        float verticalExtents = Camera.main.orthographicSize;
        float horizontalExtents = verticalExtents*Screen.width/Screen.height;
        if(GlobalVars.resetTimer > 0)
        {
            ReturnToOriginalZoom();
        }
        if (!GlobalVars.paused)
        {
            if(cutSceneBoundaryTransform)
            {
                BoxCollider2D mySceneBounds = cutSceneBoundaryTransform.GetComponent<BoxCollider2D>();
                minCamera.x = mySceneBounds.bounds.min.x;
                maxCamera.x = mySceneBounds.bounds.max.x;
                minCamera.y = mySceneBounds.bounds.min.y;
                maxCamera.y = mySceneBounds.bounds.max.y;
            }
            if (GameObject.Find("Boundary")&&cutSceneBoundaryTransform == null)
            {
                minCamera.x = boundaryTransform.GetComponent<BoxCollider2D>().bounds.min.x;
                maxCamera.x = boundaryTransform.GetComponent<BoxCollider2D>().bounds.max.x;
                minCamera.y = boundaryTransform.GetComponent<BoxCollider2D>().bounds.min.y;
                maxCamera.y = boundaryTransform.GetComponent<BoxCollider2D>().bounds.max.y;
            }
            if (target)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
                Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
                Vector3 destination = myPosition + delta;
                Vector3 mouseToCameraDifference = myPosition - mousePos;
                if (target.GetComponent<Player>())
                {
                    Player myPlayer = target.GetComponent<Player>();
                    if (myPlayer.gunEquipped)
                    {
                        mouseToCameraDifference = Vector3.ClampMagnitude(mouseToCameraDifference, circleSize);
                    }
                    else
                    {
                        mouseToCameraDifference = Vector3.zero;
                    }
                }
                if(cutSceneBoundaryTransform == null || cutSceneBoundaryTransform && cutSceneBoundaryTransform.parent.GetComponent<Cutscene>()._played&&cutSceneBoundaryTransform.GetComponent<BoxCollider2D>().size.x > horizontalExtents * 2)
                {
                    myPosition.x = Mathf.Lerp(myPosition.x, destination.x - mouseToCameraDifference.x, Time.deltaTime * dampTime);
                }
                else if(cutSceneBoundaryTransform && cutSceneBoundaryTransform.parent.GetComponent<Cutscene>()._played&&cutSceneBoundaryTransform.GetComponent<BoxCollider2D>().size.x < horizontalExtents * 2)
                {
                    myPosition.x = minCamera.x + Camera.main.orthographicSize;
                }
                if(cutSceneBoundaryTransform == null || cutSceneBoundaryTransform && cutSceneBoundaryTransform.parent.GetComponent<Cutscene>()._played&&cutSceneBoundaryTransform.GetComponent<BoxCollider2D>().size.y > verticalExtents * 2)
                {
                    myPosition.y = Mathf.Lerp(myPosition.y, destination.y - mouseToCameraDifference.y, Time.deltaTime * dampTime);
                }
                else
                {
                    myPosition.y = minCamera.y + verticalExtents;
                }
            }
            if (withinBounds)
            {
                myPosition.x = Mathf.Clamp(myPosition.x, minCamera.x + horizontalExtents, maxCamera.x - horizontalExtents);
                myPosition.y = Mathf.Clamp(myPosition.y, minCamera.y + verticalExtents, maxCamera.y - verticalExtents);
                //myPosition = new Vector3(Mathf.Clamp(myPosition.x, minCamera.x + horizontalExtents, maxCamera.x - horizontalExtents), Mathf.Clamp(myPosition.y, minCamera.y + verticalExtents, maxCamera.y - verticalExtents), -1f);
            }
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraZoom, 2 * Time.deltaTime);
            transform.position = myPosition;
        }
    }

    public void Zoom(float zoomValue = 13.39f, float speed = 2.0f)
    {
        cameraZoom = Mathf.MoveTowards(cameraZoom, zoomValue, speed * Time.deltaTime);
    }

    public void ReturnToOriginalZoom(bool immediate = true, float speed = 2.0f)
    {
        cameraZoom = immediate?originalZoom:Mathf.MoveTowards(cameraZoom,originalZoom,speed*Time.deltaTime);
    }
}
