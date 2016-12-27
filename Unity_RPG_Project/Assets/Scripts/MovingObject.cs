/* MovingObject.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: 	This is a Singleton therefore, all derived classes will be singleton.
 * 					It is mainly for function organization for readability.
 * 					This script is the base class for the player, and can be derived
 * 					by all SINGLETON moving objects. It has an AttemptMove() to either move 
 * 					up/down/left/right, or call OnCantMove(). The derived class should
 * 					implement a public move function to be called by GameManager which
 * 					calls AttemptMove(). 
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : Singleton<MovingObject> {

	protected MovingObject () {} //prevents construction

	//these can be adjusted in editor
	public float move_time = 0.1f;	//rate of movement [seconds]
	public LayerMask blocking_layer; //the layer that will block moving objects

	protected bool i_am_moving = false;
	protected BoxCollider2D box_collider; //reference to this component

	private Rigidbody2D rigidbody_2d;			//reference to this component
	private float inverse_move_time;		//so division is done once [1/seconds]

	//initializations
	protected virtual void Start () {
		box_collider = GetComponent<BoxCollider2D> (); //find component reference
		rigidbody_2d = GetComponent<Rigidbody2D> (); //find component reference
		inverse_move_time = 1 / move_time;		 //must be calculated here since move_time isnt static
	}

	/// <summary>
	/// returns true if successful move, and updates hit with collidee's location
	/// </summary>
	/// <param name="x_dir">X dir.</param>
	/// <param name="y_dir">Y dir.</param>
	/// <param name="hit">Hit.</param>
	protected bool Move (int x_dir, int y_dir, out RaycastHit2D hit) {
		Vector2 start = transform.position; //get current position
		Vector2 end = start + new Vector2 (x_dir, y_dir); //the x_dir/y_dir are user input values 1, 0, or -1
		
		box_collider.enabled = false; //avoid lincast from hitting this box collider
		hit = Physics2D.Linecast(start, end, blocking_layer); //see if there is somthing blocking
		box_collider.enabled = true;  //renable after calculation
        
		if (hit.transform != null) //onCantMove will be called by the AttemptMove function
			return false;

        i_am_moving = true; //movement is taking place, to not attempt to move again until done
        StartCoroutine (SmoothMovement (end));
        return true;
	}

	/// <summary>
	/// Move the caller from current position to "end" at the rate of "move_time"
	/// </summary>
	/// <returns>The movement.</returns>
	/// <param name="end">End.</param>
	protected IEnumerator SmoothMovement (Vector3 end) {

		float sqr_remaining_dist = (transform.position - end).sqrMagnitude; //transform.position for v3 subtraction (rigidbody_2d=v2)
		while (sqr_remaining_dist > float.Epsilon) { //float.Epsilon is ~~0, i guess to round the movement)
			Vector3 new_position = Vector3.MoveTowards(rigidbody_2d.position, end, inverse_move_time * Time.deltaTime);
			/* move_time = (seconds/unit)
			(Units/second)(passed time) = How many units moved 
			repeat until rigidbody_2d.position is at end */
			rigidbody_2d.MovePosition (new_position);
			sqr_remaining_dist = (transform.position - end).sqrMagnitude;
			yield return null; //wait for a frame before loop reiteration
		}
		i_am_moving = false;
	}

	/// <summary>
	/// Attempt to move, otherwise call OnCantMove().
	/// Designed to interact with a single component type.
	/// </summary>
	/// <param name="x_dir">X dir.</param>
	/// <param name="y_dir">Y dir.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	protected virtual void AttemptMove <T> (int x_dir, int y_dir)
			where T : Component {
	
		RaycastHit2D hit;
		bool canMove = Move (x_dir, y_dir, out hit);

		if (hit.transform != null) {
			T hit_component = hit.transform.GetComponent<T> ();
			if (canMove == false && hit_component != null)
				OnCantMove (hit_component);
		}
	}

	/// <summary>
	/// This is called if the caller cannot move as attempted
	/// </summary>
	/// <param name="component">Component.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	protected abstract void OnCantMove <T> (T component) //no brackets since abstract //play thumpty sound
		where T : Component;
	
}
