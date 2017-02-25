/* NAME:            Monster.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is a place holder so that the Game Manager can compile.
 * REQUIREMENTS:    Base class MovingObject.cs must be present.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MovingObject {

    //public Stats MonsterStats;
    public float MoveRate = 0.5f;
    private float NextActionTime = 0f;
	private Animator AnimatorMonster;
    private Transform Target;
    private bool Paused = false;

    public void Pause(float pause_time)
    {
        Paused = true;
        StartCoroutine(PauseCoroutine(pause_time));
    }

    private IEnumerator PauseCoroutine(float pause_time)
    {
        yield return new WaitForSeconds(pause_time);
        Paused = false;
    }

	// Use this for initialization
	protected override void Awake () {
        AnimatorMonster = GetComponent<Animator>();
		//MonsterStats = new Stats ("Monster", 1, 50, 0, 5, 5);
        base.Awake();
    }

    private void Start()
    {
        Target = Player.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsState(GameStates.IdleState)) {
            if (!GameManager.Instance.IsState(GameStates.PlayerMovingState))
                NextActionTime = Time.time;
        }
        else if (!Paused) { //only move when game is in idle mode
            if (Time.time > NextActionTime) { //see if it is time to move again
                DetectPlayer();
                NextActionTime += MoveRate; //set the next time to move
            }
        }
    }

    /// <summary>
    /// Attempts to move in the direction (x_dir, y_dir)
    /// </summary>
    protected override void AttemptMove<T>(int x_dir, int y_dir)
    {
        base.AttemptMove<T>(x_dir, y_dir);
    }

    /* ################################################################### NOT WORKING :((
    /// <summary>
    /// Called on when the monster enters a collision
    /// </summary>
    private void OnTriggerEnter2D(Collision2D collision)
    {
        print("collide with " + collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            //start battle screen, get component of the battle game object, or maybe the game manager will handle this
            Debug.Log("Monster wants to battle!");
            BattleManager.Instance.Encounter(this.GetComponent<Stats>());
        }
    }
    */

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f, 1 << LayerMask.NameToLayer("Player"));
        int x_dir = 0;
        int y_dir = 0;
        //If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(Target.position.x - transform.position.x) < float.Epsilon) {
            if (Mathf.Abs(Target.position.y - transform.position.y) < float.Epsilon)
                BattleManager.Instance.Encounter(GetComponent<Stats>());
            else //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
                y_dir = Target.position.y > transform.position.y ? 1 : -1;
        }
        //If the difference in positions is not approximately zero (Epsilon) do the following:
        else {
            //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            x_dir = Target.position.x > transform.position.x ? 1 : -1;
        }

        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject.tag == "Player") {
                AttemptMove<Player>(x_dir, y_dir);
            }
        }
    }
}
