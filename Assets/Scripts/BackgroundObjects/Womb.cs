using UnityEngine;
using System.Collections;

public class Womb : MonoBehaviour {

    Animator myAnimator;
    Player playerCharController;
    ShootProjectile shootProjectile;
    RotateTowardsMouse rotateTowardsMouse;
    public GameObject weaponGameObject;
    public GameObject playerGameObject;
	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
        playerCharController = playerGameObject.GetComponent<Player>();
        shootProjectile = weaponGameObject.GetComponent<ShootProjectile>();
        rotateTowardsMouse = weaponGameObject.GetComponent<RotateTowardsMouse>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GlobalVars.resetTimer > 0)
        {
            myAnimator.SetBool("opened", false);
        }
        if (!GlobalVars.paused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myAnimator.SetBool("opened", true);
            }
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("openWomb"))
            {
                if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    playerCharController.gameObject.GetComponentInChildren<Animator>().SetBool("born", true);
                    if (playerCharController.gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        if(!Camera.main.transform.GetComponent<CameraControls>().cutSceneBoundaryTransform)
                        {
                            playerCharController.canMove = true;
                        }
                    }
                    shootProjectile.enabled = true;
                    rotateTowardsMouse.enabled = true;
                    weaponGameObject.GetComponentInChildren<Renderer>().enabled = true;
                }
            }
            else if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("idleWomb"))
            {
                playerCharController.gameObject.GetComponentInChildren<Animator>().SetBool("born", false);
                playerCharController.canMove = false;
                shootProjectile.enabled = false;
                rotateTowardsMouse.enabled = false;
                weaponGameObject.GetComponentInChildren<Transform>().rotation = Quaternion.identity;
                weaponGameObject.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
    }
}
