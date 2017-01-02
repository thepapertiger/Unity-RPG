/* NAME:            StatsManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The script class keeps track of the player and enemy stats and is not destroyed between loads.
 * 				    So enemies and the player in each scene get information from this object.
 * 				    It derives from  singleton.
 * REQUIREMENTS:    Base class Singleton.cs must be present.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : Singleton<StatsManager> {

	protected StatsManager () {} //constructor cannot be used - is null

	public Stats PlayerStats;
	public Stats MonsterStats;

	//called on initialization
	void Awake()
	{
		PlayerStats = Player.Instance.GetComponent<Stats>();
	}

}
