/* MovingObject.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: This is the PLayer's script
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {
	
	private Animator animator;
	private int max_hp; //set player_hp <= max_hp when healing
	private int player_hp; //current hp for scene
	private int player_attack_damage; //current_attack_damage for the scene

	// Use this for initialization
	protected override void Start () { //overrides the MovingObject's Start function
		animator = GetComponent<Animator>();
		player_hp = StatsManager.instance.player_hp;
		base.Start ();
	}

	//called when the player is disabled (at the end of the current scene
	void onDisable () {
		StatsManager.instance.player_hp = player_hp;
	}

	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.players_turn)
			return;

		int horizontal = 0;
		int vertical = 0;

		//these return 1, 0r, -1 depending on directional arrows0
		horizontal = (int) Input.GetAxisRaw ("Horizontal");
		vertical = (int) Input.GetAxisRaw ("Vertical");

		if (horizontal != 0) //go horzontal only for diagonal input
			vertical = 0;

		//execute the following only if user input
		if (horizontal != 0 || vertical != 0) {
			//determine which direction to show the sprite
			if (horizontal == 0 && vertical > 0)
				animator.SetTrigger ("PlayerUp");
			else if (horizontal == 0 && vertical < 0)
				animator.SetTrigger ("PlayerDown");
			else if (horizontal > 0 && vertical == 0)
				animator.SetTrigger ("PlayerRight");
			else if (horizontal < 0 && vertical == 0)
				animator.SetTrigger ("PlayerLeft");
			//attempt to move with user's input
			AttemptMove<Monster> (horizontal, vertical);
		}
	}

	/// <summary>
	/// This overrides class MovingObject's AttemptMove function which attempt to move the caller
	/// </summary>
	/// <param name="x_dir">X dir.</param>
	/// <param name="y_dir">Y dir.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	protected override void AttemptMove <T> (int x_dir, int y_dir) {
		base.AttemptMove<T> (x_dir, y_dir);
		CheckIfGameOver ();
		GameManager.instance.players_turn = false;
	}

	/// <summary>
	/// This overrides MovingObject's function. It is called if the caller cannot move as attempted
	/// </summary>
	/// <param name="component">Component.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	protected override void OnCantMove<T> (T component) {
		//TODO: define what the player does when run into a blocking object
	}

	/// <summary>
	/// This is called when the mover runs into a trigger
	/// </summary>
	/// <param name="collidee">Collidee.</param>
	private void OnTriggerEnter2D (Collider2D collidee) {
		//TODO: this is for entering doors to trigger a scene changeS
	}

	/// <summary>
	/// Checks if the player has died to end the game.
	/// </summary>
	private void CheckIfGameOver () {
		if (player_hp <= 0)
			GameManager.instance.GameOver();
	}

}
