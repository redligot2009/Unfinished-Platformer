using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
public class Door : MonoBehaviour {

    public bool locked = true;
    bool originalState;
    public List<string> possibleItems;
    Inventory inventory;
    public string sceneName;

    // Use this for initialization
    void Start () {
        originalState = locked;
        inventory = GameObject.Find("Main").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update () {
        if (GlobalVars.resetTimer > 0)
        {
            Reset();
        }
	}

    public void Reset()
    {
        locked = originalState;
    }

    public void useItemOnDoor(BasicItem basicItem)
    {
        for(int i = 0; i < possibleItems.Count; i ++)
        {
            if (basicItem.myName == possibleItems[i]&&basicItem!=null)
            {
                locked = false;
                inventory.DropKey();
                break;
            }
        }
    }

    public void OpenDoor()
    {
        GlobalVars.level = sceneName;
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName,LoadSceneMode.Single);
    }
}
