/* NAME:            GameManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The Game Manager that is always open and manages the flow of the game as it is played.
 * 				    It is not destroyed when new levels and scenes are loaded.
 *	 			    It derives from  singleton.
 * REQUIREMENTS:    Base class Singleton.cs must be present.
 */

using System.Collections;
using System.Collections.Generic; //for lists
using UnityEngine;

public class GameManager : Singleton<GameManager> {


	protected GameManager () {} //constructor cannot be used - is null

	//publics to be defined in editor
	public float TurnDelay = 0.1f; //time between "turns" in realworld for movements
	[HideInInspector] public bool PlayersTurn = true; //player moves before monsters

	private List<Monster> Monsters; //list of monsters in current scene	*/
	private bool MonstersMoving; //tells whether monsters are moving across frames


	//called on initialization
	void Awake () {
		Monsters = new List<Monster> ();
	}

	//called once per frame
	void Update () {
		if (!PlayersTurn && !MonstersMoving) {
			StartCoroutine (MoveMonsters ());
		}
	}

	/// <summary>
	/// the Monsters can add themselves to the GameManager's list by calling this
	/// </summary>
	public void AddMonsterToList (Monster a_monster) {
		Monsters.Add (a_monster); //note that this is a class object not a GameObject
	}

	/// <summary>
	/// a coroutine to move monsters across frames
	/// </summary>
	IEnumerator MoveMonsters () {
		MonstersMoving = true;
		yield return new WaitForSeconds(TurnDelay); //pause before code
		if (Monsters.Count == 0) {
			yield return new WaitForSeconds (TurnDelay); //pause even if there is no monster
														//which evens out player movement
		}

		for (int i = 0; i < Monsters.Count; i++) {
/*TODO			Monsters [i]. ***MoveFunction*** ();
			yield return new WaitForSeconds (Monsters [i].monsterMoveTime);	*/
		}

		PlayersTurn = true;
		MonstersMoving = false;
	}

	/// <summary>
	/// Stops the game by disabling the Game Manager.
	/// </summary>
	public void GameOver() {
		enabled = false;
	}
}
