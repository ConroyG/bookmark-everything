using System.Collections;
using System.Collections.Generic;
using BookmarkEverything;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

static class ToolbarStyles
{
    public static readonly GUIStyle commandButtonStyle;

    static ToolbarStyles()
    {
        commandButtonStyle = new GUIStyle("Command")
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            fontStyle = FontStyle.Bold
        };
    }
}

[InitializeOnLoad]
public class BookmarkEverythingToolbar
{
    private static BookmarkEverythingEditor.SaveData _currentSettings;

    static BookmarkEverythingToolbar()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI); 
        ToolbarExtender.RightToolbarGUI.Add(RightRefresh);
    }

    private static void RightRefresh()
    {
        GUILayout.FlexibleSpace();
        GUIContent c = new GUIContent(EditorGUIUtility.IconContent("Refresh"));

        if (GUILayout.Button(c, ToolbarStyles.commandButtonStyle))
        {
            _currentSettings =
                IOHelper.ReadFromDisk<BookmarkEverythingEditor.SaveData>(BookmarkEverythingEditor.SETTINGS_FILENAME);
        }
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if (_currentSettings == null)
        {
            _currentSettings =
                IOHelper.ReadFromDisk<BookmarkEverythingEditor.SaveData>(BookmarkEverythingEditor.SETTINGS_FILENAME);
        }

        foreach (var entry in _currentSettings.EntryData)
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
                        if (_currentSettings.PingType == PingTypes.Ping)
                        {
                            if (Selection.activeObject)
                            {
                                Selection.activeObject = null;
                            }

                            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
                        }
                        else if (_currentSettings.PingType == PingTypes.Selection)
                        {
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                        }
                        else if (_currentSettings.PingType == PingTypes.Both)
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