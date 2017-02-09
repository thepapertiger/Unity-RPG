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

//Global enum. The possible game states set in the animator state machine.
public enum GameStates {IdleState, PlayerMovingState, DialogueState, BattleState, MainMenuState};

public class GameManager : Singleton<GameManager> {

    private Animator GameState; //The game state machine (see notes in header)
    private HashSet<string> ItemReceivedRecords = new HashSet<string>();

    public bool GetReceived (string key)
    {
        return ItemReceivedRecords.Contains(key);
    }

    public void SetReceived(string key)
    {
        if (!ItemReceivedRecords.Contains(key))
            ItemReceivedRecords.Add(key);
    }

    public void SaveGame()
    {
        //store ItemReceivedRecords, stats, etc
    }

    protected GameManager() { } //constructor cannot be used - is null

    protected override void Awake()
    {
        GameState = this.GetComponent<Animator>();
        base.Awake();
    }
    
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

    /// <summary>
	/// Stops the game
	/// </summary>
	public void GameOver() {

	}
}
