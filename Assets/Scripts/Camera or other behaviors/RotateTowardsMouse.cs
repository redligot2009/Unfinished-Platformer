using UnityEngine;
using System.Collections;

public class RotateTowardsMouse : MonoBehaviour {
    //public Transform target;
    Vector3 targetPos;
    public float speed;
    float newAngle;
    Transform weaponGraphics;
    // Use this for initialization
    void Start () {
        weaponGraphics = GetComponentInChildren<Transform>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!GlobalVars.paused)
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (targetPos.x >= transform.position.x)
            {
                newAngle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x);
                transform.localScale = new Vector3(1f, 1, 1);
                transform.eulerAngles = new Vector3(0, 0, newAngle * Mathf.Rad2Deg);
            }
            if (targetPos.x < transform.position.x)
            {
                newAngle = Mathf.Atan2(transform.position.y - targetPos.y, transform.position.x - targetPos.x);
                transform.localScale = new Vector3(-1f, 1, 1);
                transform.eulerAngles = new Vector3(0, 0, (newAngle * Mathf.Rad2Deg));
            }
        }
    }
}
