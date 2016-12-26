/* Monster.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: This is a place holder so that the Game Manager can compile.
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MovingObject {

    private Animator animator_monster;
    private GameObject player;

	// Use this for initialization
	protected override void Start () {
        /*
	//the Monsters can add themselves to the GameManager's list by calling:
		GameManager.AddMonsterToList (Monster this); 
		*/
        GameManager.AddMonsterToList(Monster this); //add monsters to GameManager's list
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        animator_monster = GetComponent<Animator>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //start battle screen, get component of the battle game object, or maybe the game manager will handle this
            Debug.Log("Monster wants to battle!");
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        //TODO: define what the monster does when run into a blocking object
    }

    /// <summary>
    /// if player reaches the monster's line of sight, the monster chases after the player
    /// </summary>
    private void DetectPlayer()
    {

<<<<<<< HEAD
    }
=======
	}

>>>>>>> ce98d42e47466566a4bb2364e09b67ad1f5545cc
}
