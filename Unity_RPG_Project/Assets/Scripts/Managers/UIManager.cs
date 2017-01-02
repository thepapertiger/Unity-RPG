/* NAME:            UIManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     Manages the UI elements outside of battle including Dialogue and menus.
 *                  It is a singleton.
 * REQUIREMENTS:    Base class Singleton.cs must be present.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    protected UIManager() { } //prevent constructor from being used

    [SerializeField]
    protected GameObject DialogueCanvas;
    [SerializeField]
    protected Text DialogueText;
    [SerializeField]
    protected Text  NextDialogueText;
    [SerializeField]
    protected string NextDialogueMessage;
    public float SecondsBetweenChar = 0.05f;

    private static bool IsPrinting = false; //tells Update when printing is in progress
    private static bool SkipAnimation = false; //tells corouines when user wants to skip

    void Start()
    {
        DialogueCanvas.SetActive(false); //dialogue is inactive when game starts
#if UNITY_EDITOR || UNITY_STANDALONE
        NextDialogueMessage = "<Press Space>";
#else
        Debug.LogError("Dialogue controls not setup for this platform");
#endif
    }

    void Update()
    {
        //stops printing if currently printing and player wishes to skip
        if (IsPrinting && Input.GetButtonDown("Accept")) {
            SkipAnimation = true;
        }
    }

    /// <summary>
    /// This activate the dialogue box and runs it until user is done reading.
    /// It will eventually invoks the interactor's Next() function to return control
    /// to the Interactable. This does not disable player movement, that is up to
    /// the Interactable to do if desired.
    /// </summary>
    public void RunDialogue(string message, Interactable interactor)
    {
        DialogueCanvas.SetActive(true);
        StartCoroutine(PrintString(message, interactor)); //start printing the message
    }

    /// <summary>
    /// Print a message, one character at a time, at a desired speed.
    /// It is skippable and invokes the interactor's Next() function.
    /// </summary>
    IEnumerator PrintString(string message, Interactable interactor)
    {
        int string_length = message.Length; //save length
        int i = 0; //index to get each character from 0 to string_length
        DialogueText.text = "";

        IsPrinting = true;
        while (i < string_length) //printing loop
        {
            if (SkipAnimation) {
                SkipAnimation = false;
                DialogueText.text = message;
                break;
            }
            DialogueText.text += message[i];
            i++;
            if (i < string_length) //wait before next char
                yield return new WaitForSeconds(SecondsBetweenChar);
        }
        IsPrinting = false;

        NextDialogueText.text = NextDialogueMessage; //display message to go to next dialogue
        yield return null; //wait a fram to let ButtonDown reset
        while (!Input.GetButtonDown("Accept")) {
            yield return null; //wait for user to finish reading, and push "accept"
        }
        DialogueCanvas.SetActive(false);
        DialogueText.text = "";
        NextDialogueText.text = "";
        interactor.Next();
    }

}