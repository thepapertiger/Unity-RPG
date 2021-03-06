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
	private Text desc_text; 	//text that describes the battle, etc.

	private Button[] buttons;
	private Button ATTACK_Button;
	private Button DEFEND_Button;
	private Button ITEMS_Button;
	private Button RUN_Button;
	private GameObject OptionsMenu; 

	private List<Stats> turn_order = new List<Stats>();	//list that will be ordered to keep track of turn order
	private Stats current_stats;						//the current stat taking its turn in combat
	//private bool player_turn = false;

	//ListIndex property: keeps track of whose turn it is in battle
	private int _ListIndex = 0;
	public int ListIndex
	{
		get {return _ListIndex; }
		set 
		{
			_ListIndex = value;
			if (_ListIndex >= turn_order.Count)
				_ListIndex = 0;
		}
	}

	//option states
	public enum states {NULL = 0, ATTACK = 1, DEFEND = 2, ITEMS = 3, RUN = 4};
	private states curr_state;

	protected override void Awake()
	{
		MonsterStats = new Stats ("GenericMonster", false, 1, 50, 0, 5, 5, 5, 5, 5, 5);
		PlayerStats = new Stats ("GenericPlayer", true, 1, 100, 100, 10, 10, 10, 10, 10, 10);
		gameObject.SetActive(false);	//hide BattleCanvas from game view
        base.Awake();
	}

	// Use this for initialization
	void Start () 
	{
		//set up battle's turn order
		ListIndex = 0;											//set ListIndex to beginning of the list
        PlayerStats = Player.Instance.GetComponent<Stats>();	//grab Stats from Player and put into PlayerStats
		turn_order.Add (PlayerStats);							//add PlayerStats
		turn_order.Add (MonsterStats);							//add MonsterStats
		turn_order = turn_order.OrderByDescending(x => x.Agility).ToList();	//order tunr_order list by Agility(descending)
		current_stats = turn_order.ElementAt(0);				//set current_stats to first in turn_order
		//curr_state = states.ATTACK;								//curr_state is ATTACK by default
		curr_state = states.NULL;

		//assign Text children to Text instances in file
		texts = GetComponentsInChildren<Text> ();				
		foreach(Text child in texts)
		{
			if (child.name == "CanvasText")
				canvas_text = child;
			else if (child.name == "BottomText")
				bot_text = child;
			else if (child.name == "TopText")
				top_text = child;
			else if (child.name == "DescriptionText")
				desc_text = child;
		}
		canvas_text.gameObject.SetActive (false);	//hides canvas_text
		desc_text.text = "";						//clears desc_text

		//assign Button children to Button instances in file
		buttons = GetComponentsInChildren<Button> ();
		foreach (Button child in buttons) 
		{
			if (child.name == "ATTACK_Button")
				ATTACK_Button = child;
			else if(child.name == "DEFEND_Button")
				DEFEND_Button = child;
			else if(child.name == "ITEMS_Button")
				ITEMS_Button = child;
			else if(child.name == "RUN_Button")
				RUN_Button = child;
		}
		OptionsMenu = GameObject.Find("OptionsMenu");
		OptionsMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//switch between turns
		desc_text.text = current_stats.Name + "'s Turn";
		Debug.Log ("Current State: " + curr_state);
		current_stats.defending = false;
		if (current_stats.Playable) 
		{
			//go up the menu
			if (curr_state == states.NULL) 
			{
				curr_state = states.ATTACK;
				OptionsMenu.SetActive(true);
				ATTACK_Button.interactable = true;
			}
			ChangeSelectionColor(curr_state);
			if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0) 		//up is pressed
			{
				if (curr_state != states.ATTACK)
					--curr_state;
			}
			//go down the menu
			else if(Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)	//down is pressed
			{
				if(curr_state != states.RUN)
					++curr_state;
			}
			//choose highlighted option
			else if(Input.GetButtonDown("Submit"))
			{
				switch (curr_state) 
				{
					case states.ATTACK:
					{
						desc_text.text = current_stats.Name + " attacks " + MonsterStats.Name + "!";
						current_stats.Attack (MonsterStats);
						break;
					}
					case states.DEFEND:
					{
						desc_text.text = current_stats.Name + " defends!";
						current_stats.defending = true;
						break;
					}
					case states.ITEMS:
					{
						break;
					}
					case states.RUN:
					{
						desc_text.text = "Attempting to run...";
						StartCoroutine(BattleEscape());
						break;
					}
				}	//end: switch
				if (curr_state == states.ATTACK)
					ATTACK_Button.interactable = false;
				curr_state = states.NULL;
				OptionsMenu.SetActive(false);
				current_stats = turn_order.ElementAt (++ListIndex);

			}	//end: else if(Input.GetButtonDown("Submit"))
		}	//end: if (current_stats.Playable) 
		else 
		{
			//Monster's turn
			current_stats.Attack (PlayerStats);
			++ListIndex;	//because properties sucks
			current_stats = turn_order.ElementAt (ListIndex);
		}
			
		//update texts
		bot_text.text = PlayerStats.Name + "\nHP: " + PlayerStats.HP + "\nMP: " + PlayerStats.MP;
		top_text.text = MonsterStats.Name + "\nHP: " + MonsterStats.HP + "\nMP: " + MonsterStats.MP;

		//checks for DEFEAT and VICTORY
		if (PlayerStats.HP <= 0)
		{
			OptionsMenu.SetActive (false);
			StartCoroutine(BattleDefeat());
		}
		else if (MonsterStats.HP <= 0)
		{
			OptionsMenu.SetActive (false);
			StartCoroutine(BattleVictory());
		}
	}

	/// <summary>
	/// A function that changes the color of texts in options menu
	/// </summary>
	private void ChangeSelectionColor(states current) {
		Debug.Log ("Color change has been called");
		switch (current) 
		{
			case states.ATTACK:
			{	
				ATTACK_Button.Select();
				break;
			}
			case states.DEFEND:
			{	
				DEFEND_Button.Select();
				break;
			}
			case states.ITEMS:
			{	
				ITEMS_Button.Select();
				break;
			}
			case states.RUN:
			{	
				RUN_Button.Select();
				break;
			}
			default:
			{
				break;
			}
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
	/// Wait for 1 sec
	/// </summary>
	IEnumerator WaitASec()
	{
		yield return new WaitForSeconds (1);
	}

	/// <summary>
	/// Exits the battle when the player loses
	/// </summary>
	IEnumerator BattleEscape()
	{
		Debug.Log ("Escape triggered");
		desc_text.gameObject.SetActive(false);
		if (current_stats.Agility >= 2 * MonsterStats.Agility) 
		{
			//OptionsMenu.SetActive (false);
			canvas_text.text = "ESCAPED!";
			canvas_text.gameObject.SetActive (true);
			yield return new WaitForSeconds (2);
			canvas_text.gameObject.SetActive (false);
			gameObject.SetActive(false);
		} 
		else 
		{
			canvas_text.text = "CANNOT ESCAPE!";
			yield return new WaitForSeconds (2);
		}
	}

	/// <summary>
	/// Exits the battle when the player loses
	/// </summary>
	IEnumerator BattleDefeat()
	{
		Debug.Log ("Defeat triggered");
		desc_text.text = "";
		canvas_text.text = "DEFEAT!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (2);
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
		desc_text.text = "";
		canvas_text.text = "VICTORY!";
		canvas_text.gameObject.SetActive (true);
		yield return new WaitForSeconds (2);
		canvas_text.gameObject.SetActive (false);
		gameObject.SetActive(false);
	}
}
