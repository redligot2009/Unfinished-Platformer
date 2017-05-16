using UnityEngine;
using System.Collections;

public class ShootProjectile : MonoBehaviour {
    public GameObject bulletPrefab;
    public float timeBeforeNextShot = 0.075f;
    public float shotTimer;
    int facing;
    GameObject player;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("player");
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!GlobalVars.paused)
        {
            if (mousePos.x > player.transform.position.x)
            {
                facing = 1;
            }
            if (mousePos.x < player.transform.position.x)
            {
                facing = -1;
            }
            if (Input.GetMouseButton(0) && shotTimer <= 0)
            {
                GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.FindChild("Muzzle").transform.position, Quaternion.identity);
                if (mousePos.x > player.transform.position.x)
                {
                    bullet.GetComponent<BulletScript>().facing = 1;
                }
                else
                {
                    bullet.GetComponent<BulletScript>().facing = -1;
                }
                shotTimer = timeBeforeNextShot;
            }
            if (shotTimer > 0)
            {
                shotTimer -= Time.deltaTime;
            }
        }
	}
}
