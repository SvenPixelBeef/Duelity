#if UNITY_EDITOR

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Reflection;
using UnityEditorInternal;
using Object = UnityEngine.Object;

public static class EditorHelper
{
    /// <summary>
    /// Used for finding an asset by type and returning the first instance found.
    /// Searches the entire project folder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadFirstAssetOfType<T>() where T : class
    {
        foreach (string guid in AssetDatabase.FindAssets(null, null))
        {
            if (AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) is T objectToLoad)
                return objectToLoad;
        }
        return null;
    }

    /// <summary>
    /// Used for finding assets by type and returning a collection of the specified type.
    /// Searches the entire project folder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> LoadAssetsOfType<T>() where T : class
    {
        Type type = typeof(T);
        string[] guids = AssetDatabase.FindAssets("t:" + type.FullName, null);
        int length = guids.Length;

        for (int i = 0; i < length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            T objectToLoad = AssetDatabase.LoadAssetAtPath(path, type) as T;
            yield return objectToLoad;
        }
    }

    public static IEnumerable<Object> LoadAssetsInFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("", new string[] { folderPath });
        int length = guids.Length;

        for (int i = 0; i < length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Object objectToLoad = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            yield return objectToLoad;
        }
    }

    public static IEnumerable<T> LoadOtherAssetsOfSameType<T>(this T t) where T : Object
    {
        Type type = t.GetType();
        string[] guids = AssetDatabase.FindAssets("t:" + type.FullName, null);
        int length = guids.Length;

        for (int i = 0; i < length; i++)
        {
            if (guids[i] == t.GetAssetGuid())
                continue;
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            T objectToLoad = AssetDatabase.LoadAssetAtPath(path, type) as T;
            yield return objectToLoad;
        }
    }


    /// <summary>
    /// Used for finding an asset by type and in the given folder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadFirstAssetOfType<T>(params string[] folderNames) where T : class
    {
        foreach (string guid in AssetDatabase.FindAssets(null, folderNames))
        {
            if (AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) is T objectToLoad)
                return objectToLoad;
        }
        return null;
    }

    /// <summary>
    /// Used for finding assets by folder and returning a collection of the specified type.
    /// Ignores assets whose type does not match the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="folderNames"></param>
    /// <returns></returns>
    public static IEnumerable<T> LoadAssetsOfType<T>(params string[] folderNames)
    {
        foreach (string guid in AssetDatabase.FindAssets(null, folderNames))
        {
            if (AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) is T objectToLoad)
                yield return objectToLoad;
        }
    }


    public static T GetOrCreateWindowOfType<T>() where T : EditorWindow
    {
        Type type = typeof(T);

        Type windowType = Assembly.Load("XDasherEditorAssembly").GetType(type.FullName);
        if (windowType == null)
            windowType = Assembly.Load("XDasherAssembly").GetType(type.FullName);

        var window = EditorWindow.GetWindow(windowType) as T;
        if (window == null)
        {
            window = ScriptableObject.CreateInstance(windowType) as T;
            window.Show();
        }
        return window;
    }

    public static T OpenNewWindowOfType<T>() where T : EditorWindow
    {
        Type type = typeof(T);
        // Create new inspector and focus target object
        Type windowType = Assembly.Load("XDasherEditorAssembly").GetType(type.FullName);
        if (windowType == null)
            windowType = Assembly.Load("XDasherAssembly").GetType(type.FullName);
        T newInspectorInstance = ScriptableObject.CreateInstance(windowType) as T;
        newInspectorInstance.Show();
        return newInspectorInstance;
    }

    public static EditorWindow CreateCostumInspectorEditorWindow(UnityEngine.Object objectToInspect)
    {
        Type windowType = Assembly.Load("XDasherEditorAssembly").GetType("CustomInspectorWindow");
        EditorWindow window = ScriptableObject.CreateInstance(windowType) as EditorWindow;
        window.Show();
        MethodInfo method = window.GetType().GetMethod("ShowObject");
        method.Invoke(window, new object[] { objectToInspect });
        return window;
    }

    public static EditorWindow GetOrCreateWindowOfType(UnityEngine.Object objectToInspect)
    {
        Type windowType = Assembly.Load("XDasherEditorAssembly").GetType("CustomInspectorWindow");
        EditorWindow window = EditorWindow.GetWindow(windowType);
        if (window == null)
        {
            window = ScriptableObject.CreateInstance(windowType) as EditorWindow;
            window.Show();
        }
        MethodInfo method = window.GetType().GetMethod("ShowObject");
        method.Invoke(window, new object[] { objectToInspect });
        return window;
    }

    public static void SwitchToSceneView()
    {
        Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.SceneView");
        EditorWindow alreadyExistingWindow = EditorWindow.GetWindow(inspectorType);

        if (alreadyExistingWindow)
            alreadyExistingWindow.Focus();
    }

    public static void SwitchToGameView()
    {
        Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        EditorWindow alreadyExistingWindow = EditorWindow.GetWindow(inspectorType);

        if (alreadyExistingWindow)
            alreadyExistingWindow.Focus();
    }

    public static void ToggleMaximizeGameView()
    {
        Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        EditorWindow gameView = EditorWindow.GetWindow(inspectorType);
        gameView.maximized = !gameView.maximized;
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    public static void LockFirstInspectorWindow()
    {
        Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        EditorWindow alreadyExistingInspectorWindow = EditorWindow.GetWindow(inspectorType);
        PropertyInfo isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
        isLocked.GetSetMethod().Invoke(alreadyExistingInspectorWindow, new object[] { true });
    }

    public static void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Editor));
        Type type = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    public static string GetAssetGuid(this UnityEngine.Object asset)
    {
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
    }

    public static string GetAssetPath(this UnityEngine.Object asset)
    {
        return AssetDatabase.GetAssetPath(asset);
    }


    /// <summary>
    /// Used for finding an asset by type, name and in the given folder.
    /// The name should NOT include the asset file extension.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadAssetOfTypeByName<T>(string assetName, string[] folderNames = null) where T : class
    {
        foreach (string guid in AssetDatabase.FindAssets(assetName, folderNames))
        {
            if (AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) is T objectToLoad)
                return objectToLoad;
        }
        return null;
    }


    public static void SelectGameObjectInHierarchy(GameObject gameObject)
    {
        Selection.activeGameObject = gameObject;
        InternalEditorUtility.RepaintAllViews();
    }

    public static void SelectAsset(Object objectToSelect)
    {
        Selection.activeObject = objectToSelect;
        EditorUtility.FocusProjectWindow();
    }

    public static void OpenAssetForEditing(Object objectToSelect)
    {
        if (SceneView.sceneViews.Count == 0)
            return;
        else
        {
            SceneView sceneView = SceneView.sceneViews?[0] as SceneView;
            sceneView?.Focus();
        }

        AssetDatabase.OpenAsset(objectToSelect);
    }


    public static string AbsolutePathToRelative(string absolutePath)
    {
        if (string.IsNullOrEmpty(absolutePath))
            return null;

        if (absolutePath.Contains("Assets") == false)
            return null;

        Regex regex = new Regex($"(Assets).+?", RegexOptions.RightToLeft);
        return regex.Match(absolutePath).Value;
    }

    public static string RelativePathToAbsolute(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return null;

        if (relativePath.Contains("Assets") == false)
            return null;

        return $"{Application.dataPath.Replace("Assets", "")}{relativePath}";
    }



    public static void LoadSceneInEditor(string sceneName)
    {
        string path = AssetDatabase.GetAssetOrScenePath(LoadAssetOfTypeByName<SceneAsset>(sceneName));
        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
    }

    public static void ForceRecompile()
    {
        AssetDatabase.ImportAsset("Assets/CSharpAssemblyClass.cs");
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public static void SaveAsset(this Object asset)
    {
        EditorUtility.SetDirty(asset);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }


    // Source: https://forum.unity.com/threads/scripted-scene-changes-not-being-saved.526453/
    public static void SetObjectDirty(this Object o)
    {
        if (Application.isPlaying)
            return;

        if (o is GameObject gameObject)
            SetObjectDirty(gameObject);
        else if (o is Component component)
            SetObjectDirty(component);
        else
            EditorUtility.SetDirty(o);
    }

    public static void SetObjectDirty(this GameObject go)
    {
        if (Application.isPlaying)
            return;

        HandlePrefabInstance(go);
        EditorUtility.SetDirty(go);

        //This stopped happening in EditorUtility.SetDirty after multi-scene editing was introduced.
        EditorSceneManager.MarkSceneDirty(go.scene);
    }

    public static void SetObjectDirty(this Component comp)
    {
        if (Application.isPlaying)
            return;

        HandlePrefabInstance(comp.gameObject);
        EditorUtility.SetDirty(comp);

        //This stopped happening in EditorUtility.SetDirty after multi-scene editing was introduced.
        EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);
    }

    // Some prefab overrides are not handled by Undo.RecordObject or EditorUtility.SetDirty.
    // eg. adding an item to an array/list on a prefab instance updates that the instance
    // has a different array count than the prefab, but not any data about the added thing
    private static void HandlePrefabInstance(GameObject gameObject)
    {
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
    }


    public static void OpenFolderOrFile(string path)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            FileName = path,
            UseShellExecute = true,
            Verb = "open"
        });
    }

    [MenuItem("Window/Pixel Beef/OpenPersistentDataPathFolder", priority = 1)]
    public static void OpenPersistentDataPathFolder()
    {
        OpenFolderOrFile(Application.persistentDataPath);
    }
}

#endif