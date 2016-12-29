/* NAME:            Monster.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is a place holder so that the Game Manager can compile.
 * REQUIREMENTS:    Base class MovingObject.cs must be present.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MovingObject {
   
	public Stats MonsterStats;

	private Animator AnimatorMonster;
    private GameObject Player;
    private Transform Target;

	// Use this for initialization
	protected override void Start () {
        /*
	//the Monsters can add themselves to the GameManager's list by calling:
		GameManager.AddMonsterToList (Monster this); 
		*/
        GameManager.Instance.AddMonsterToList(this); //add monsters to GameManager's list
        Player = GameObject.FindGameObjectWithTag("Player").gameObject;
        AnimatorMonster = GetComponent<Animator>();
		MonsterStats = new Stats ("Monster", 1, 50, 0, 5);
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Attempts to move in the direction (x_dir, y_dir)
    /// </summary>
    protected override void AttemptMove<T>(int x_dir, int y_dir)
    {
        base.AttemptMove<T>(x_dir, y_dir);
    }

    /// <summary>
    /// Called on when the monster enters a collision
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //start battle screen, get component of the battle game object, or maybe the game manager will handle this
            Debug.Log("Monster wants to battle!");
        }
    }

    /// <summary>
    /// Called if the Monster cannot move
    /// </summary>
    protected override void OnCantMove<T>(T component)
    {
        //TODO: define what the monster does when run into a blocking object

        //Declare hitPlayer and set it to equal the encountered componenet.
        Player hitPlayer = component as Player;
    }

    /// <summary>
    /// if player reaches the monster's line of sight, the monster chases after the player
    /// </summary>
    private void DetectPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f, 1 << LayerMask.NameToLayer("Player"));
        int x_dir = 0;
        int y_dir = 0;
        //If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(Target.position.x - transform.position.x) < float.Epsilon)
        {
            //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            y_dir = Target.position.y > transform.position.y ? 1 : -1;
        }
        //If the difference in positions is not approximately zero (Epsilon) do the following:
        else
        {
            //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            x_dir = Target.position.x > transform.position.x ? 1 : -1;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject.tag == "Player")
            {
                AttemptMove<Player>(x_dir, y_dir);
            }
        }
    }
	

}
