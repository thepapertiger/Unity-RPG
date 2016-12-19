/* Loader.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: This loads the Game Manager from the main camera object when the gamd is first initialized.
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject game_manager; //reference to the GameManager object
	public GameObject stats_manager; //refernce to the StatsManager object

	void Awake () { //called when this script component starts
		//Note this is the static variable since a GameManager is a class, not a reference
		if (GameManager.instance == null) //if there is no GameManager currently,
			Instantiate(game_manager); //instantiate it

		if (StatsManager.instance == null) //if there is no GameManager currently,
			Instantiate(stats_manager); //instantiate it
	}
}
