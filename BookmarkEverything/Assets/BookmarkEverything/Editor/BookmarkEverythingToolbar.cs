using System.Collections;
using System.Collections.Generic;
using BookmarkEverything;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using UnityToolbarExtender.Examples;

[InitializeOnLoad]
public class BookmarkEverythingToolbar
{
    static BookmarkEverythingToolbar()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    private static string GetNameForFile(string path)
    {
        if (BookmarkEverythingEditor.CurrentSettings.ShowFullPath)
        {
            return path;
        }

        string[] s = path.Split('/');
        return s[s.Length - 1];
    }


    private static string GetNameForFolder(string path)
    {
        if (BookmarkEverythingEditor.CurrentSettings.ShowFullPathForFolders)
        {
            return path;
        }

        string[] s = path.Split('/');
        return s[s.Length - 1];
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        foreach (var entry in BookmarkEverythingEditor.CurrentSettings.EntryData)
        {
            if (entry.Category == BookmarkEverythingEditor.CATEGORY_STARRED)
            {
                string path = AssetDatabase.GUIDToAssetPath(entry.GUID);
                bool exists = IOHelper.Exists(path);
                GUIContent content;
                if (exists)
                {
                    content = BookmarkEverythingEditor.ContentWithIcon("", path);
                }
                else
                {
                    content = BookmarkEverythingEditor.RetrieveGUIContent("", "console.erroricon.sml");
                }

                if (GUILayout.Button(content, ToolbarStyles.commandButtonStyle))
                {
                    if (exists)
                    {
                        if (BookmarkEverythingEditor.CurrentSettings.PingType == PingTypes.Ping)
                        {
                            if (Selection.activeObject)
                            {
                                Selection.activeObject = null;
                            }

                            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
                        }
                        else if (BookmarkEverythingEditor.CurrentSettings.PingType == PingTypes.Selection)
                        {
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                        }
                        else if (BookmarkEverythingEditor.CurrentSettings.PingType == PingTypes.Both)
                        {
                            if (Selection.activeObject)
                            {
                                Selection.activeObject = null;
                            }

                            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                        }

                    }
                }
            }
        }
    }
}