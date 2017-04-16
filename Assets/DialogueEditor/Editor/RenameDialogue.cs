/**
 * Author: Sander Homan
 * Copyright 2012
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenameDialogue : EditorWindow
{
    public string newName = "";
    public DialogueFile file;
    public string oldName = "";

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("New name");
        newName = GUILayout.TextField(newName);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Rename"))
        {
            // TODO rename it
            if (newName == oldName)
            {
                Close();
                return;
            }

            DialogueFile.DialogueEntry currentEntry = null;
            // check if new name already exists
            foreach (DialogueFile.DialogueEntry entry in file.entries)
            {
                if (entry.id == newName)
                {
                    EditorUtility.DisplayDialog("Error", "Dialogue with same name exists", "Ok");
                    return;
                }
                else if (entry.id == oldName)
                    currentEntry = entry;
            }

            currentEntry.id = newName;

            foreach (DialogueFile.DialogueLine line in file.lines)
            {
                if (line.dialogueEntry == oldName)
                    line.dialogueEntry = newName;
            }

            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }

        GUILayout.EndHorizontal();
    }
}

