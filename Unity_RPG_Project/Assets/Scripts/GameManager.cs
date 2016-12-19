/* GameManager.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: The Game Manager that is always open and manages the flow of the game as it is played.
 * 				It is not destroyed when new levels and scenes are loaded.
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic; //for lists
using UnityEngine;

public class GameManager : MonoBehaviour {
	//publics to be defined in editor
	public static GameManager instance = null; //the static variable for singleton pattern
	public float turn_delay = 0.1f; //time between "turns" in realworld for movements
	[HideInInspector] public bool players_turn = true; //player moves before monsters

	private List<Monster> monsters; //list of monsters in current scene	*/
	private bool monsters_moving; //tells whether monsters are moving across frames

	//called on initialization
	void Awake () {
		//singleton pattern to ensure only one GameManager
		if (instance == null)
			instance = this;
		else
			Destroy (gameObject);
		
		DontDestroyOnLoad (gameObject); //The GameManager should always exist across scenes
		monsters = new List<Monster> ();
	}

	//called once per frame
	void Update () {
		if (!players_turn && !monsters_moving) {
			StartCoroutine (MoveMonsters ());
		}
	}

	/// <summary>
	/// the Monsters can add themselves to the GameManager's list by calling this
	/// </summary>
	/// <param name="a_monster">A monster.</param>
	public void AddMonsterToList (Monster a_monster) {
		monsters.Add (a_monster); //note that this is a class object not a GameObject
	}

	/// <summary>
	/// a coroutine to move monsters across frames
	/// </summary>
	/// <returns>The monsters.</returns>
	IEnumerator MoveMonsters () {
		monsters_moving = true;
		yield return new WaitForSeconds(turn_delay); //pause before code
		if (monsters.Count == 0) {
			yield return new WaitForSeconds (turn_delay); //pause even if there is no monster
														//which evens out player movement
		}

		for (int i = 0; i < monsters.Count; i++) {
/*TODO			monsters [i]. ***MoveFunction*** ();
			yield return new WaitForSeconds (monsters [i].monsterMoveTime);	*/
		}

		players_turn = true;
		monsters_moving = false;
	}

	/// <summary>
	/// Stops the game by disabling the Game Manager.
	/// </summary>
	public void GameOver() {
		enabled = false;
	}
}
