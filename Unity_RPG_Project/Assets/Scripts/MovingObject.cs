/* NAME:            MovingObject.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This script is the base class for moving objects. 
 *                  It has an AttemptMove() to either move 
 * 					up/down/left/right, or call OnCantMove(). The derived class should
 * 					call the public function AttemptMove() in order to move. 
 * REQUIREMENTS:    None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

	protected MovingObject () {} //prevents construction

	//these can be adjusted in editor
	public float MoveTime = 0.1f;	//rate of movement [seconds]
	public LayerMask BlockingLayer; //the layer that will block moving objects

	protected float CurrentVelocity = 0;
	protected BoxCollider2D BoxCollider; //reference to this component
    protected bool IAmMoving = false;

	private Rigidbody2D Rigidbody2D;			//reference to this component
	private float InverseMoveTime;		//so division is done once [1/seconds]

	//initializations
	protected virtual void Awake() {
		BoxCollider = GetComponent<BoxCollider2D> (); //find component reference
		Rigidbody2D = GetComponent<Rigidbody2D> (); //find component reference
		InverseMoveTime = 1 / MoveTime;		 //must be calculated here since MoveTime isnt static
	}

	/// <summary>
	/// returns true if successful move, and updates hit with collidee's location
	/// </summary>
	protected bool Move (int x_dir, int y_dir, out RaycastHit2D hit) {
		Vector2 start = transform.position; //get current position
		Vector2 end = start + new Vector2 (x_dir, y_dir); //the x_dir/y_dir are user input values 1, 0, or -1
		
		BoxCollider.enabled = false; //avoid lincast from hitting this box collider
		hit = Physics2D.Linecast(start, end, BlockingLayer); //see if there is somthing blocking
		BoxCollider.enabled = true;  //renable after calculation
        
		if (hit.transform != null) //onCantMove will be called by the AttemptMove function
			return false;

        if (!IAmMoving) {
            IAmMoving = true;
            StartCoroutine(SmoothMovement(end));
        }
        return true;
	}

	/// <summary>
	/// Move the caller from current position to "end" at the rate of "MoveTime"
	/// </summary>
	protected IEnumerator SmoothMovement (Vector3 end) {

		float sqr_remaining_dist = (transform.position - end).sqrMagnitude; //transform.position for v3 subtraction (Rigidbody2D=v2)
		while (sqr_remaining_dist > float.Epsilon) { //float.Epsilon is ~~0, i guess to round the movement)
			Vector3 new_position = Vector3.MoveTowards(Rigidbody2D.position, end, InverseMoveTime * Time.deltaTime);
			/* MoveTime = (seconds/unit)
			(Units/second)(passed time) = How many units moved 
			repeat until Rigidbody2D.position is at end */
			Rigidbody2D.MovePosition (new_position);
			sqr_remaining_dist = (transform.position - end).sqrMagnitude;
			yield return null; //wait for a frame before loop reiteration
		}
        IAmMoving = false;
	}

	/// <summary>
	/// Attempt to move, otherwise call OnCantMove().
	/// Designed to interact with a single component type.
	/// </summary>
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
	protected abstract void OnCantMove <T> (T component) //no brackets since abstract //play thumpty sound
		where T : Component;
	
}
