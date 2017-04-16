/*!
 * 
 * \mainpage Dialogue Editor
 * 
 * \section intro_sec Introduction
 * Thank you for using the Dialogue Editor for the Unity 3D engine. This editor extension provides an easy way to add complex dialogues to you game or application. This extension includes an interactive tutorial made with this extension.
 *
 * \section usage_sec Usage
 *
 * \subsection step1 Creating the dialogue
 * After installing the extension, the window menu has an extra item called DialogueEditor. This opens the dialogue editor window.
 * 
 * \image html Screen1.png
 * The top left of the window has a combobox to select already made dialogue files in your project. The '+' button next to it creates a new one in the root of the project. Below the '+' button are the 'import' and 'export' buttons that allow you to import xml dialogue files into the currently selected file and export the currently selected file to an dialogue xml file. Note: The import causes a warning, but this is harmless. The cause of the warning is deserializing a ScriptableObject through the .net serializer functions.
 * Just below that, is a list of dialogues in the currently selected file. Below the list, is a '+' and a '-' button to create and delete dialogues in the current file.
 * 
 * The right side of the window is used to create the dialogue itself. For a new dialogue, the only node available is the 'BEGIN' node. To add a node to an existing graph, press the '+' button next to a selected node. This will create a new node and connect it to the selected node. To delete a node, press the small 'x' on the top right of a node.
 * To make a link to a different node, click the '->' button and click again on the node you want to connect to. To break an incoming link, click the big 'X' on the left of the node and click on the node that is linking to the currently selected node.
 * 
 * In the node, there are 2 items you can set. The first is the speaker, and the second is userdata. The speaker can be selected from the configured speakers list. To configure this list, click the 'Edit Speakers' button in the top right of the window. The userdata field can contain any text you want and is used to provide metadata to the scripts.
 * To actually give the node text, fill in the big textfield in the bottom of the window.
 * 
 * \subsection step2 Using the dialogue in scripts
 * 
 * First you have to load the correct dialogue file. This can be done using DialogueManager.LoadDialogueFile. This returns a manager that contains all the dialogues in the selected file.
 * Next you will need to get the desired dialogue from the manager using manager.GetDialogue. The return value of this method is a Dialogue. Through usage of the Dialogue class, you can have the characters perform their lines.
 * The 2 important functions in the Dialogue class are Dialogue.GetChoices and Dialogue.PickChoice. GetChoices returns the next possible lines. PickChoice selects the given line and continues the dialogue. In a normal dialogue, you will be constantly calling GetChoices and PickChoice.
 * Another handy method is the Dialogue.Start() function. This method returns a dialogue back to the start.
 * 
 * Example from the tutorial script:
 * \code
 *      manager = DialogueManager.LoadDialogueFile(dialogueFile);
 *      currentDialogue = manager.GetDialogue("TutorialStart");
 *      currentChoice = currentDialogue.GetChoices()[0];
 *      currentDialogue.PickChoice(currentChoice);
 * \endcode
 * \code
 *      if (currentDialogue.GetChoices().Length > 1)
 *      {
 *          // sort list
 *          Dialogue.Choice[] list = currentDialogue.GetChoices();
 *          System.Array.Sort(list, (o1, o2) => o1.userData.CompareTo(o2.userData));
 *          GUILayout.BeginVertical();
 *          foreach (Dialogue.Choice choice in list)
 *          {
 *              if (GUILayout.Button(choice.dialogue))
 *              {
 *                  currentDialogue.PickChoice(choice);
 *                  currentChoice = choice;
 *              }
 *          }
 *          GUILayout.EndVertical();
 *      }
 * \endcode
 *
 * \section versions Version Changelog
 * 
 * \subsection v11 V1.1
 * 
 * Added import and export to xml. This does not require xml at runtime.
 * Note: Import causes a warning, but this warning is harmless and can be ignored.
 * 
 * \subsection v10 V1.0
 * 
 * Initial release. A fully working dialogue editor capable of easily making complex dialogues.
 * 
 * \author Sander Homan
 * \copyright Sander Homan 2012
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains all the lines and userdata configured for a specific dialogue
/// </summary>
public class Dialogue
{
    private Dictionary<int, DialogueFile.DialogueLine> lines = new Dictionary<int, DialogueFile.DialogueLine>();

    private int currentIndex = 0;

    /// <summary>
    /// The choice class contains all the data of a node in the dialogue
    /// </summary>
    public class Choice
    {
        /// <summary>
        /// Used internally. The identifier of a choice
        /// </summary>
        public int id;
        /// <summary>
        /// The dialogue line
        /// </summary>
        public string dialogue;
        /// <summary>
        /// Which speaker is saying this line
        /// </summary>
        public string speaker;
        /// <summary>
        /// UserData defined in the editor
        /// </summary>
        public string userData;
    }

    /// <summary>
    /// Used internally. Adds a line to the dialogue.
    /// </summary>
    /// <param name="line">The line to be added</param>
    public void AddLine(DialogueFile.DialogueLine line)
    {
        lines.Add(line.id, line);
    }

    /// <summary>
    /// Resets the dialogue to the start
    /// </summary>
    public void Start()
    {
        // set up the start of the dialogue
        currentIndex = 0;
    }

    /// <summary>
    /// Gets all the possible choices for the current dialogue node
    /// </summary>
    /// <returns>An array of type Choice containing all the possible choices</returns>
    public Choice[] GetChoices()
    {
        List<Choice> choices = new List<Choice>();

        foreach (int id in lines[currentIndex].output)
        {
            Choice c = new Choice();
            c.id = id;
            c.dialogue = lines[id].dialogue;
            c.speaker = lines[id].speaker;
            c.userData = lines[id].userData;
            choices.Add(c);
        }

        return choices.ToArray();
    }

    /// <summary>
    /// Advanced the dialogue with the given choice. Could also be used to jump to a different position in the dialogue, but this is not recommended.
    /// </summary>
    /// <param name="c"></param>
    public void PickChoice(Choice c)
    {
        currentIndex = c.id;
    }
}

/// <summary>
/// The DialogueManager is the main class to interface with the stored dialogues.
/// <example>
/// How to get a dialogue from script
/// <code>
/// DialogueManager manager = DialogueManager.LoadDialogueFile(dialogueFile);
/// Dialogue dialogue = manager.GetDialogue("SampleDialogue");
/// </code>
/// </example>
/// </summary>
public class DialogueManager
{
    private static Dictionary<DialogueFile, DialogueManager> managers = new Dictionary<DialogueFile, DialogueManager>();

    /// <summary>
    /// Load a dialogue file from the resources folder.
    /// </summary>
    /// <param name="resourcePath">The path in the Resource folder</param>
    /// <returns>A DialogueManager holding the file reference</returns>
    public static DialogueManager LoadDialogueFile(string resourcePath)
    {
        return LoadDialogueFile(Resources.Load(resourcePath) as DialogueFile);
    }

    /// <summary>
    /// Load a dialogue file by the given DialogueFile reference
    /// </summary>
    /// <param name="file">A reference to a DialogueFile</param>
    /// <returns>A DialogueManager holding the file reference</returns>
    public static DialogueManager LoadDialogueFile(DialogueFile file)
    {
        
        if (managers.ContainsKey(file))
            return managers[file];

        // load file, optimize for searching dialogues
        DialogueManager manager = new DialogueManager();
        managers.Add(file, manager);

        manager.file = file;

        return manager;
    }

    private DialogueManager()
    {
    }

    private DialogueFile file;

    /// <summary>
    /// Retreives a specific dialogue from the manager
    /// </summary>
    /// <param name="dialogueName">The name of the dialogue</param>
    /// <returns>A Dialogue containing all the lines</returns>
    public Dialogue GetDialogue(string dialogueName)
    {
        // create the dialogue and return it
        Dialogue result = new Dialogue();

        // get the lines
        foreach (DialogueFile.DialogueLine line in file.lines)
        {
            if (line.dialogueEntry == dialogueName)
                result.AddLine(line);
        }

        return result;
    }
}

