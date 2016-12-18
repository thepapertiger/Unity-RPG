using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {
	
	private Animator animator;
	private int maxHP; //set playerHP <= maxHP when healing
	private int playerHP; //current hp for scene

	// Use this for initialization
	protected override void Start () { //overrides the MovingObject's Start function
		animator = GetComponent<Animator>();
		playerHP = GameManager.instance.savedPlayerHP;
		base.Start ();
	}

	//called when the player is disabled (at the end of the current scene
	void onDisable () {
		GameManager.instance.savedPlayerHP = playerHP;
	}

	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.playersTurn)
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

	protected override void AttemptMove <T> (int xDir, int yDir) {
		base.AttemptMove<T> (xDir, yDir);
		CheckIfGameOver ();
		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D (Collider2D collidee) {
		//TODO: this is for entering doors to trigger a scene changeS
	}

	protected override void OnCantMove<T> (T component) {
		//TODO: define what the player does when run into a blocking object
	}

	private void CheckIfGameOver () {
		if (playerHP <= 0)
			GameManager.instance.GameOver();
	}

}
