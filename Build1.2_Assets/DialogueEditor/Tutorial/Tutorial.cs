/**
 * Author: Sander Homan
 * Copyright 2012
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Tutorial : MonoBehaviour
{
    public DialogueFile dialogueFile = null;

    private DialogueManager manager;
    private Dialogue currentDialogue;
    private Dialogue.Choice currentChoice = null;

    public List<Texture2D> images = new List<Texture2D>();

    void Start()
    {
        manager = DialogueManager.LoadDialogueFile(dialogueFile);
        currentDialogue = manager.GetDialogue("TutorialStart");
        currentChoice = currentDialogue.GetChoices()[0];
        currentDialogue.PickChoice(currentChoice);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 680, 560));
        GUILayout.Label(currentChoice.dialogue);
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
        else if (currentDialogue.GetChoices().Length == 1)
        {
            if (GUILayout.Button("Next"))
            {
                currentChoice = currentDialogue.GetChoices()[0];
                currentDialogue.PickChoice(currentChoice);
            }

            // check if we need to display an image
            if (currentChoice.userData.IndexOf("image:") == 0)
            {
                Debug.Log(currentChoice.userData.Substring(6));
                int imageIndex = int.Parse(currentChoice.userData.Substring(6));
                Texture2D tex = images[imageIndex];
                GUI.DrawTexture(new Rect(0, 100, tex.width, tex.height), tex);
            }
        }
        else
        {
            // end of tutorial
        }

        GUILayout.EndArea();
    }
}

