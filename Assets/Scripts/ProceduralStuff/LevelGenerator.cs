using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {
    public List<GameObject> levelPrefabs = new List<GameObject>();
    public List<GameObject> roomsInScene = new List<GameObject>();
    public int maxNumberOfRooms = 5;
	// Use this for initialization
	void Start () {
        AddRoom(transform.position, levelPrefabs[5]);
	}

    public void AddRoom(Vector2 position, GameObject levelPrefab)
    {
        GameObject theObject = (GameObject)Instantiate(levelPrefab, position, Quaternion.identity);
        roomsInScene.Add(theObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
