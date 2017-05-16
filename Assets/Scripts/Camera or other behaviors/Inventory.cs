using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Inventory : MonoBehaviour {

    public List<BasicItem> InventoryList = new List<BasicItem>();
    public List<BasicItem> keyList;
    public int maxInventoryCapacity;
    public int currentSlot = 1;
    public Transform playerObject;
    public Color currentSlotColor;
    public Color notCurrentSlotColor;
    BasicItem mySlot;
    public bool opened = true;
    [HideInInspector]
    public Cutscene currentCutscene;
    [HideInInspector]
    public int firstEmptySlot = 0;
    // Use this for initialization
    void Start () {
        if (GameObject.Find("player"))
        {
            playerObject = GameObject.Find("player").GetComponent<Transform>();
        }
	}
    // Update is called once per frame
    void Update () {
        currentSlot = Mathf.Clamp(currentSlot, 1, maxInventoryCapacity);
        GameObject slotText;
        GameObject slotBackground;

        //current slot
        if(Input.GetKey(KeyCode.Alpha1))
        {
            currentSlot = 1;
        }
        if(Input.GetKey(KeyCode.Alpha2))
        {
            currentSlot = 2;
        }
        if(Input.GetKey(KeyCode.Alpha3))
        {
            currentSlot = 3;
        }
        if(Input.GetKey(KeyCode.Alpha4))
        {
            currentSlot = 4;
        }

        //slotText setting etc.
        if(keyList != null)
        {
            for (int i = 0; i < keyList.Count; i++)
            {
                if(keyList[i].GetComponentInChildren<Renderer>().enabled)
                {
                    keyList[i].GetComponentInChildren<Renderer>().enabled = false;
                }
            }
        }
        if (InventoryList != null)
        {
            for (int i = 0; i < maxInventoryCapacity; i++)
            {
                slotText = GameObject.Find("slotText" + (i + 1));
                if (InventoryList[i] != null)
                {
                    if (slotText != null)
                    {
                        slotText.GetComponent<Text>().text = InventoryList[i].myName;
                    }
                    if(InventoryList[i].GetComponentInChildren<Renderer>().enabled)
                    {
                        InventoryList[i].GetComponentInChildren<Renderer>().enabled = false;
                    }
                }
            }
            for (int i = 0; i < maxInventoryCapacity; i++)
            {
                if(InventoryList[i] == null)
                {
                	firstEmptySlot = i;
                	break;
                }
            }
        }
        for (int i = 0; i < maxInventoryCapacity; i++)
        {
            GameObject notCurrentSlotBackground = GameObject.Find("slotBackground" + (i + 1));
            slotBackground = GameObject.Find("slotBackground" + currentSlot);
            slotBackground.GetComponent<Image>().color = currentSlotColor;
            if(notCurrentSlotBackground != slotBackground)
            {
                notCurrentSlotBackground.GetComponent<Image>().color = notCurrentSlotColor;
            }
        }
        
        GameObject InventoryText = GameObject.Find("InventoryText");
        GameObject UI = GameObject.Find("UI");
        if (!GlobalVars.paused)
        {
            if (Input.GetKeyDown(KeyCode.Q) && InventoryList.ElementAtOrDefault(currentSlot - 1))
            {
                Drop(currentSlot);
            }
        }
        Transform InvObject = UI.transform.Find("Inventory");
        Transform KeysObject = UI.transform.Find("Keys");
        if(opened)
        {
            FadeIn(InvObject.GetComponent<CanvasGroup>());
        }
        else
        {
            FadeOut(InvObject.GetComponent<CanvasGroup>());
        }
        if(keyList.Count > 0)
        {
            FadeIn(KeysObject.GetComponent<CanvasGroup>());
        }
        else
        {
            FadeOut(KeysObject.GetComponent<CanvasGroup>());
        }
        //UI.transform.Find("Keys").GetComponent<CanvasGroup>().alpha = keyList.Count > 0?1f:0f;
        KeysObject.Find("AmountKeys").GetComponent<Text>().text = "Keys: " + keyList.Count;
        //reset
        if (GlobalVars.resetTimer > 0)
        {
            Reset();
        }
        //turn on-off
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!opened)
            {
                opened = true;
            }
            else if (opened)
            {
                opened = false;
            }
        }
    }

    public static void FadeIn(CanvasGroup myGroup, float delay = 0f)
    {
        if(myGroup.alpha < 0.99f)
        {
            myGroup.alpha = Mathf.Lerp(myGroup.alpha, 1, Time.deltaTime * 8f);
        }
        else
        {
            myGroup.alpha = 1f;
        }
    }

    public static void FadeOut(CanvasGroup myGroup, float delay = 0f)
    {
        if(delay <= 0)
        {
            if(myGroup.alpha > 0.05)
            {
                myGroup.alpha = Mathf.Lerp(myGroup.alpha, 0, Time.deltaTime * 8f);
            }
            else
            {
                myGroup.alpha = 0;
            }
        }
    }

    public void Reset()
    {
        for(int i = 0; i < maxInventoryCapacity; i ++)
        {
            GameObject.Find("slotText" + (i+1)).GetComponent<Text>().text = "Empty";
        }
        keyList.Clear();
    }

    public void Drop(int slot)
    {
        GameObject.Find("slotText" + (slot)).GetComponent<Text>().text = "Empty";
        GameObject.Find("slotBackground" + (slot)).GetComponent<Image>().color = currentSlotColor;
        mySlot = InventoryList[slot - 1];
        mySlot.GetComponent<Transform>().position = new Vector3(playerObject.position.x, playerObject.position.y, 0f);
        mySlot.GetComponentInChildren<Renderer>().enabled = true;
        mySlot.GetComponent<BasicItem>().velocity = playerObject.GetComponent<Player>().velocity;
        //InventoryList.Remove(mySlot);
        InventoryList[slot-1] = null;
    }

    public void DropAndDestroy(int slot)
    {
        GameObject.Find("slotText" + (slot)).GetComponent<Text>().text = "Empty";
        GameObject.Find("slotBackground" + (slot)).GetComponent<Image>().color = currentSlotColor;
        mySlot = InventoryList[slot - 1];
        InventoryList[slot-1] = null;
    }

    public void DropKey()
    {
    	if(keyList.Count > 0)
    	{
    		keyList.RemoveAt(0);
    	}
    }
}
