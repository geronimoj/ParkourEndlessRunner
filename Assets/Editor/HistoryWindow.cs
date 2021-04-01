using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class HistoryWindow : EditorWindow
{
    List<Object> history = new List<Object>();

    int maxItems = 30;
    bool showTypes = false;
    bool showAll = false;

    [MenuItem("Window/History")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(HistoryWindow));
    }

    void Awake()
    {
        titleContent = new GUIContent("History", "History of recent selections");
    }

    int index;

    void OnGUI()
    {
        if (history.Count == 0)
        {
            ReadHistory();
        }

        int numItems = (int)EditorGUILayout.Slider("History Size", maxItems, 5, 100, null);
        if (numItems != maxItems)
        {
            maxItems = numItems;
            while (maxItems < history.Count)
                history.RemoveAt(0);
        }
        showTypes = GUILayout.Toggle(showTypes, "Show Object Types");
        showAll = GUILayout.Toggle(showAll, "Show Full History");
        
        // remove any entries from the list that have been deleted
        history.RemoveAll(obj => { return obj == null; });

        string[] options = new string[history.Count];
        int i = 0;
        foreach (Object obj in history)
        {
            // get a trimmed typename
            string typeName = "";
            if (showTypes)
            {
                typeName = obj.GetType().ToString();
                typeName = typeName.Replace("UnityEngine.", "");
                typeName = typeName.Replace("UnityEditor.", "");
                typeName = "(" + typeName + ")";
            }

            // button for selecting objects if showAll is true
            if (showAll)
            {
                if (GUILayout.Button(obj.name + typeName))
                    Selection.activeObject = obj;
            }
            options[i] = obj.name + typeName;
            i++;
        }

        // droplist option
        if (!showAll)
        {
            Rect pos = new Rect(0, 60, 300, 24);
            int newIndex = EditorGUI.Popup(pos, index, options);
            if (newIndex != index)
            {
                index = history.Count-1;
                Selection.activeObject = history[newIndex];
            }
        }
    }

    void OnSelectionChange()
    {
        bool changed = false;

        // when we select something new, add it to our list if it isn't already in there
        Object[] objs = UnityEditor.Selection.objects;
        foreach (Object obj in objs)
        {
            if (obj && obj.GetType() != typeof(DefaultAsset))
            {
                if (history.Contains(obj))
                {
                    history.Remove(obj);
                }

                // keep the list to max number of items items
                if (history.Count >= maxItems)
                    history.RemoveAt(0);

                history.Add(obj);
                changed = true;
            }
        }

        if (changed)
        {
            // save history
            SaveHistory();

            // redraw the buttons
            Repaint();
        }
    }

    void SaveHistory()
    {
        StreamWriter writer = new StreamWriter("History.txt");
        if (writer != null)
        {
            // save the options
            writer.WriteLine(maxItems.ToString());
            writer.WriteLine(showTypes.ToString());
            writer.WriteLine(showAll.ToString());
            // save every asset path
            foreach (Object o in history)
                writer.WriteLine(AssetDatabase.GetAssetPath(o));
            writer.Close();
        }
    }

    void ReadHistory()
    {
        StreamReader reader = new StreamReader("History.txt");
        if (reader != null)
        {
            // read in the options
            int.TryParse(reader.ReadLine(), out maxItems);
            bool.TryParse(reader.ReadLine(), out showTypes);
            bool.TryParse(reader.ReadLine(), out showAll);
            // read all lines into the history
            while (!reader.EndOfStream)
            {
                string path = reader.ReadLine();
                {
                    Object o = AssetDatabase.LoadMainAssetAtPath(path);
                    if (o != null)
                        history.Add(o);
                }
            }
            reader.Close();
        }
    }
}

