using UnityEngine;
using System.Collections;

public class BasicItem : MonoBehaviour {

    public string myName = "none";
    BoxCollider2D myCollider;
    BoxCollider2D itemCollider;
    Inventory inventory;
    Controller2D controller;
    public Vector3 velocity;
    public Vector3 maxVelocity;
    Renderer myRenderer;
    // Use this for initialization
    void Start () {
        myCollider = GetComponent<BoxCollider2D>();
        itemCollider = GetComponentInChildren<BoxCollider2D>();
        inventory = GameObject.Find("Main").GetComponent<Inventory>();
        controller = GetComponent<Controller2D>();
        myRenderer = gameObject.GetComponentInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!GlobalVars.paused)
        {
            controller.Move(velocity * Time.deltaTime);
            if (!inventory.InventoryList.Contains(this))
            {
                velocity.y -= 1f;
            }
            if (controller.collisions.below)
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
            if (controller.collisions.left)
            {
                velocity.x = 5f;
            }
            if (controller.collisions.right)
            {
                velocity.x = -5f;
            }
            if (velocity.x != 0)
            {
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 2.5f);
            }
            velocity.x = Mathf.Clamp(velocity.x, -maxVelocity.x, maxVelocity.x);
            velocity.y = Mathf.Clamp(velocity.y, -maxVelocity.y, Mathf.Infinity);
        }
        //reset
        if (GlobalVars.resetTimer > 0)
        {
            Reset();
        }
    }
    
    public void Reset()
    {
        controller.ResetPos();
        inventory.InventoryList.Remove(this);
        myRenderer.enabled = true;
    }
}
