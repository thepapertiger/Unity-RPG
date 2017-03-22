/* NAME:            GameManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The Game Manager is responsible for saving and loading the game.
 *                  It keeps the defined states of the game using an animator which has
 *                  a nice live visual representation in the editor.
 *                  Check the game state by calling:
 *                  if (GameManager.Instance.GameState.GetCurrentAnimatorStateInfo(0).IsName("BattleState"))...
 *                  *** OR to avoid typos ***
 *                  if (GameManager.Instance.GameState.GetCurrentAnimatorStateInfo(0)
 *                                                       .IsName(GameStates.BattleState.ToString()))...
 *                                                       
 *                  Note: To change to a certain state, set its trigger by the same name:
 *                          if (GameManager.Instance.GameState.GetCurrentAnimatorStateInfo(0).IsName("IdleState"))
 *                              GameManager.Instance.GameState.SetTrigger("BattleState");
 *                        !!!Notice how we make sure we are in "IdleState" to avoid multiple triggers being set!!!
 * REQUIREMENTS:    None
 */

using System.Collections;
using System.Collections.Generic; //for lists
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Global enum. The possible game states set in the animator state machine.
public enum GameStates {IntroState, IdleState, PlayerMovingState, DialogueState, BattleState, MainMenuState};

public class GameManager : Singleton<GameManager> {

    //for demo
    public GameObject Overall;
    public List<Sprite> CutScenes = new List<Sprite>();
    public int index = 0;
    public GameObject WorldCanvas;

    //gamemanager
    protected GameManager() { } //constructor cannot be used - is null

    private Animator GameState; //The game state machine (see notes in header)
    private HashSet<string> ItemReceivedRecords = new HashSet<string>();

    /// <summary>
    /// This is to be called by scene change triggers in the game
    /// to load a scene.
    /// </summary>
    public void LoadScene(string scene_name)
    {
        if (!SceneManager.GetSceneByName(scene_name ).isLoaded)
            SceneManager.LoadScene(scene_name);
    }

    /// <summary>
    /// This is for item-givers to check if their item was given so
    /// that it will not be given to the player again.
    /// </summary>
    public bool GetReceived (string key)
    {
        return ItemReceivedRecords.Contains(key);
    }

    /// <summary>
    /// This is for item-givers to say that their item was given so
    /// that it will not be given to the player again.
    /// </summary>
    public void SetReceived(string key)
    {
        if (!ItemReceivedRecords.Contains(key))
            ItemReceivedRecords.Add(key);
    }

    /// <summary>
    /// This is called by the main menu save feature
    /// </summary>
    public void SaveGame()
    {
        //store ItemReceivedRecords, stats, etc
    }

    /// <summary>
	/// Stops the game
	/// </summary>
	public void GameOver() {
        Application.Quit();
	}

    protected override void Awake()
    {
        GameState = this.GetComponent<Animator>();
        base.Awake();
    }

    private void Start()
    {
        Player.Instance.Party.Add(ResourceManager.Instance.GetCharacter("Ella"));
        Player.Instance.Party.Add(ResourceManager.Instance.GetCharacter("Darius"));
        Player.Instance.Party.Add(ResourceManager.Instance.GetCharacter("Gunther"));
        Player.Instance.Party.Add(ResourceManager.Instance.GetCharacter("Margarethe"));
        //WorldCanvas.SetActive(false);
    }

    /*
    private void Update()
    {
        if (IsState(GameStates.IntroState) && Input.GetButtonDown("Submit") && index < 5) {
            if (index == 0)
                SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("WeddingSong"));
            Overall.GetComponent<Image>().sprite = CutScenes[index];
            index++;
        }
        if (IsState(GameStates.IntroState) && Input.GetButtonDown("Submit") && index > 4) {
            SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("CastleMusic"));
            Overall.SetActive(false);
            SetState(GameStates.IdleState);
            WorldCanvas.SetActive(true);
        }
    }
    */

    /// <summary>
    /// This is the function to check whether the current game state
    /// is a certain game state from the "GameStates" enum.j
    /// </summary>
    public bool IsState(GameStates state)
    {
        return GameState.GetCurrentAnimatorStateInfo(0).IsName(state.ToString());
    }

    /// <summary>
    /// This is the function to set a trigger of the next desired
    /// game state
    /// </summary>
    public void SetState(GameStates state)
    {
        GameState.SetTrigger(state.ToString());
    }


}
