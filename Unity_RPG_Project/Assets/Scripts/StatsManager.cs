/* StatsManager.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: The script class keeps track of the player and enemy stats and is not destroyed between loads.
 * 				So enemies and the player in each scene get information from this object.
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {

	public static StatsManager instance = null; //the static variable for singleton pattern

	//Saved data across scenes
	public int player_hp;
	public int player_attack_power;

	// Use this for initialization
	void Awake () {
		//singleton pattern to ensure only one GameManager
		if (instance == null)
			instance = this;
		else
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject); //The StatsManager should always exist across scenes

		player_hp = 100;
		player_attack_power = 10;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
