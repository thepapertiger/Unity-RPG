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
    private Transform target;

	// Use this for initialization
	protected override void Start () {
        /*
	//the Monsters can add themselves to the GameManager's list by calling:
		GameManager.AddMonsterToList (Monster this); 
		*/
        GameManager.Instance.AddMonsterToList(this); //add monsters to GameManager's list
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

        //Declare hitPlayer and set it to equal the encountered componenet.
        Player hitPlayer = component as Player;
    }

    /// <summary>
    /// if player reaches the monster's line of sight, the monster chases after the player
    /// </summary>
    private void DetectPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f, 1 << LayerMask.NameToLayer("Player"));
        int xDir = 0;
        int yDir = 0;
        //If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        //If the difference in positions is not approximately zero (Epsilon) do the following:
        else
        {
            //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject.tag == "Player")
            {
                AttemptMove<Player>(xDir, yDir);
            }
        }
    }
	

}
