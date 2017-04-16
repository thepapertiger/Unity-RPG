/**
 * Author: Sander Homan
 * Copyright 2012
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class EditSpeakers : EditorWindow
{
    private DialogueFile file = null;
    private HashSet<string> speakers = new HashSet<string>();

    private Vector2 fileSpeakerListScroll = Vector2.zero;
    private Vector2 entrySpeakerListScroll = Vector2.zero;

    //private string currentEntryId;
    private DialogueFile.DialogueEntry currentEntry = null;
    private HashSet<string> entrySpeakers = new HashSet<string>();

    private string newSpeaker = "";

    private string fileSelected = "";
    private string entrySelected = "";

    public void Init(DialogueFile file, string entry)
    {
        this.file = file;
        //this.currentEntryId = entry;

        // get all the speakers in the file
        speakers.Clear();
        entrySpeakers.Clear();
        foreach (DialogueFile.DialogueEntry e in file.entries)
        {
            if (e.id == entry)
                currentEntry = e;
            foreach (string s in e.speakers)
            {
                speakers.Add(s);
                if (e.id == entry)
                {
                    entrySpeakers.Add(s);
                }
            }
        }

        speakers.ExceptWith(entrySpeakers);
        if (currentEntry.speakers == null)
            currentEntry.speakers = new List<string>();
    }

    void saveSpeakers()
    {
        currentEntry.speakers.Clear();
        foreach (string s in entrySpeakers)
        {
            currentEntry.speakers.Add(s);
        }
        EditorUtility.SetDirty(file);
    }

    void OnGUI()
    {
        // show speakers list
        GUILayout.BeginHorizontal();

        // speakers available in file
        GUILayout.BeginVertical();
        fileSpeakerListScroll = GUILayout.BeginScrollView(fileSpeakerListScroll, (GUIStyle)"box", GUILayout.Width(200));
        foreach (string s in speakers)
        {
            if (fileSelected == s)
                GUILayout.Label(s, (GUIStyle)"boldLabel");
            else
            {
                if (GUILayout.Button(s, (GUIStyle)"label"))
                    fileSelected = s;
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        // buttons
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("->"))
        {
            // move selected speaker to other side
            if (speakers.Contains(fileSelected))
            {
                speakers.Remove(fileSelected);
                entrySpeakers.Add(fileSelected);
                saveSpeakers();
            }
        }
        if (GUILayout.Button("<-"))
        {
            // move selected speaker to other side
            if (entrySpeakers.Contains(entrySelected))
            {
                entrySpeakers.Remove(entrySelected);
                speakers.Add(entrySelected);
                saveSpeakers();
            }
        }
        newSpeaker = GUILayout.TextField(newSpeaker, GUILayout.MinWidth(100));
        if (GUILayout.Button("+"))
        {
            entrySpeakers.Add(newSpeaker);
            newSpeaker = "";
            saveSpeakers();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        // assigned list
        GUILayout.BeginVertical();
        entrySpeakerListScroll = GUILayout.BeginScrollView(entrySpeakerListScroll, (GUIStyle)"box", GUILayout.Width(200));
        foreach (string s in entrySpeakers)
        {
            if (entrySelected == s)
                GUILayout.Label(s, (GUIStyle)"boldLabel");
            else
            {
                if (GUILayout.Button(s, (GUIStyle)"label"))
                {
                    entrySelected = s;
                }
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}

