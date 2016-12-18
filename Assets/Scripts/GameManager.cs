using System.Collections;
using System.Collections.Generic; //for lists
using UnityEngine;

public class GameManager : MonoBehaviour {
	//publics to be defined in editor
	public static GameManager instance = null; //the static variable for singleton pattern
	public float turnDelay = 0.1f; //time between "turns" in realworld for movements
	[HideInInspector] public bool playersTurn = true; //player moves before monsters

	//Saved data across scenes
	public int savedPlayerHP; //store and load from here across scenes
	public int savedPlayerMana; //store and load from here across scenes
	private int savedPlayerLevel; //store and load from here across scenes

	private List<Monster> monsters; //list of monsters in current scene	*/
	private bool monstersMoving; //tells whether monsters are moving across frames

	//called on initialization
	void Awake () {
		//singleton pattern to ensure only one GameManager
		if (instance == null)
			instance = this;
		else
			Destroy (gameObject);
		
		DontDestroyOnLoad (gameObject); //The GameManager should always exist across scenes
		monsters = new List<Monster> ();
		savedPlayerHP = 100;
		savedPlayerMana = 100;
		savedPlayerLevel = 1;
	}

	//called once per frame
	void Update () {
		if (!playersTurn && !monstersMoving) {
			StartCoroutine (MoveMonsters ());
		}
	}

	//the Monsters can add themselves to the GameManager's list by calling this
	public void AddMonsterToList (Monster aMonster) {
		monsters.Add (aMonster); //note that this is a class object not a GameObject
	}

	//a coroutine to move monsters across frames
	IEnumerator MoveMonsters () {
		monstersMoving = true;
		yield return new WaitForSeconds(turnDelay); //pause before code
		if (monsters.Count == 0) {
			yield return new WaitForSeconds (turnDelay); //pause even if there is no monster
														//which evens out player movement
		}

		for (int i = 0; i < monsters.Count; i++) {
/*TODO			monsters [i]. ***MoveFunction*** ();
			yield return new WaitForSeconds (monsters [i].monsterMoveTime);	*/
		}

		playersTurn = true;
		monstersMoving = false;
	}

	public void GameOver() {
		enabled = false;
	}
}
