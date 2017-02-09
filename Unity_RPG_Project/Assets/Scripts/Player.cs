/* NAME:            Player.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is the Player's script. It derives from MovingObject and is a Singleton.
 *                  Singleton pattern is harcoded at the bottom since this class cannot derive
 *                  from the Singleton base class because it already derives from MovingObject.
 *                  MovingObject has more implementations so it is best derived.
 *                  The player is truly the manager of the entire game's state, since the game 
 *                  responds to the player's actions.
 * REQUIREMENTS:    Base class MovingObject.cs must be present.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject
{
    //TODO: remove this
    //temporary until update BattleManager to check state machine
    public bool PlayersTurn = true;

    public int Chips = 100;

    protected Player() { } //constructor cannot be used - is null

    private static Player BackingInstance; //the backing variable for singleton pattern
	private static object LockObject = new object ();
    private static bool applicationIsQuitting = false;

    private Animator animator;
	private Stats player_stats;
    private Vector2 Front = Vector2.down; //indicates the direction the player is facing
    private float PlayerVelocity = 0;

    protected override void Awake()
    {
        if (BackingInstance != null)
            Destroy(gameObject);
        else {
            BackingInstance = Instance;
            DontDestroyOnLoad(BackingInstance);
            
            //initializations
            animator = GetComponent<Animator>();
            player_stats = GetComponent<Stats>();
            //PlayerHP = MaxHP;
            base.Awake();
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsState(GameStates.IdleState) && IAmMoving) {
            GameManager.Instance.SetState(GameStates.PlayerMovingState);
            animator.SetBool("IAmMoving", IAmMoving);
        }
        else if (GameManager.Instance.IsState(GameStates.PlayerMovingState) && !IAmMoving) {
            GameManager.Instance.SetState(GameStates.IdleState);
            animator.SetBool("IAmMoving", IAmMoving);
        }
            
        if (!GameManager.Instance.IsState(GameStates.IdleState))
            return;

        int horizontal = 0;
        int vertical = 0;

        //these return 1, 0, -1 depending on directional arrows
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0) //go horzontal only for diagonal input
            vertical = 0;

        //execute the following only if directional input
        if (horizontal != 0 || vertical != 0) {
            //determine which direction to show the sprite
            if (horizontal == 0 && vertical > 0) {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerUp"))
                    animator.SetTrigger("PlayerUp"); //set the sprite to face up
                Front = Vector2.up; //set front to be "up"
            }
            else if (horizontal == 0 && vertical < 0) {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDown"))
                    animator.SetTrigger("PlayerDown");
                Front = Vector2.down;
            }
            else if (horizontal > 0 && vertical == 0) {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRight"))
                    animator.SetTrigger("PlayerRight");
                Front = Vector2.right;
            }
            else if (horizontal < 0 && vertical == 0) {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerLeft"))
                    animator.SetTrigger("PlayerLeft");
                Front = Vector2.left;
            }
            //attempt to move with user's input
            AttemptMove<Monster>(horizontal, vertical);
        }
        //if the spacebar is pressed
        else if (Input.GetButtonDown("Accept")) {
            //setup to send a raycast to detect if
            //there is an object in fron of player
            Vector2 start = transform.position;
            Vector2 end = start + Front;

            BoxCollider.enabled = false; //avoid detecting player
            RaycastHit2D hit = Physics2D.Linecast(start, end);
            BoxCollider.enabled = true;

            if (hit.transform != null) //if something is in front
            {
                Interactable hit_component = hit.transform.GetComponent<Interactable>();
                if (hit_component)
                    hit_component.Interact();
            }
        }
    }
    
    /// <summary>
    /// This overrides class MovingObject's AttemptMove function which attempt to move the caller
    /// </summary>
    protected override void AttemptMove<T>(int x_dir, int y_dir)
    {
        base.AttemptMove<T>(x_dir, y_dir);
        CheckIfGameOver(); //maybe player ran into damaging object
    }

    /// <summary>
    /// This overrides MovingObject's function. It is called if the caller cannot move as attempted
    /// </summary>
    protected override void OnCantMove<T>(T component)
    {
        //TODO: define what the player does when run into a blocking object
    }

    /// <summary>
    /// This is called when the mover runs into a trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collidee)
    {
        //TODO: this is for entering doors to trigger a scene changeS
    }

    /// <summary>
    /// Checks if the player has died to end the game.
    /// </summary>
    private void CheckIfGameOver()
    {
		if (player_stats.HP <= 0)
            GameManager.Instance.GameOver();
    }

    /// <summary>
	/// This is the Instance property for the Singleton Pattern.
	/// </summary>
    public static Player Instance
    {
        get
        {
            if (applicationIsQuitting)
            { //do not create another after application quits
                Debug.LogWarning("[Singleton] Instance 'Player' " +
                    "already destroyed on aplication quit.");
                return null;
            }
            lock (LockObject)
            { //use lock to ensure only one thread is accesing this critical code block
                if (BackingInstance == null)
                {

                    BackingInstance = (Player)FindObjectOfType(typeof(Player));

                    if (FindObjectsOfType(typeof(Player)).Length > 1)
                    { //if there are more than one, error msg
                        Debug.LogError("[Singleton] There are more than one singleton! " +
                            "Try reopening the scene.");
                        return BackingInstance;
                    }

                    if (BackingInstance == null)
                    { //if there are none, create singleton
                        Debug.LogError("There is no player.");

                    }
                }

                return BackingInstance;
            }
        }
    }

    /// <summary>
    /// This is to prevent a buggy ghost of the Instance
    /// after the singleton is destroyed.
    /// </summary>
    public void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

}
