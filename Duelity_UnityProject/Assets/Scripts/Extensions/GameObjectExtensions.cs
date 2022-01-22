using Duelity.Utility;
using System;
using System.Collections;
using UnityEngine;

public static class ObjectExtensions
{
    public static void WaitAndDo(this object obj, YieldInstruction yieldInstruction, Action action)
    {
        Coroutiner.Instance.StartCoroutine(WaitAndDoRoutine(yieldInstruction, action));
    }

    static IEnumerator WaitAndDoRoutine(YieldInstruction yieldInstruction, Action action)
    {
        yield return yieldInstruction;
        action?.Invoke();
    }

    public static void WaitAndDo(this object obj, CustomYieldInstruction yieldInstruction, Action action)
    {
        Coroutiner.Instance.StartCoroutine(WaitAndDoRoutine(yieldInstruction, action));
    }

    static IEnumerator WaitAndDoRoutine(CustomYieldInstruction yieldInstruction, Action action)
    {
        yield return yieldInstruction;
        action?.Invoke();
    }
}

public static class GameObjectExtensions
{
    public static string CleanedName(this GameObject gameObject)
    {
        return gameObject.name.Replace("(Clone)", "");
    }

    public static GameObject CreateInstance(this GameObject original)
    {
        return GameObject.Instantiate(original);
    }

    public static GameObject CreateInstance(this GameObject original, Transform parent)
    {
        return GameObject.Instantiate(original, parent);
    }

    public static GameObject CreateInstance(this GameObject original, Vector3 spawnPosition)
    {
        return GameObject.Instantiate(original, spawnPosition, Quaternion.identity);
    }

    public static GameObject CreateInstance(this GameObject original, Vector3 spawnPosition, Quaternion rotation)
    {
        return GameObject.Instantiate(original, spawnPosition, rotation);
    }

    public static GameObject CreateInstance(this GameObject original, Vector3 spawnPosition, Quaternion rotation, Transform parent)
    {
        return GameObject.Instantiate(original, spawnPosition, rotation, parent);
    }
}
