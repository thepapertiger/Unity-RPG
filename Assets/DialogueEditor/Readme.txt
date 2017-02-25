Dialogue Editor 
 Introduction

Thank you for using the Dialogue Editor for the Unity 3D engine. This editor extension provides an easy way to add complex dialogues to you game or application. This extension includes an interactive tutorial made with this extension.


Usage
 Creating the dialogue

After installing the extension, the window menu has an extra item called DialogueEditor. This opens the dialogue editor window.
 

The top left of the window has a combobox to select already made dialogue files in your project. The '+' button next to it creates a new one in the root of the project. Below the '+' button are the 'import' and 'export' buttons that allow you to import xml dialogue files into the currently selected file and export the currently selected file to an dialogue xml file. Note: The import causes a warning, but this is harmless. The cause of the warning is deserializing a ScriptableObject through the .net serializer functions. Just below that, is a list of dialogues in the currently selected file. Below the list, is a '+' and a '-' button to create and delete dialogues in the current file.

The right side of the window is used to create the dialogue itself. For a new dialogue, the only node available is the 'BEGIN' node. To add a node to an existing graph, press the '+' button next to a selected node. This will create a new node and connect it to the selected node. To delete a node, press the small 'x' on the top right of a node. To make a link to a different node, click the '->' button and click again on the node you want to connect to. To break an incoming link, click the big 'X' on the left of the node and click on the node that is linking to the currently selected node.

In the node, there are 2 items you can set. The first is the speaker, and the second is userdata. The speaker can be selected from the configured speakers list. To configure this list, click the 'Edit Speakers' button in the top right of the window. The userdata field can contain any text you want and is used to provide metadata to the scripts. To actually give the node text, fill in the big textfield in the bottom of the window.

 Using the dialogue in scripts

First you have to load the correct dialogue file. This can be done using DialogueManager.LoadDialogueFile. This returns a manager that contains all the dialogues in the selected file. Next you will need to get the desired dialogue from the manager using manager.GetDialogue. The return value of this method is a Dialogue. Through usage of the Dialogue class, you can have the characters perform their lines. The 2 important functions in the Dialogue class are Dialogue.GetChoices and Dialogue.PickChoice. GetChoices returns the next possible lines. PickChoice selects the given line and continues the dialogue. In a normal dialogue, you will be constantly calling GetChoices and PickChoice. Another handy method is the Dialogue.Start() function. This method returns a dialogue back to the start.

Example from the tutorial script: 
      manager = DialogueManager.LoadDialogueFile(dialogueFile);
      currentDialogue = manager.GetDialogue("TutorialStart");
      currentChoice = currentDialogue.GetChoices()[0];
      currentDialogue.PickChoice(currentChoice);
      if (currentDialogue.GetChoices().Length > 1)
      {
          // sort list
          Dialogue.Choice[] list = currentDialogue.GetChoices();
          System.Array.Sort(list, (o1, o2) => o1.userData.CompareTo(o2.userData));
          GUILayout.BeginVertical();
          foreach (Dialogue.Choice choice in list)
          {
              if (GUILayout.Button(choice.dialogue))
              {
                  currentDialogue.PickChoice(choice);
                  currentChoice = choice;
              }
          }
          GUILayout.EndVertical();
      }
	  

Version Changelog
 V1.1
	Added import and export to xml. This does not require xml at runtime. Note: Import causes a warning, but this warning is harmless and can be ignored.
 V1.0
	Initial release. A fully working dialogue editor capable of easily making complex dialogues.

Author:
Sander Homan (homans.nhlrebel.com)
Copyright:
Sander Homan 2012