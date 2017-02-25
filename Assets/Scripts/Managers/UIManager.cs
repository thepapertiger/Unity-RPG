/* NAME:            UIManager.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     Manages the UI elements outside of battle including Dialogue and menus.
 *                  It keeps many references public since buttons will activate/deactivate.
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

    [Header("Dialogue")]
    public float SecondsBetweenChar = 0.05f;
    public GameObject DialogueCanvas;
    [SerializeField]
    protected Text DialogueTextRef;
    [SerializeField]
    protected Text DialogueInstructionRef;
    [SerializeField]
    protected string DialogueInstruction = "<Press Space>";
    [SerializeField]
    protected Text QandATextRef;
    [SerializeField]
    protected Text QandAInstructionRef;
    [SerializeField]
    protected string QandAInstruction = "<Click Answer>";
    public GameObject DialogueView;
    public GameObject QandAView;
    public GameObject[] AnswerButtons;
    public Text[] AnswerText;
    private int AnswerSelected = -1;
    [Space(10)]
    
    [Header("Menu General References")]
    public Button MainMenuButtonComponent;
    public MainMenuButton MainMenuButtonScript;
    public GameObject BackgroundBlur;
    public GameObject MenuCanvas;
    public GameObject[] ScrollViews;
    [Space(10)]

    [Header("Menu List Areas References")]
    public GameObject PartyArea;
    public GameObject InventoryArea;
    public GameObject SynopsisArea;
    public GameObject SaveArea;
    public GameObject OptionsArea;
    public GameObject ActiveArea;
    public GameObject ExitPopUp;
    public Text LastSaveTime;
    public Text LastSaveLocation;
    public bool IsDraggingItem = false;
    [Space(10)]

    [Header("Main Menu Item Area References")]
    public GameObject GoodiesGrid;
    public GameObject WeaponsGrid;
    public GameObject GearGrid;
    public GameObject MaterialsGrid;
    public GameObject KeysGrid;
    public GameObject ItemSelectGlow;
    public Image MainMenuDetailsImage;
    public Text MainMenuDetailsText;
    public GameObject TossItemButton;
    public GameObject TossItemPopUp;
    private ItemBase _SelectedItem;
    [SerializeField]
    private ScrollRect ItemScrollRect;
    [Space(10)]

    //internal variables
    private Queue<IEnumerator> MessageQueue = new Queue<IEnumerator>();
    private static bool CoroutineRunning = false; //tells whether coroutine is running
    private static bool IsPrinting = false; //tells Update when printing is in progress
    private static bool SkipAnimation = false; //tells corouines when user wants to skip
    private GameObject ActiveGrid;

    //automatically disables selected glow when no item is selected
    public ItemBase SelectedItem {
        get
        {
            return _SelectedItem;
        }
        set
        {
            if (ReferenceEquals(value, null)) {
                ItemSelectGlow.SetActive(false);
                MainMenuDetailsImage.sprite = null;
                MainMenuDetailsText.text = "";
            }
            _SelectedItem = value;
        }
    }

    /// <summary>
    /// Resets all the scrollable areas to be back at the top.
    /// </summary>
    public void ResetScrolls()
    {
        foreach (GameObject scroll in ScrollViews) {
            scroll.GetComponent<RectTransform>().localPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// Properly resets menu objects for next time it is opened.
    /// </summary>
    public void CloseMainMenu()
    {
        if (GameManager.Instance.IsState(GameStates.MainMenuState)) {
            UIManager.Instance.ResetScrolls();
            UIManager.Instance.SelectedItem = null;
            UIManager.Instance.ExitPopUp.SetActive(false);
            UIManager.Instance.TossItemPopUp.SetActive(false);
            UIManager.Instance.MenuCanvas.SetActive(false);
            GameManager.Instance.SetState(GameStates.IdleState);
        }
    }

    /// <summary>
    /// Switches the Main Area depending on the category button chosen on the left.
    /// Areas are public, pass in the one you want to switch to.
    /// </summary>
    public void SwitchArea(GameObject new_area)
    {
        //clear current item selection
        UIManager.Instance.SelectedItem = null;

        if (new_area == InventoryArea)
            TossItemButton.SetActive(true);
        else
            TossItemButton.SetActive(false);

        //switch active grid
        ActiveArea.SetActive(false);
        new_area.SetActive(true);
        ActiveArea = new_area;
    }

    /// <summary>
    /// This toggles the Exit Game pop-up. It is used by the Exit button
    /// in the main menu and the pop-up's cancel button.
    /// </summary>
    public void ExitButtonPressed()
    {
        if (!ExitPopUp.activeSelf) {
            ExitPopUp.SetActive(true);
            //LastSaveTime.text = GameManager.Instance.LastSaveTime;
            //LastSaveLocation.text = GameManager.Instance.LastSaveLocation;
        }
        else {
            ExitPopUp.SetActive(false);
        }
    }

    /// <summary>
    /// Switches the item-type grid displaying all items of a certain type.
    /// Grids are public, pass in the one you want to switch to.
    /// </summary>
    public void SwitchItemGrid (GameObject new_grid)
    {
        //clear current item selection
        UIManager.Instance.SelectedItem = null;

        //disable "Toss" button for Key item since they are not discardable
        if (new_grid == KeysGrid) {
            TossItemButton.GetComponent<Button>().image.color = new Color(1, 0, 0, 0.5f);
            TossItemButton.GetComponent<TossItemButton>().IsEnabled = false;
        }
        else if (ActiveGrid == KeysGrid) {
            TossItemButton.GetComponent<Button>().image.color = Color.red;
            TossItemButton.GetComponent<TossItemButton>().IsEnabled = true;
        }
        //switch active grid
        ActiveGrid.SetActive(false);
        new_grid.SetActive(true);
        ActiveGrid = new_grid;
        ItemScrollRect.content = ActiveGrid.GetComponent<RectTransform>();
        ResetScrolls();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        /*
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
        */
        ItemSelectGlow.SetActive(false);
        TossItemPopUp.SetActive(false);
        ActiveGrid = GoodiesGrid;
        WeaponsGrid.SetActive(false);
        GearGrid.SetActive(false);
        MaterialsGrid.SetActive(false);
        KeysGrid.SetActive(false);
        DialogueCanvas.SetActive(false); //dialogue is inactive when game starts
        MenuCanvas.SetActive(false);
        ActiveArea = PartyArea;
        InventoryArea.SetActive(false);
        SynopsisArea.SetActive(false);
        SaveArea.SetActive(false);
        OptionsArea.SetActive(false);
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
    /// This adds a coroutine to the queue which will display a single message.
    /// </summary>
    public void RunDialogue(Interactable caller, string message)
    {

        MessageQueue.Enqueue(PrintString(caller, message)); //start printing the message
    }

    /// <summary>
    /// This adds a coroutine to the queue which will display QandA
    /// It overrides RunDialogue() for a single message
    /// </summary>
    public void RunDialogue(Interactable caller, List<string> message)
    {
        MessageQueue.Enqueue(PrintQandA(caller, message)); //start printing the message
    }

    /// <summary>
    /// Print a message, one character at a time, at a desired speed.
    /// It is skippable and invokes the caller's Next() function.
    /// </summary>
    IEnumerator PrintString(Interactable caller, string message)
    {
        DialogueView.SetActive(true);
        QandAView.SetActive(false);
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
        if (MessageQueue.Count == 0)
            caller.Next(caller);
    }

    /// <summary>
    /// For QandA. It is the same as String() except it works with a List
    /// outputs answers and waits for one to be clicked.
    /// The message List [0] is the question and the rest are answers.
    /// </summary>
    IEnumerator PrintQandA(Interactable caller, List<string> message)
    {
        DialogueView.SetActive(false);
        QandAView.SetActive(true);
        int string_length = message[0].Length; //save length
        int i = 0; //index to get each character from 0 to string_length
        QandATextRef.text = "";

        IsPrinting = true;
        while (i < string_length) //printing loop
        {
            if (SkipAnimation) {
                SkipAnimation = false;
                QandATextRef.text = message[0];
                break;
            }
            QandATextRef.text += message[0][i];
            i++;
            if (i < string_length) //wait before next char
                yield return new WaitForSeconds(SecondsBetweenChar);
        }
        IsPrinting = false;

        //set the text of each answer to the buttons
        for (int j = 1; j < message.Count; j++) {
            AnswerButtons[j - 1].SetActive(true);
            AnswerText[j-1].text = message[j];
        }

        QandAInstructionRef.text = QandAInstruction; //display message to go to next dialogue
        yield return null; //wait a frame to let ButtonDown reset
        while (AnswerSelected < 0) {
            yield return null; //wait for user to finish reading, and push "accept"
        }
        //deactivate all buttons since they may not all be necesary next QandA
        foreach (GameObject buttonGO in AnswerButtons) {
            buttonGO.SetActive(false);
        }
        QandATextRef.text = "";
        QandAInstructionRef.text = "";
        CoroutineRunning = false;
        if (MessageQueue.Count == 0)
            caller.Next(caller, AnswerSelected);
        AnswerSelected = -1;
    }

    /// <summary>
    /// This is to be called by the event of the QandA answer buttons
    /// </summary>
    public void SelectAnswer(int selection)
    {
        AnswerSelected = selection;
    }
}