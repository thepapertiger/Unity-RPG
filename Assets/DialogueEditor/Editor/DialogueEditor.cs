using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class DialogueEditor : EditorWindow
{

	public Texture2D linetexture = null;

	[System.Serializable]
	public class DialogueWindow
	{
		[SerializeField]
		public Rect rect;
		[SerializeField]
		public DialogueFile.DialogueLine line;
		[SerializeField]
		public bool startConversation = false;

		public DialogueWindow(Rect rc, DialogueFile.DialogueLine line = null, bool startConversation = false)
		{
			rect = rc;
			this.line = line;
			this.startConversation = startConversation;
		}
	}

	Vector2 dialogueListScroll = Vector2.zero;
	Vector2 dialogueTreeScroll = Vector2.zero;

	[SerializeField]
	private List<DialogueWindow> windows = new List<DialogueWindow>();

	[SerializeField]
	private List<DialogueFile> files = new List<DialogueFile>();
	[SerializeField]
	private GUIContent[] filePopupList = null;
	//private int[] filePopupValues = null;
	[SerializeField]
	private int filePopupSelectedIndex = -1;

	[SerializeField]
	private DialogueFile.DialogueEntry selectedEntry = null;

	[SerializeField]
	private int lastFocusWindow = -1;
	[SerializeField]
	private int createdWindow = -1;

	private bool linkDragging = false;
	private bool breakLink = false;
	private DialogueWindow linkDragStartWindow = null;

	private Vector2 mousePos = Vector2.zero;

	private GUIStyle windowButtonStyle;

	[MenuItem("Window/DialogueEditor")]
	public static void InitWindow()
	{
		DialogueEditor editor = GetWindow<DialogueEditor>();
		editor.linetexture = AssetDatabase.LoadAssetAtPath("Assets/Dialogue/Editor/Line.png", typeof(Texture2D)) as Texture2D;
		editor.files.Clear();
		// get dialogue from scene
		DialogueFile[] files = FindObjectsOfTypeIncludingAssets(typeof(DialogueFile)) as DialogueFile[];
		if (files != null)
		{
			foreach (DialogueFile f in files)
			{
				editor.files.Add(f);
			}
		}

		editor.Init();
	}

	public void Init()
	{
		// do stuff
		// build popup list
		buildFilePopupArray();

		// create a new guistyle for the window buttons
		windowButtonStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
		windowButtonStyle.padding = new RectOffset(0, 0, 0, 0);
		windowButtonStyle.contentOffset = new Vector2(-2, 0);
		windowButtonStyle.margin = new RectOffset(0, 0, 0, 0);
		windowButtonStyle.alignment = TextAnchor.MiddleCenter;
	}

	public void buildFilePopupArray()
	{
		//List<GUIContent> filePopupList = new List<GUIContent>();
		//foreach (DialogueFile file in files)
		//{
		//    if (AssetDatabase.GetAssetPath(file)!="")
		//        filePopupList.Add(new GUIContent(AssetDatabase.GetAssetPath(file).Substring(7)));
		//}
		files.RemoveAll((item) => AssetDatabase.GetAssetPath(item) == "");

		this.filePopupList = new GUIContent[files.Count];
		// this.filePopupValues = new int[files.Count];

		for (int i = 0; i < files.Count; i++)
		{
			this.filePopupList[i] = new GUIContent(AssetDatabase.GetAssetPath(files[i]).Substring(7));
			//this.filePopupValues[i] = i;
		}
	}

	Rect getBoundingRect(Rect minimumRect)
	{
		Rect result = minimumRect;

		// iterate over the rects
		foreach (DialogueWindow window in windows)
		{
			if (window.rect.x < result.x) result.x = window.rect.x;
			if (window.rect.y < result.y) result.y = window.rect.y;
			if (window.rect.xMax > result.xMax) result.xMax = window.rect.xMax;
			if (window.rect.yMax > result.yMax) result.yMax = window.rect.yMax;
		}

		return result;
	}

	void createWindows()
	{
		lastFocusWindow = -1;
		windows.Clear();
		//windows.Add(new DialogueWindow(new Rect(10,10,100,50), null, true));

		foreach (DialogueFile.DialogueLine line in files[filePopupSelectedIndex].lines)
		{
			if (line.dialogueEntry == selectedEntry.id)
			{
				windows.Add(new DialogueWindow(new Rect(line.position.x, line.position.y, 100, 65), line, false));
			}
		}
	}

	int addWindow(DialogueFile.DialogueLine line)
	{
		windows.Add(new DialogueWindow(new Rect(line.position.x, line.position.y, 100, 65), line, false));
		return windows.Count - 1;
	}

	//public int FocusedWindow
	//{
	//    get
	//    {
	//        System.Reflection.FieldInfo field = typeof(GUI).GetField("focusedWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

	//        return (int)field.GetValue(null);
	//    }
	//}

	public int FocusedWindow { get; set; }

	string createUniqueDialogueName()
	{
		DialogueFile file = files[filePopupSelectedIndex];
		int index = 0;
		string baseName = "newDialogue";
		string currentCheck = baseName;
		bool found = true;
		while (found)
		{
			found = false;
			foreach (DialogueFile.DialogueEntry entry in file.entries)
			{
				if (entry.id.IndexOf(currentCheck) == 0)
				{
					index++;
					currentCheck = baseName + index;
					found = true;
					break;
				}
			}
		}
		return currentCheck;
	}

	void saveUndo(string name)
	{
		if (filePopupSelectedIndex >= 0)
		{
			Object[] saveObjects = new Object[2];
			saveObjects[0] = this;
			saveObjects[1] = files[filePopupSelectedIndex];
			Undo.RegisterUndo(saveObjects, name);
		}
	}

	void OnGUI()
	{
		TextAnchor oldAlignment = GUI.skin.window.alignment;
		GUI.skin.window.alignment = TextAnchor.UpperLeft;

		// check if windows need to be recreated
		if (Event.current.type == EventType.Repaint && filePopupSelectedIndex >= 0 && selectedEntry != null)
			createWindows();

		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical(GUILayout.Width(200));
		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
		int t = filePopupSelectedIndex;
		filePopupSelectedIndex = EditorGUILayout.Popup(filePopupSelectedIndex, filePopupList);
		if (t != filePopupSelectedIndex) { } // the selected index has changed, load the correct dialogue file

		// create new dialogue file
		if (GUILayout.Button("+"))
		{
			saveUndo("creating dialogue file");
			// create new dialogueFile
			DialogueFile file = ScriptableObject.CreateInstance<DialogueFile>();
			AssetDatabase.CreateAsset(file, AssetDatabase.GenerateUniqueAssetPath("Assets/Script.asset"));
			AssetDatabase.SaveAssets();
			files.Add(file);
			buildFilePopupArray();
		}
		GUILayout.EndHorizontal();

		// import export buttons
		GUILayout.BeginHorizontal();
		if (filePopupSelectedIndex < 0)
			GUI.enabled = false;
		if (GUILayout.Button("Import"))
		{
			string filename = EditorUtility.OpenFilePanel("Import dialogue", ".", "xml");
			DialogueFile importedFile = deserializeFromXML(filename);
			// import the various entries and lines
			// check if the existing file already has the same entries
			bool foundSameEntryName = false;
			string sameEntryName = "";
			foreach (DialogueFile.DialogueEntry entry in importedFile.entries)
			{
				foreach (DialogueFile.DialogueEntry oldEntry in files[filePopupSelectedIndex].entries)
				{
					if (entry.id == oldEntry.id)
					{
						// found same entry
						foundSameEntryName = true;
						sameEntryName = entry.id;
						break;
					}
				}
			}
			bool continueImport = true;
			if (foundSameEntryName)
			{
				continueImport = EditorUtility.DisplayDialog("Same dialogue entry found", "The existing dialogue already has an entry named " + sameEntryName + ". If you continue, existing dialogues will be overwritten. Do you want to continue?", "Continue", "Cancel");
			}

			if (continueImport)
			{
				saveUndo("Importing dialogue");
				// add the entries and lines
				foreach (DialogueFile.DialogueEntry entry in importedFile.entries)
				{
					// check if it already exists
					bool dialogueExists = false;
					foreach (DialogueFile.DialogueEntry oldEntry in files[filePopupSelectedIndex].entries)
					{
						if (entry.id == oldEntry.id)
						{
							dialogueExists = true;
							// clear out existing lines
							files[filePopupSelectedIndex].lines.RemoveAll((item) => item.dialogueEntry == entry.id);
						}
					}
					if (!dialogueExists)
						files[filePopupSelectedIndex].entries.Add(entry); // add entry if it does not exists
				}

				// add the lines in one go
				files[filePopupSelectedIndex].lines.AddRange(importedFile.lines);
				EditorUtility.SetDirty(files[filePopupSelectedIndex]);
			}
		}
		if (GUILayout.Button("Export"))
		{
			// test serialization
			string filename = EditorUtility.SaveFilePanelInProject("Export dialogue", "dialogue", "xml", "");
			serializeToXML(filename, files[filePopupSelectedIndex]);
			AssetDatabase.Refresh();
		}
		if (filePopupSelectedIndex < 0)
			GUI.enabled = true;
		GUILayout.EndHorizontal();
		// draw the dialogues
		dialogueListScroll = GUILayout.BeginScrollView(dialogueListScroll, (GUIStyle)"box");

		if (filePopupSelectedIndex >= 0)
		{
			foreach (DialogueFile.DialogueEntry entry in files[filePopupSelectedIndex].entries)
			{
				if (entry == selectedEntry)
				{
					GUILayout.Label(entry.id, (GUIStyle)"boldLabel");
				}
				else
				{
					if (GUILayout.Button(entry.id, (GUIStyle)"label"))
					{
						selectedEntry = entry;
						// build windows
						createWindows();
					}
				}
			}
		}

		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();
		if (filePopupSelectedIndex < 0) GUI.enabled = false;
		if (GUILayout.Button("+"))
		{
			saveUndo("creating dialogue");
			// create new dialogue entry
			DialogueFile.DialogueEntry newEntry = new DialogueFile.DialogueEntry(createUniqueDialogueName());
			DialogueFile.DialogueLine beginLine = new DialogueFile.DialogueLine();
			beginLine.dialogueEntry = newEntry.id;
			beginLine.id = 0;
			beginLine.position = new Vector2(10, 10);
			files[filePopupSelectedIndex].entries.Add(newEntry);
			files[filePopupSelectedIndex].lines.Add(beginLine);
			EditorUtility.SetDirty(files[filePopupSelectedIndex]);
		}
		if (GUILayout.Button("-"))
		{
			saveUndo("deleting dialogue");
			// delete all the lines corresponding to this entry
			files[filePopupSelectedIndex].lines.RemoveAll((item) => item.dialogueEntry == selectedEntry.id);
			// delete the entry
			files[filePopupSelectedIndex].entries.Remove(selectedEntry);
			selectedEntry = null;
			windows.Clear();
			EditorUtility.SetDirty(files[filePopupSelectedIndex]);

		}
		if (filePopupSelectedIndex < 0) GUI.enabled = true;
		GUILayout.EndHorizontal();

		GUILayout.Space(5);
		GUILayout.EndVertical();

		GUILayout.BeginVertical();

		// show selected dialogueinfo
		GUILayout.BeginHorizontal();
		// display name
		if (selectedEntry != null) GUILayout.Label("Name: " + selectedEntry.id);
		else GUILayout.Label("Name: ");
		GUILayout.FlexibleSpace();

		if (selectedEntry == null) GUI.enabled = false;
		// rename button
		if (GUILayout.Button("Rename"))
		{
			// open rename dialog
			RenameDialogue dialog = GetWindow<RenameDialogue>(true);
			dialog.oldName = dialog.newName = selectedEntry.id;
			dialog.file = files[filePopupSelectedIndex];
		}
		if (GUILayout.Button("Edit Speakers"))
		{
			// open rename dialog
			EditSpeakers dialog = GetWindow<EditSpeakers>(true);
			dialog.Init(files[filePopupSelectedIndex], selectedEntry.id);
		}
		if (selectedEntry == null) GUI.enabled = true;
		GUILayout.EndHorizontal();

		// get rect for scrollview
		GUIStyle style = new GUIStyle();
		style.stretchHeight = true;
		style.stretchWidth = true;
		style.margin = new RectOffset(4, 4, 4, 4);
		Rect rc = GUILayoutUtility.GetRect(new GUIContent(), style);
		GUI.Box(rc, "");
		Rect bounds = getBoundingRect(new Rect(25, 25, rc.width - 50, rc.height - 50));
		bounds.x -= 25; bounds.y -= 25; bounds.width += 50; bounds.height += 50;
		dialogueTreeScroll = GUI.BeginScrollView(rc, dialogueTreeScroll, bounds);

		BeginWindows();
		int index = 0;
		bool createWindow = false;
		bool createLink = false;
		bool breakLink = false;
		bool deleteWindow = false;
		foreach (DialogueWindow window in windows)
		{
			string title = "";
			if (window.line.id == 0)
			{
				title = "BEGIN";
			}
			else
			{
				if (window.line != null) title = window.line.dialogue;
			}

			// draw output lines
			if (window.line != null)
			{
				foreach (int outputId in window.line.output)
				{
					// find connecting line
					foreach (DialogueFile.DialogueLine outputLine in files[filePopupSelectedIndex].lines)
					{
						if (outputLine.dialogueEntry == selectedEntry.id && outputLine.id == outputId)
						{
							Color color = Color.black;
							if (lastFocusWindow >= 0 && windows.Count > lastFocusWindow && windows[lastFocusWindow].line.id == outputId)
								color = Color.green;
							else if (lastFocusWindow >= 0 && windows.Count > lastFocusWindow && windows[lastFocusWindow].line == window.line)
								color = Color.red;

							Vector2 startPos = window.line.position + new Vector2(100, 25);
							Vector2 endPos = outputLine.position + new Vector2(0, 25);
							Vector2 midPos = (endPos - startPos).normalized;

							// found correct line
							float diffY = Mathf.Abs(startPos.y - endPos.y) / 4;
							float diffX = Mathf.Abs(startPos.x - endPos.x) / 4;
							Vector2 tPos = new Vector2(startPos.x + Mathf.Min(25, diffX), (startPos.y < endPos.y) ? startPos.y + Mathf.Min(25, diffY) : startPos.y - Mathf.Min(25, diffY));
							Vector2 tPos2 = new Vector2(endPos.x - Mathf.Min(25, diffX), (startPos.y < endPos.y) ? endPos.y - Mathf.Min(25, diffY) : endPos.y + Mathf.Min(25, diffY));
							Handles.DrawBezier(startPos, tPos, startPos + new Vector2(10, 0), tPos + midPos * -20, color, linetexture, 2);

							Handles.DrawBezier(tPos, tPos2, tPos + midPos * 20, tPos2 + midPos * -20, color, linetexture, 2);

							Handles.DrawBezier(tPos2, endPos, tPos2 + midPos * 20, endPos + new Vector2(-10, 0), color, linetexture, 2);
							break;
						}
					}
				}
			}

			window.rect = GUI.Window(index, window.rect, doWindow, title);

			if (window.line != null)
				window.line.position = new Vector2(window.rect.x, window.rect.y);
			//if (FocusedWindow < 0) GUI.FocusWindow(lastFocusWindow);
			GUI.FocusWindow(FocusedWindow);
			if (index == FocusedWindow)
			{
				// show extra buttons
				if (GUI.Button(new Rect(window.rect.xMax, window.rect.y + 10, 20, 20), "+", windowButtonStyle))
				{
					createWindow = true;
				}
				if (GUI.Button(new Rect(window.rect.xMax, window.rect.y + 30, 20, 20), "->", windowButtonStyle))
				{
					createLink = true;
				}
				if (GUI.Button(new Rect(window.rect.x - 20, window.rect.y + 10, 20, 20), "X", windowButtonStyle))
				{
					breakLink = true;
				}
				if (index != 0)
				{
					if (GUI.Button(new Rect(window.rect.x + 80, window.rect.y - 15, 15, 15), "x", windowButtonStyle))
					{
						deleteWindow = true;
					}
				}
			}
			lastFocusWindow = FocusedWindow;
			index++;
		}

		if (createdWindow >= 0)
		{
			GUI.BringWindowToFront(createdWindow);
			createdWindow = -1;
		}

		if (createWindow)
		{
			saveUndo("creating node");
			// create new line and window
			DialogueFile.DialogueLine line = new DialogueFile.DialogueLine();
			line.dialogue = "";
			line.dialogueEntry = selectedEntry.id;
			line.id = selectedEntry.maxLineId++;

			// set line's position
			Rect windowRc = windows[lastFocusWindow].rect;
			line.position = new Vector2(windowRc.xMax + 50, windowRc.y);

			files[filePopupSelectedIndex].lines.Add(line);

			if (windows[lastFocusWindow].line != null) windows[lastFocusWindow].line.output.Add(line.id);
			int w = addWindow(line);
			GUI.FocusWindow(w);
			createdWindow = w;

			EditorUtility.SetDirty(files[filePopupSelectedIndex]);
		}

		if (createLink)
		{
			linkDragging = true;
			linkDragStartWindow = windows[lastFocusWindow];
			wantsMouseMove = true;
		}
		if (breakLink)
		{
			linkDragging = true;
			this.breakLink = true;
			linkDragStartWindow = windows[lastFocusWindow];
			wantsMouseMove = true;
		}

		if (linkDragging)
		{
			if (Event.current.type == EventType.MouseMove)
			{
				mousePos = Event.current.mousePosition;
				Repaint();
			}

			if (this.breakLink)
			{
				// draw bezier curve from start window to cursor
				Vector2 pos1 = linkDragStartWindow.line.position + new Vector2(0, 25);
				Vector2 pos2 = mousePos;
				Handles.DrawBezier(pos1, pos2, pos1, pos2, Color.red, linetexture, 2);
			}
			else
			{
				// draw bezier curve from start window to cursor
				Vector2 pos1 = linkDragStartWindow.line.position + new Vector2(100, 25);
				Vector2 pos2 = mousePos;
				Handles.DrawBezier(pos1, pos2, pos1, pos2, Color.green, linetexture, 2);
			}
		}

		if (deleteWindow)
		{
			saveUndo("deleting node");
			// find lines that link to this one
			foreach (DialogueFile.DialogueLine line in files[filePopupSelectedIndex].lines)
			{
				if (line.dialogueEntry == windows[lastFocusWindow].line.dialogueEntry)
				{
					if (line.output.Contains(windows[lastFocusWindow].line.id))
						line.output.Remove(windows[lastFocusWindow].line.id);
				}
			}
			// remove currently selected window
			files[filePopupSelectedIndex].lines.Remove(windows[lastFocusWindow].line);
			windows.RemoveAt(lastFocusWindow);
			GUI.FocusWindow(0);
			lastFocusWindow = 0;
			EditorUtility.SetDirty(files[filePopupSelectedIndex]);
		}

		EndWindows();

		// dialogue line text area
		GUI.EndScrollView();
		if (lastFocusWindow > 0 && windows.Count > lastFocusWindow)
			windows[lastFocusWindow].line.dialogue = GUILayout.TextArea(windows[lastFocusWindow].line.dialogue, GUILayout.Height(GUI.skin.textArea.lineHeight * 3.1f));
		else
			GUILayout.TextArea("", GUILayout.Height(GUI.skin.textArea.lineHeight * 3.1f));

		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
		GUI.skin.window.alignment = oldAlignment;
	}

	void doWindow(int id)
	{
		if (selectedEntry == null || windows.Count <= id) return; // invalid window

		if (Event.current.type == EventType.mouseDown)
			FocusedWindow = id;

		if (linkDragging && Event.current.type == EventType.mouseDown)
		{
			//Debug.Log("Create Link");
			// find window the cursor is above
			foreach (DialogueEditor.DialogueWindow window in windows)
			{
				if (window.rect.Contains(mousePos))
				{
					// found window
					if (breakLink)
					{
						saveUndo("breaking link");
						window.line.output.Remove(linkDragStartWindow.line.id);
						breakLink = false;
					}
					else
					{
						saveUndo("creating link");
						linkDragStartWindow.line.output.Add(window.line.id);
					}
					linkDragging = false;
					wantsMouseMove = false;
					break;
				}
			}
		}

		if (id != 0) GUI.DragWindow(new Rect(0, 0, 100, 15));

		// show popup with users
		List<GUIContent> items = new List<GUIContent>();
		int index = 0;
		int selectedIndex = -1;

		foreach (string s in selectedEntry.speakers)
		{
			items.Add(new GUIContent(s));
			if (windows[id].line.speaker == s)
				selectedIndex = index;
			index++;
		}
		int newIndex = EditorGUILayout.Popup(selectedIndex, items.ToArray());
		if (newIndex != selectedIndex)
		{
			windows[id].line.speaker = items[newIndex].text;
			EditorUtility.SetDirty(files[filePopupSelectedIndex]);
		}

		if (windows[id].line.userData == null)
			windows[id].line.userData = "";

		windows[id].line.userData = GUILayout.TextField(windows[id].line.userData);
		if (GUI.changed)
			EditorUtility.SetDirty(files[filePopupSelectedIndex]);
	}

	void serializeToXML(string filename, DialogueFile dialogue)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(DialogueFile));
		TextWriter textWriter = new StreamWriter(filename);
		serializer.Serialize(textWriter, dialogue);
		textWriter.Close();
	}

	DialogueFile deserializeFromXML(string filename)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(DialogueFile));
		TextReader textReader = new StreamReader(filename);
		DialogueFile result = serializer.Deserialize(textReader) as DialogueFile;
		textReader.Close();
		return result;
	}
}
