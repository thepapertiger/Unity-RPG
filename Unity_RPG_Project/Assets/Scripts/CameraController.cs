/* CameraController.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: 	This scripts simply moves the camera with the player after everything
 * 					else has been calculated.
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Update is called once per frame
	void LateUpdate () {
		Vector3 player_position = Player.Instance.transform.position; //get player's position
		player_position += new Vector3 (0, 0, -10); //move the back -10 so camera can see everything
		transform.position = player_position; //set the camera's new position
	}
}
