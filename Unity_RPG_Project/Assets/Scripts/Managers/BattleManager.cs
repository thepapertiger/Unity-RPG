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
using System.Linq;

public class BattleManager : Singleton<BattleManager> {

    protected BattleManager() { } //null constructor

	public Stats PlayerStats;
	public Stats MonsterStats;

	//private Canvas BattleCanvas;
	private Text[] texts;		//for getting an array of texts of which are children to the BattleCanvas
	private Text canvas_text;	//text in center of canvas used for DEFEAT or VICTORY announcement
	private Text bot_text;		//text on bottom showing player stat
	private Text top_text;		//text on top showing monster stat
	private List<Stats> turn_order = new List<Stats>();
	private Stats current_stats;
	private int ListIndex = 0;
	private bool player_turn = false;

	protected override void Awake()
	{
		//BattleCanvas = GetComponent<Canvas> ();
		MonsterStats = new Stats ("GenericMonster", false, 1, 50, 0, 5, 5);
		PlayerStats = new Stats ("GenericPlayer", true, 1, 100, 100, 10, 10);
		gameObject.SetActive(false);	//hide BattleCanvas from game view

		//sets up turn order for testing
		turn_order.Add (PlayerStats);
		turn_order.Add (MonsterStats);
		turn_order = turn_order.OrderByDescending(x => x.Agility).ToList();
        base.Awake();
	}

	// Use this for initialization
	void Start () 
	{
        PlayerStats = Player.Instance.GetComponent<Stats>();
        texts = GetComponentsInChildren<Text> ();
		current_stats = turn_order.ElementAt(ListIndex);

		foreach(Text child in texts)
		{
			if (child.name == "CanvasText")
				canvas_text = child;
			else if (child.name == "BottomText")
				bot_text = child;
			else if(child.name == "TopText")
				top_text = child;
		}

		canvas_text.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//switch between turns
		Debug.Log(current_stats.Name + "'s turn");
		if (current_stats.Playable) 
		{
			//player action
		} 
		else 
		{
			current_stats.Attack (PlayerStats);
		}

		ListIndex++;
		if (ListIndex >= turn_order.Count)
			ListIndex = 0;
		current_stats = turn_order.ElementAt (ListIndex);

		//update texts
		bot_text.text = PlayerStats.Name + "\nHP: " + PlayerStats.HP + "\nMP: " + PlayerStats.MP;
		top_text.text = MonsterStats.Name + "\nHP: " + MonsterStats.HP + "\nMP: " + MonsterStats.MP;

		//checks for DEFEAT and VICTORY
		if (PlayerStats.HP <= 0)
		{
			StartCoroutine(BattleDefeat());
		}
		else if (MonsterStats.HP <= 0)
		{
			StartCoroutine(BattleVictory());
		}
	}

	/// <summary>
	/// Public function that Player can call when initiating a battle
	/// </summary>
	public void Encounter(Stats hit_monster)
	{
		Player.Instance.PlayersTurn = false;	//disable player movement controls
		gameObject.SetActive(true);				//activate BattleCanvas so it can be seen
		MonsterStats = hit_monster;				//sets the Monster stats to that of encountered monster

		//sets turn order and resets the ListIndex
		turn_order.Add (PlayerStats);
		turn_order.Add (MonsterStats);
		turn_order = turn_order.OrderByDescending(x => x.Agility).ToList();
		ListIndex = 0;
	}


	/// <summary>
	/// Used for testing. Prints the order of battle in the console.
	/// </summary>
	public void PrintOrder()
	{
		foreach (Stats t in turn_order)
			Debug.Log (t.Name);
	}

	/// <summary>
	/// Have the player attack the Monster
	/// </summary>
	public void PlayerAttack()
	{
		PlayerStats.Attack (MonsterStats);
		player_turn = false;
		//PrintOrder ();
	}

	/// <summary>
	/// Exits the battle when the player loses
	/// </summary>
	IEnumerator BattleDefeat()
	{
		Debug.Log ("Defeat triggered");
		canvas_text.text = "DEFEAT!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (1);
		canvas_text.gameObject.SetActive (false);
		gameObject.SetActive(false);
		GameManager.Instance.GameOver();
	}

	/// <summary>
	/// Exits the battle when the player wins
	/// </summary>
	IEnumerator BattleVictory()
	{
		Debug.Log ("Victory triggered");
		canvas_text.text = "VICTORY!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (1);
		canvas_text.gameObject.SetActive (false);
		gameObject.SetActive(false);
	}
}
