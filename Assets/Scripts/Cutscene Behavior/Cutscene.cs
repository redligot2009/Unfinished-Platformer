using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Dialogue{

	public string name = "argh";
	public string line = "no";
	public string command = "zoom";
	public float commandVal = 13.0f;
}

[System.Serializable]
public class DialogueList{
	public Dialogue[] dialogues;
}

public class Cutscene : MonoBehaviour {
	BoxCollider2D sceneBounds;
	public bool _played;
	public bool _playOnStart;
	public bool _triggerPlay;
	public bool _stopScene;
	public bool _cutSceneHasDuration;
	public bool _hasDialogue;
	public string _dialogueFile;
	public string nameOfScene = "default";
	public int _currDialogue = 1;
	public float _cutSceneDuration = 5f;
	public float _cutSceneTimer;
	bool playedFromStart;
	bool isTyping = false;
	bool cancelTyping = false;
	public bool finished = false;
	public float typeSpeed;
	CameraControls myCamera;
	TextAsset dialogueJSON;
	DialogueList myDialogue;
	Text dialogueGO;
	CanvasGroup dialogueGroup;
	Player player;
	public bool playerCanMove;
	Transform mainObject;

	// Use this for initialization
	void Start () {
		sceneBounds = transform.Find("sceneBounds").GetComponent<BoxCollider2D>();
		myCamera = Camera.main.transform.GetComponent<CameraControls>();
		if(_hasDialogue)
		{
			dialogueJSON = Resources.Load(Path.Combine("Dialogue JSON", _dialogueFile)) as TextAsset;
			myDialogue = new DialogueList();
			string json = dialogueJSON.text;
			myDialogue = JsonUtility.FromJson<DialogueList>(json);
			foreach(Dialogue element in myDialogue.dialogues)
			{
				Debug.Log(element.name + ": " + element.line);
			}
		}
		mainObject = GameObject.Find("Main").transform;
		player = GameObject.Find("player").GetComponent<Player>();
		dialogueGO = GameObject.Find("Dialogue").transform.Find("Text").GetComponent<Text>();
		dialogueGroup = GameObject.Find("Dialogue").GetComponent<CanvasGroup>();
		playedFromStart = _playOnStart;
	}
	
	public virtual void PlayScene()
	{
		mainObject.GetComponent<Inventory>().currentCutscene = this;
		if(_cutSceneHasDuration)
		{
			_cutSceneTimer = _cutSceneDuration;
		}
		myCamera.cutSceneBoundaryTransform = sceneBounds.transform;
		_played = true;
		_playOnStart = false;
		_triggerPlay = false;
		finished = false;
		if(myDialogue.dialogues[_currDialogue].name != "no_name")
		{
			StartCoroutine(TextScroll(myDialogue.dialogues[_currDialogue-1].name + ": " + myDialogue.dialogues[_currDialogue-1].line));
		}
		else
		{
			StartCoroutine(TextScroll(myDialogue.dialogues[_currDialogue-1].line));
		}
	}

	public virtual void EndScene()
	{
		mainObject.GetComponent<Inventory>().currentCutscene = null;
		myCamera.cutSceneBoundaryTransform = null;
		_played = false;
		_currDialogue = 1;
		_stopScene = false;
		finished = true;
		player.canMove = true;
		player.gunEquipped = true;
	}

	public void NextDialogue()
	{
		_currDialogue ++;
		if(_currDialogue <= myDialogue.dialogues.Length)
		{
			if(myDialogue.dialogues[_currDialogue-1].name != "no_name")
			{
				StartCoroutine(TextScroll(myDialogue.dialogues[_currDialogue-1].name + ": " + myDialogue.dialogues[_currDialogue-1].line));
			}
			else
			{
				StartCoroutine(TextScroll(myDialogue.dialogues[_currDialogue-1].line));
			}
		}
	}

	public void ResetCutscene()
	{
		StopAllCoroutines();
		_currDialogue = 1;
		_playOnStart = playedFromStart;
		finished = false;
	}

	IEnumerator TextScroll(string line)
	{
		int letter = 0;
		dialogueGO.text = "";
		isTyping = true;
		cancelTyping = false;
		while(isTyping && !cancelTyping && (letter < line.Length - 1))
		{
			dialogueGO.text += line[letter];
			letter ++;
			yield return new WaitForSeconds(typeSpeed);
		}
		dialogueGO.text = line;
		isTyping = false;
		cancelTyping = false;
	}

	// Update is called once per frame
	void Update () {
		if(player.transform.Find("Animations").GetComponent<Animator>().GetBool("born"))
		{
			if(GlobalVars.resetTimer > 0)
			{
				ResetCutscene();
			}
			if(_playOnStart||_triggerPlay)
			{
				PlayScene();
			}
			if(_cutSceneHasDuration)
			{
				if(_played&&_cutSceneTimer <= 0)
				{
					EndScene();
				}
				if(_cutSceneTimer > 0)
				{
					_cutSceneTimer -= Time.deltaTime;
				}
			}
			else
			{
				if(_stopScene)
				{
					if(!isTyping)
					{
						EndScene();
					}
				}

				if(_hasDialogue)
				{
					
					if(_currDialogue < myDialogue.dialogues.Length+1)
					{
						if(_played&&mainObject.GetComponent<Inventory>().currentCutscene == this)
						{
							Inventory.FadeIn(dialogueGroup);	
							if(myDialogue.dialogues[_currDialogue-1].command == "zoom")
							{
								myCamera.Zoom(myDialogue.dialogues[_currDialogue-1].commandVal);
							}
							if(!playerCanMove)
							{
								player.canMove = false;
								player.gunEquipped = false;
							}
							else
							{
								player.canMove = true;
								player.gunEquipped = true;
							}
						}
						if(Input.GetKeyDown(KeyCode.Space)&&_played)
						{
							if(!isTyping)
							{
								NextDialogue();
							}
							else if(isTyping && !cancelTyping)
							{
								cancelTyping = true;
							}
						}
					}
					else
					{
						_stopScene = true;
					}
				}
			}
		}
		else
		{
			dialogueGroup.alpha = 0;
			ResetCutscene();
		}
		if(!_played && finished)
		{
			if(dialogueGroup.alpha != 0)
			{
				Inventory.FadeOut(dialogueGroup);
			}
			else
			{
				finished = false;
			}
		}
	}
}
