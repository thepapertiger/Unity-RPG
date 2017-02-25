/**
 * Author: Sander Homan
 * Copyright 2012
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores the actual dialogues. This is the actual asset you see in the project
/// </summary>
public class DialogueFile : ScriptableObject
{
    /// <summary>
    /// A node in the graph; line in the conversation
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        /// <summary>
        /// The identifier for the node
        /// </summary>
        public int id;
        /// <summary>
        /// The dialogue this line belongs to
        /// </summary>
        public string dialogueEntry;
        /// <summary>
        /// The actual line
        /// </summary>
        public string dialogue;

        /// <summary>
        /// A list containing all the nodes this node links to
        /// </summary>
        public List<int> output = new List<int>();


        // editor specific
        /// <summary>
        /// The position of the node in the graph. This is only used in the editor
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// An optional string for the user to fill in
        /// </summary>
        public string userData = "";
        /// <summary>
        /// The speaker that is saying this line
        /// </summary>
        public string speaker = "";
    }

    /// <summary>
    /// Contains the meta data of a dialogue
    /// </summary>
    [System.Serializable]
    public class DialogueEntry
    {
        /// <summary>
        /// The name of the dialogue
        /// </summary>
        public string id;

        /// <summary>
        /// Used in editor only to give every line an unique id
        /// </summary>
        public int maxLineId = 1;

        /// <summary>
        /// All the available speakers in this dialogue
        /// </summary>
        public List<string> speakers = new List<string>();

        /// <summary>
        /// Default constructor. needed for xml serialization
        /// </summary>
        public DialogueEntry()
        {
            id = "Unknown";
        }

        /// <summary>
        /// Constructs a dialogue entry
        /// </summary>
        /// <param name="id">The name of the dialogue</param>
        public DialogueEntry(string id)
        {
            this.id = id;
        }
    }

    /// <summary>
    /// A list of all the entries
    /// </summary>
    [HideInInspector]
    public List<DialogueEntry> entries = new List<DialogueEntry>();
    /// <summary>
    /// A list of all the lines
    /// </summary>
    [HideInInspector]
    public List<DialogueLine> lines = new List<DialogueLine>();
}

