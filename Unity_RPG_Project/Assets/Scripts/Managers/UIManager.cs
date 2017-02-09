/* NAME:            UIManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     Manages the UI elements outside of battle including Dialogue and menus.
 *                  It is a singleton.
 * REQUIREMENTS:    Base class Singleton.cs must be present.
 *                  If RunDialogue() is called, interactable objects must call their inherited
 *                  function: QuitDialogue();
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    protected UIManager() { } //prevent constructor from being used

    public GameObject DialogueCanvas;
    public float SecondsBetweenChar = 0.05f;
    [HideInInspector]
    public Button MainMenuButton;
    [HideInInspector]
    public GameObject BackgroundBlur;
    [HideInInspector]
    public GameObject MenuCanvas;
    /// <summary>
    /// These are the scroll view content panels that will parent items in inventory
    /// </summary>
    public GameObject GoodiesGrid;
    public GameObject WeaponsGrid;
    public GameObject GearGrid;
    public GameObject MaterialsGrid;
    public GameObject KeysGrid;
    public GameObject ItemSelectGlow;
    public Image MainMenuDetailsImage;
    public Text MainMenuDetailsText;
    public ItemBase SelectedItem = null;

    protected Text DialogueTextRef;
    protected Text  DialogueInstructionRef;
    protected string DialogueInstruction;

    private Queue<IEnumerator> MessageQueue = new Queue<IEnumerator>();
    private static bool CoroutineRunning = false; //tells whether coroutine is running
    private static bool IsPrinting = false; //tells Update when printing is in progress
    private static bool SkipAnimation = false; //tells corouines when user wants to skip
    private GameObject ActiveGrid;

    /// <summary>
    /// Switches the item-type grid displaying all items of a certain type.
    /// Grids are public, pass in the one you want to switch to.
    /// </summary>
    public void SwitchItemGrid (GameObject new_grid)
    {
        UIManager.Instance.SelectedItem = null;
        ItemSelectGlow.SetActive(false);
        MainMenuDetailsImage.sprite = null;
        MainMenuDetailsText.text = "";
        ActiveGrid.SetActive(false);
        new_grid.SetActive(true);
        ActiveGrid = new_grid;
    }

    protected override void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        DialogueInstruction = "<Press Space>";
#else
        Debug.LogError("Dialogue controls not setup for this platform");
#endif
        base.Awake();
    }

    private void Start()
    {
        DialogueCanvas = GameObject.Find("DialogueCanvas");
        DialogueTextRef = GameObject.Find("DialogueText").GetComponent<Text>();
        DialogueInstructionRef = GameObject.Find("DialogueInstruction").GetComponent<Text>();
        MenuCanvas = GameObject.Find("MenuCanvas");
        MainMenuButton = GameObject.FindObjectOfType<MainMenuButton>().GetComponent<Button>();
        GoodiesGrid = GameObject.Find("GoodiesGrid");
        WeaponsGrid = GameObject.Find("WeaponsGrid");
        GearGrid = GameObject.Find("GearGrid");
        MaterialsGrid = GameObject.Find("MaterialsGrid");
        KeysGrid = GameObject.Find("KeysGrid");
        ItemSelectGlow = GameObject.Find("ItemSelectGlow");
        MainMenuDetailsImage = GameObject.Find("DetailsImage").GetComponent<Image>();
        MainMenuDetailsText = GameObject.Find("DetailsText").GetComponent<Text>();
        ItemSelectGlow.SetActive(false);
        ActiveGrid = GoodiesGrid;
        GoodiesGrid.SetActive(true);
        WeaponsGrid.SetActive(false);
        GearGrid.SetActive(false);
        MaterialsGrid.SetActive(false);
        KeysGrid.SetActive(false);
        DialogueCanvas.SetActive(false); //dialogue is inactive when game starts
        MenuCanvas.SetActive(false);
    }

        void Update()
    {
        //stops printing if currently printing and player wishes to skip
        if (IsPrinting && Input.GetButtonDown("Accept")) {
            SkipAnimation = true;
        }
        //check if next message should be displayed
        if (MessageQueue.Count > 0 && !CoroutineRunning) {
            if (GameManager.Instance.IsState(GameStates.IdleState))
                GameManager.Instance.SetState(GameStates.DialogueState);
            DialogueCanvas.SetActive(true);
            CoroutineRunning = true;
            StartCoroutine(MessageQueue.Dequeue());
        }
    }

    /// <summary>
    /// This activate the dialogue box and runs it until user is done reading.
    /// It will eventually invoks the interactor's Next() function to return control
    /// to the Interactable. This does not disable player movement, that is up to
    /// the Interactable to do if desired.
    /// </summary>
    public void RunDialogue(Interactable interactor, string message)
    {
            MessageQueue.Enqueue(PrintString(message, interactor)); //start printing the message
    }

    /// <summary>
    /// Print a message, one character at a time, at a desired speed.
    /// It is skippable and invokes the interactor's Next() function.
    /// </summary>
    IEnumerator PrintString(string message, Interactable interactor)
    {
        int string_length = message.Length; //save length
        int i = 0; //index to get each character from 0 to string_length
        DialogueTextRef.text = "";

        IsPrinting = true;
        while (i < string_length) //printing loop
        {
            if (SkipAnimation) {
                SkipAnimation = false;
                DialogueTextRef.text = message;
                break;
            }
            DialogueTextRef.text += message[i];
            i++;
            if (i < string_length) //wait before next char
                yield return new WaitForSeconds(SecondsBetweenChar);
        }
        IsPrinting = false;

        DialogueInstructionRef.text = DialogueInstruction; //display message to go to next dialogue
        yield return null; //wait a frame to let ButtonDown reset
        while (!Input.GetButtonDown("Accept")) {
            yield return null; //wait for user to finish reading, and push "accept"
        }
        DialogueTextRef.text = "";
        DialogueInstructionRef.text = "";
        CoroutineRunning = false;
        interactor.Next();
    }
}