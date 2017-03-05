/* NAME:            1v1Manager.cs
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

public class OneVOneManager : Singleton<OneVOneManager> {

	protected OneVOneManager() { } //null constructor

	public Monster Monster;
	public Stats PlayerStats;
	public Stats MonsterStats;
	//public Text DamageText;
	private Canvas BattleCanvas;

	private Text[] texts;		//for getting an array of texts of which are children to the BattleCanvas
	private Text canvas_text;	//text in center of canvas used for DEFEAT or VICTORY announcement
	private Text bot_text;		//text on bottom showing player stat
	private Text top_text;		//text on top showing monster stat
	private Text desc_text; 	//text that describes the battle, etc.
	private Text dmg1;
	private Text dmg2;
	private Text dmg3;
	private Text dmg4;

	private Button[] buttons;
	private Button ATTACK_Button;
	private Button DEFEND_Button;
	private Button SKILLS_Button;
	private Button ITEMS_Button;
	private Button RUN_Button;
	private GameObject OptionsMenu;
	private List<AudioSource> allAudioSources;

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
	public enum states {NULL = 0, ATTACK = 1, DEFEND = 2, SKILLS = 3, ITEMS = 4, RUN = 5, TARGETING = 9};
	private states curr_state;	//memorizes what is the crrent state the battle_canvas is in

	protected override void Awake()
	{
		//MonsterStats = new Stats ("GenericMonster", false, 1, 50, 0, 5, 5, 5, 5, 5, 5);
		//PlayerStats = new Stats ("GenericPlayer", true, 1, 100, 100, 10, 10, 10, 10, 10, 10);
		BattleCanvas = GetComponent<Canvas>();
		//current_stats = turn_order.ElementAt (0);
		base.Awake();
		gameObject.SetActive(false);    //hide BattleCanvas from game view 
	}

	// Use this for initialization
	void Start () 
	{
		//set up battle's turn order
		ListIndex = 0;											//set ListIndex to beginning of the list for tunr_order
		PlayerStats = Player.Instance.GetComponent<Stats>();	//grab Stats from Player and put into PlayerStats
//		turn_order.Add (PlayerStats);							//add PlayerStats
//		turn_order.Add (MonsterStats);							//add MonsterStats
//		turn_order = turn_order.OrderByDescending(x => x.Agility).ToList();	//order tunr_order list by Agility(descending)
//		current_stats = turn_order.ElementAt(0);				//set current_stats to first in turn_order
		curr_state = states.NULL;

		//assign Text children to Text instances in file
		texts = GetComponentsInChildren<Text> ();				
		foreach(Text child in texts)
		{
			switch(child.name) {
			case "CanvasText":
				canvas_text = child;
				break;
			case "BottomText":
				bot_text = child;
				break;
			case "TopText":
				top_text = child;
				break;
			case "DescriptionText":
				desc_text = child;
				break;
			case "damage1":
				dmg1 = child;
				break;
			case "damage2":
				dmg2 = child;
				break;
			case "damage3":
				dmg3 = child;
				break;
			case "damage4":
				dmg4 = child;
				break;
			}
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
			else if (child.name == "SKILLS_Button")
				SKILLS_Button = child;
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
		//Debug.Log("turn_order size: " + turn_order.Count());
		current_stats.defending = false;
		if (current_stats.Playable) 
		{
			//go up the menu
			OptionsMenu.SetActive(true);
			if (curr_state == states.NULL) 
			{
				Debug.Log ("Switched from NULL to ATTACK");
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
						//target = Select();
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
				case states.SKILLS:
					{
						//DisplayDamage(9999);
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
			//damage display
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
		case states.SKILLS:
			{
				SKILLS_Button.Select();
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
	/// Displays damage taken for characters
	/// </summary>
	public void DisplayDamage(int pos, int damage)
	{
		//Debug.Log ("Displayed damage");
		switch (pos) 
		{
		case 1:
			{
				dmg1.text = damage.ToString ();
				break;
			}
		case 2:
			{
				dmg2.text = damage.ToString ();
				break;
			}
		case 3:
			{
				dmg3.text = damage.ToString ();
				break;
			}
		case 4:
			{
				dmg4.text = damage.ToString ();
				break;
			}
		default:
			{
				Debug.Log ("DisplayDamage: Invalid pos input");
				break;
			}
		}
		//Text DmgTextClone = Instantiate(DamageText, loc, Quaternion.identity) as Text;
		//DmgTextClone.text = damage.ToString();
		//DmgTextClone.transform.SetParent (BattleCanvas.transform, false);
	}

	/// <summary>
	/// Public function that Player can call when initiating a battle
	/// </summary>
	public void Encounter(Monster hit_monster)
	{
		Monster = hit_monster;
		//stop all monster sounds when entering battle
		Monster[] all_monsters = FindObjectsOfType<Monster>();
		foreach (Monster mon in all_monsters) {
			mon.GetComponent<AudioSource>().Stop();
		}
		if (GameManager.Instance.IsState(GameStates.IdleState)) {
			GameManager.Instance.SetState(GameStates.BattleState); //disable player movement
			SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("BattleMusic"));
			gameObject.SetActive(true);             //activate BattleCanvas so it can be seen
			curr_state = states.NULL;

			turn_order.Clear ();
			PlayerStats = Player.Instance.GetComponent<Stats>();
			MonsterStats = hit_monster.GetComponent<Stats> ();
			//sets turn order and resets the ListIndex
			turn_order.Add(PlayerStats);
			turn_order.Add(MonsterStats);
			//print(PlayerStats.Name);
			//print(MonsterStats.Name);
			turn_order = turn_order.OrderByDescending(x => x.Agility).ToList();
			current_stats = turn_order.ElementAt(0);
			ListIndex = 0;
		}
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
	/// Deactivates the battle canvas and changes game state to idle.
	/// </summary>
	private void EndBattle()
	{
		SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("CastleMusic"));
		if (curr_state == states.ATTACK)
			ATTACK_Button.interactable = false;
		curr_state = states.NULL;
		gameObject.SetActive(false);
		turn_order.Clear(); //clear the turn order list for the next encournter
		if (GameManager.Instance.IsState(GameStates.BattleState))
			GameManager.Instance.SetState(GameStates.IdleState);
		else
			Debug.LogError("Game should be in battle state, but you are not.");
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
		if (current_stats.Agility >= 2 * 5) 	//5 is a placeholder value
		{
			//OptionsMenu.SetActive (false);
			canvas_text.text = "ESCAPED!";
			canvas_text.gameObject.SetActive (true);
			yield return new WaitForSeconds (2);
			canvas_text.gameObject.SetActive (false);
			//MonsterStats.gameObject.GetComponent<Monster>().Pause(2f);
			Monster.Pause(2f);
			EndBattle();
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
		EndBattle();
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
		Monster.gameObject.SetActive (false);
		//MonsterStats.gameObject.SetActive(false);
		EndBattle();
	}
}
