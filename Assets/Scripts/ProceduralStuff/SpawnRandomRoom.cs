using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnRandomRoom : MonoBehaviour {
    GameObject LevelGeneratorGameObject;
    public List<Room> possibleRooms = new List<Room>();
    LevelGenerator levelGenerator;
    int RoomInt;
    public int maxNumberOfChances = 2;
	// Use this for initialization
	void Start () {
        LevelGeneratorGameObject = GameObject.Find("LevelGenerator");
        levelGenerator = LevelGeneratorGameObject.GetComponent<LevelGenerator>();
        RoomInt = Random.Range(0, maxNumberOfChances);
        if (levelGenerator.roomsInScene.Count < levelGenerator.maxNumberOfRooms)
        {
            /*if (RoomInt == 0 || RoomInt == 1)
            {
                levelGenerator.AddRoom(transform.parent.transform.position + new Vector3(levelGenerator.levelPrefabs[0].GetComponent<BoxCollider2D>().size.x, 0f), 0);
            }
            else if (RoomInt == 2)
            {
                levelGenerator.AddRoom(transform.parent.transform.position + new Vector3(levelGenerator.levelPrefabs[0].GetComponent<BoxCollider2D>().size.x, 0f), 6);
            }*/
            foreach(Room room in possibleRooms)
            {
                for (int i = 0; i < room.chances.Length; i++)
                {
                    if (RoomInt == room.chances[i])
                    {
                        if (room.horizontalPlacement)
                        {
                            levelGenerator.AddRoom(transform.parent.position + new Vector3(transform.parent.gameObject.GetComponent<BoxCollider2D>().size.x, 0f), room.prefab);
                        }
                        if (!room.horizontalPlacement)
                        {
                            levelGenerator.AddRoom(transform.parent.position + new Vector3(0f, room.prefab.GetComponent<BoxCollider2D>().size.y), room.prefab);
                        }
                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

[System.Serializable]
public class Room:System.Object
{
    public GameObject prefab;
    public int[] chances;
    public bool horizontalPlacement = true;
}