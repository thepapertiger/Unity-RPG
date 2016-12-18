/* Name:			MovingObject
 * Last Modified:	12/17/16
 * Description:		This script is the base class for the player, and can be derived
 * 					by all moving objects. It has an AttemptMove() to either move 
 * 					up/down/left/right, or call OnCantMove(). The derived class should
 * 					implement a public move function to be called by GameManager which
 * 					calls AttemptMove().
*/ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {
	//these can be adjusted in editor
	public float moveTime = 0.1f;	//rate of movement [seconds]
	public LayerMask blockingLayer; //the layer that will block moving objects

	private BoxCollider2D boxCollider; //reference to this component
	private Rigidbody2D rb2D;			//reference to this component
	private float inverseMoveTime;		//so division is done once [1/seconds]

	//initializations
	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D> (); //find component reference
		rb2D = GetComponent<Rigidbody2D> (); //find component reference
		inverseMoveTime = 1 / moveTime;		 //must be calculated here since moveTime isnt static
	}

	//returns true if successful move, and updates hit with collidee's location
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position; //get current position
		Vector2 end = start + new Vector2 (xDir, yDir); //the xDir/yDir are user input values 1, 0, or -1

		boxCollider.enabled = false; //avoid lincast from hitting this box collider
		hit = Physics2D.Linecast(start, end, blockingLayer); //see if there is somthing blocking
		boxCollider.enabled = true;  //renable after calculation

		if (hit.transform != null) //onCantMove will be called by the AttemptMove function
			return false;

		StartCoroutine (SmoothMovement (end));
		return true;
	}

	protected IEnumerator SmoothMovement (Vector3 end) {

		float sqrRemainingDist = (transform.position - end).sqrMagnitude; //transform.position for v3 subtraction (rb2D=v2)
		while (sqrRemainingDist > float.Epsilon) { //float.Epsilon is ~~0, i guess to round the movement)
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			/* moveTime = (seconds/unit)
			(Units/second)(passed time) = How many units moved 
			repeat until rb2D.position is at end */
			rb2D.MovePosition (newPosition);
			sqrRemainingDist = (transform.position - end).sqrMagnitude;
			yield return null; //wait for a frame before loop reiteration
		}
	}

	protected virtual void AttemptMove <T> (int xDir, int yDir)
			where T : Component {

		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);

		if (hit.transform != null) {
			T hitComponent = hit.transform.GetComponent<T> ();
			if (canMove == false && hitComponent != null)
				OnCantMove (hitComponent);
		}
	}

	protected abstract void OnCantMove <T> (T component) //no brackets since abstract //play thumpty sound
		where T : Component;
	
}
