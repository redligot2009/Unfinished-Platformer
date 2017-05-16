using UnityEngine;
using System.Collections;


public enum TriggerType
{
	NONE = 0,
	ONCOLLISION = 1,
	ONPRESSED = 2
};

public class CutsceneTrigger : MonoBehaviour {

	BoxCollider2D myCollider;
	BoxCollider2D triggerBounds;
	Cutscene myCutscene;
	public TriggerType myTrigger;
	public KeyCode triggerKey;
	Transform player;
	bool finished;
	public bool replayable;

	// Use this for initialization
	void Start () {
		myCollider = transform.Find("sceneBounds").GetComponent<BoxCollider2D>();
		triggerBounds = transform.Find("triggerBounds").GetComponent<BoxCollider2D>();
		myCutscene = transform.GetComponent<Cutscene>();
		player = GameObject.Find("player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalVars.resetTimer > 0)
		{
			finished = false;
		}
		if(myTrigger != TriggerType.NONE)
		{
			if(myCutscene.finished && !replayable)
			{
				finished = true;
			}
			if(myTrigger == TriggerType.ONCOLLISION)
			{
				if(player.position.x > triggerBounds.bounds.min.x && player.position.x < triggerBounds.bounds.max.x&&player.position.y > triggerBounds.bounds.min.y && player.position.y < triggerBounds.bounds.max.y && !myCutscene._played&&!finished)
				{
					myCutscene._triggerPlay = true;
				}
			}
			else if(myTrigger == TriggerType.ONPRESSED)
			{
				if(Input.GetKey(triggerKey) && player.position.x > triggerBounds.bounds.min.x && player.position.x < triggerBounds.bounds.max.x&&player.position.y > triggerBounds.bounds.min.y && player.position.y < triggerBounds.bounds.max.y && !myCutscene._played&&!finished)
				{
					myCutscene._triggerPlay = true;
				}
			}
		}
	}
}
