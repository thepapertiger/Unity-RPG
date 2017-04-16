/* NAME:            NPC.cs
 * AUTHOR:          Emmilio Segovia, Kevin Huang
 * DESCRIPTION:     This script manages and times the movement of NPCs in the overworld.
 * REQUIREMENTS:    Base class MovingObject.cs must be present.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MovingObject {

	//CLASS VARIABLES
	[Header("Timing")]
	[Range(0, 30)] public float MaxWaitTime;
	[Range(0, 30)] public float MinWaitTime;

	[Header("X bounds")]
	public int MaxX;
	public int MinX;

	[Header("Y bounds")]
	public int MaxY;
	public int MinY;

	private bool Ready = true;

	//For gizmos
	private Vector4 opaque = new Vector4 (0, 0, 0, 1.0f);
	private Vector4 translucent = new Vector4 (0, 0, 0, 0.25f);
	private Vector4 goodColor = (Vector4)(Color.green) - new Vector4 (0, 0, 0, 1f);
	private Vector4 badColor = (Vector4)(Color.red) - new Vector4 (0, 0, 0, 1f);


	//Generate random integer between a & b inclusive
	public int RandomInt(int a, int b)
	{
		return UnityEngine.Random.Range (a, b+1);
	}

	//Moves NPC
	public void MoveNPC() {
		
		int x_mov;
		int y_mov;

		//Determine x movement
		if (transform.position.x <= MinX)
			x_mov = RandomInt (0, 1);
		else if (transform.position.x >= MaxX)
			x_mov = RandomInt (-1, 0);
		else
			x_mov = RandomInt (-1, 1);

		//Determine y movement
		if (transform.position.y <= MinY)
			y_mov = RandomInt (0, 1);
		else if (transform.position.y >= MaxY)
			y_mov = RandomInt (-1, 0);
		else
			y_mov = RandomInt (-1, 1);

		//MOVE!!!
		AttemptMove<Player> (x_mov, y_mov);
	}

	//Calls MoveNPC() when timer is up
	IEnumerator MovementTimer() {
		Ready = false;
		yield return new WaitForSeconds (UnityEngine.Random.Range(MinWaitTime, MaxWaitTime));
		MoveNPC ();
		Ready = true;
	}

	//Does nothing
	protected override void OnCantMove<T> (T component)
	{
		return;
	}
		
	void OnDrawGizmosSelected() {
		
		float CenterX = (MinX + MaxX) / 2.0f;
		float CenterY = (MinY + MaxY) / 2.0f;
		float LengthX = Mathf.Abs (MaxX - MinX);
		float LengthY = Mathf.Abs (MaxY - MinY);

		Vector4 gizColor;
		bool badParams = (MaxX <= MinX || MaxY <= MinY);
		if (badParams)
			gizColor = badColor;
		else
			gizColor = goodColor;

		Gizmos.color = gizColor + opaque;
		Gizmos.DrawWireCube (new Vector3 (CenterX, CenterY, 0), new Vector3 (LengthX, LengthY, 0));

		Gizmos.color = gizColor + translucent;
		Gizmos.DrawCube (new Vector3 (CenterX, CenterY, 0), new Vector3 (LengthX, LengthY, 0));
	}

	void Start() {
		if (MaxX <= MinX)
			Debug.LogWarning (gameObject.name + ": Max X must be greater than Min X.");
		if (MaxY <= MinY)
			Debug.LogWarning (gameObject.name + ": Max Y must be greater than Min Y.");
	}

	void Update() {
        transform.rotation = Quaternion.Euler(Vector3.zero);
		if (Ready) {
			StartCoroutine (MovementTimer ());
		}
	}
		

}