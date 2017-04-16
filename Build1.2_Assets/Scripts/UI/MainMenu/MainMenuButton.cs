/* NAME:            MenuButton.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This script defines the menu button's actions, including toggling
 *                  the main menu canvas' "enabled" status.
 *                  To change game states, you should call the public function 
 *                  IsState(GameStates._____); //pass in enum
 *                  to make sure that the current state is the expected state then call
 *                  SetState(GameStates._____); //pass in enum
 * REQUIREMENTS:    None
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerClickHandler
{
    public bool IsEnabled = true;
    
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (IsEnabled) {
            //only works if in IdleState
            if (GameManager.Instance.IsState(GameStates.IdleState)) {
                //toggle active status
                UIManager.Instance.MenuCanvas.SetActive(true);
                GameManager.Instance.SetState(GameStates.MainMenuState);
                Player.Instance.UpdateParty();
            }
            else if (GameManager.Instance.IsState(GameStates.MainMenuState)) {
                UIManager.Instance.CloseMainMenu();
            }
        }
    }

}
