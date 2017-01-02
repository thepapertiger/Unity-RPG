/* NAME:            GameManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The Game Manager is responsible for saving and loading the game.
 * REQUIREMENTS:    None
 */

using System.Collections;
using System.Collections.Generic; //for lists
using UnityEngine;

public class GameManager : Singleton<GameManager> {


	protected GameManager () {} //constructor cannot be used - is null
    	/// <summary>
	/// Stops the game by disabling the Game Manager.
	/// </summary>
	public void GameOver() {
		enabled = false;
	}
}
