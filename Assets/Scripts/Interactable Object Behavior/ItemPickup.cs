using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

    public string playerLayer;
    BoxCollider2D myCollider;
    BasicItem basicItem;

    Inventory inventory;
    // Use this for initialization
    void Start () {
        myCollider = GetComponent<BoxCollider2D>();
        inventory = GameObject.Find("Main").GetComponent<Inventory>();
        basicItem = GetComponentInParent<BasicItem>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay2D(Collider2D info)
    {
        //Debug.Log(myName);
        if (!GlobalVars.paused)
        {
            if (info.gameObject.layer == LayerMask.NameToLayer(playerLayer))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if(transform.parent.GetComponent<BasicItem>().myName != "Key")
                    {
                        if (!inventory.InventoryList.Contains(basicItem) && inventory.firstEmptySlot < inventory.maxInventoryCapacity)
                        {
                            //inventory.InventoryList.Add(basicItem);
                            inventory.InventoryList[inventory.firstEmptySlot] = basicItem;
                        }
                        //Debug.Log(basicItem.myName);
                    }
                    else
                    {
                         if (!inventory.keyList.Contains(basicItem))
                        {
                            inventory.keyList.Add(basicItem);
                        }
                    }
                }
            }
        }
    }
}
