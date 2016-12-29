/* NAME:            BattleManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The script class manages the battle and its canvas when battles are triggered. 
 * 				    It derives from  singleton.
 * REQUIREMENTS:    Base class Singleton.cs must be present.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager> {

	public Stats PlayerStats;
	public Stats MonsterStats;

	private Canvas BattleCanvas;

	void Awake()
	{
		PlayerStats = new Stats();
		//MonsterStats = new Stats ("Monster", 1, 50, 0, 5);
		BattleCanvas = GetComponent<Canvas> ();
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Public function that Player can call when colliding with a Monster class
	/// </summary>
	public void Encounter(Stats hit_monster)
	{
		//this.enabled = true;
		gameObject.SetActive(true);
		MonsterStats = hit_monster;
	}
}
