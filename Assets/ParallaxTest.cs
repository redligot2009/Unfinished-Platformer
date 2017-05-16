using UnityEngine;
using System.Collections;

public class ParallaxTest : MonoBehaviour {

	Vector3 myPos;
	Vector3 originalPos;
	public float parallaxSpeed = 0.05f;
    public GameObject original;
    GameObject clone1, clone2, clone3, clone4;
    CameraControls myCamera;

	// Use this for initialization
	void Start () {
		originalPos = transform.position;
        if(original != null)
        {
            /*clone1 = Instantiate(original,transform) as GameObject;
            clone1.transform.position = new Vector3(original.transform.position.x + original.transform.localScale.x, original.transform.position.y, original.transform.position.z);
            clone2 = Instantiate(original, transform) as GameObject;
            clone2.transform.position = new Vector3(original.transform.position.x, original.transform.position.y - original.transform.localScale.y, original.transform.position.z);
            clone3 = Instantiate(original, transform) as GameObject;
            clone3.transform.position = new Vector3(original.transform.position.x + original.transform.localScale.x, original.transform.position.y - original.transform.localScale.y, original.transform.position.z);*/

        }
        myCamera = Camera.main.GetComponent<CameraControls>();
        lastCameraPos = Camera.main.transform.position;
    }

    Vector3 lastCameraPos;
    // Update is called once per frame
    void LateUpdate () {
        float verticalExtents = Camera.main.orthographicSize;
        float horizontalExtents = verticalExtents * Screen.width / Screen.height;
        myPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 2f);
        transform.localScale = new Vector3(horizontalExtents * 2, verticalExtents * 2, 1f);
        transform.position = myPos;
        Vector3 shift = Camera.main.transform.position - lastCameraPos;
        lastCameraPos = Camera.main.transform.position;

        Vector2 offset = GetComponent<MeshRenderer>().material.mainTextureOffset;
        offset.x += shift.x * parallaxSpeed;
        offset.y += shift.y * parallaxSpeed;

        GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(transform.localScale.x, transform.localScale.y) / 20f);
        GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", offset);
    }
}
