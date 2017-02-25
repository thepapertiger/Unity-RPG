using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && GameManager.Instance.IsState(GameStates.MainMenuState)) {
            //toggle active status
            UIManager.Instance.CloseMainMenu();
        }
    }
}
