using UnityEngine;
using System.Collections.Generic;
using Duelity.Utility;
using System.Collections;
using System;

public static class TransformExtensions
{
    public static T GetFirstComponentOfTypeUpHierarchy<T>(this Transform transform) where T : class
    {
        Transform currentTransfrom = transform;
        T t = transform.GetComponent<T>();
        while (t == null && currentTransfrom.parent)
        {
            currentTransfrom = currentTransfrom.parent;
            t = currentTransfrom.GetComponent<T>();
        }
        return t;
    }

    public static bool TryGetFirstComponentOfTypeUpHierarchy<T>(this Transform transform, out T t)
    {
        Transform currentTransfrom = transform;
        t = transform.GetComponent<T>();
        while (t == null && currentTransfrom.parent)
        {
            currentTransfrom = currentTransfrom.parent;
            t = currentTransfrom.GetComponent<T>();
        }
        return t != null;
    }

    public static IEnumerable<Transform> GetChildren(this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform transform = parent.GetChild(i);
            yield return transform;
        }
    }

    public static void GetAllChildTransforms(this Transform startingTransform, ref List<Transform> transforms)
    {
        for (int i = 0; i < startingTransform.childCount; i++)
        {
            Transform child = startingTransform.GetChild(i);
            transforms.Add(child);
            child.GetAllChildTransforms(ref transforms);
        }
    }

    public static void GetAllComponentsOfType<T>(this Transform startingTransform, ref List<T> components) where T : Component
    {
        if (startingTransform.TryGetComponent(out T component))
            components.Add(component);

        for (int i = 0; i < startingTransform.childCount; i++)
        {
            Transform child = startingTransform.GetChild(i);
            if (child.TryGetComponent(out T t1))
                components.Add(t1);
            child.GetAllComponentsOfType(ref components);
        }
    }

    public static bool TryGetAllComponentsOfType<T>(this Transform startingTransform, out List<T> components) where T : Component
    {
        components = new List<T>();
        if (startingTransform.TryGetComponent(out T component))
            components.Add(component);

        for (int i = 0; i < startingTransform.childCount; i++)
        {
            Transform child = startingTransform.GetChild(i);
            if (child.TryGetComponent(out T t1))
                components.Add(t1);
            child.GetAllComponentsOfType(ref components);
        }

        return components.Count > 0;
    }

    public static void GetAllComponentsOfType<T>(this Transform startingTransform, ref List<T> components, Func<T, bool> selector) where T : Component
    {
        foreach (Transform child in startingTransform)
        {
            if (child.TryGetComponent(out T t1) && selector(t1))
                components.Add(t1);
            child.GetAllComponentsOfType(ref components, selector);
        }
    }

    public static void GetNumberOfComponentsOfType<T>(this Transform transform, ref int count) where T : Component
    {
        foreach (Transform child in transform)
        {
            T t = child.GetComponent<T>();
            if (t != null)
                count++;
            child.GetNumberOfComponentsOfType<T>(ref count);
        }
    }

    #region Move

    public static void MoveTransform(this Transform transform, float duration, Vector3 targetPos, Action onDoneCallback)
    {
        Coroutiner.Instance.StartCoroutine(transform.MoveTransformRoutine(duration, targetPos, onDoneCallback));
    }

    public static IEnumerator MoveTransformRoutine(this Transform transform, float duration, Vector3 targetPos, Action onDoneCallback)
    {
        float elapsedTime = 0f;
        Vector2 startPos = transform.position;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }
        onDoneCallback?.Invoke();
    }

    public static void MoveTransform(this Transform transform, AnimationCurve animationCurve, float duration, Vector3 targetPos, Action onDoneCallback)
    {
        Coroutiner.Instance.StartCoroutine(transform.MoveTransformRoutine(animationCurve, duration, targetPos, onDoneCallback));
    }

    public static IEnumerator MoveTransformRoutine(this Transform transform, AnimationCurve animationCurve, float duration, Vector3 targetPos, Action onDoneCallback)
    {
        float elapsedTime = 0f;
        Vector2 startPos = transform.position;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, animationCurve.Evaluate(progress));
            yield return null;
        }
        onDoneCallback?.Invoke();
    }

    public static IEnumerator MoveTransformRoutineIgnoreTimeScale(this Transform transform, AnimationCurve animationCurve, float duration, Vector3 targetPos, Action onDoneCallback = null)
    {
        float elapsedTime = 0f;
        Vector2 startPos = transform.position;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, animationCurve.Evaluate(progress));
            yield return null;
        }
        onDoneCallback?.Invoke();
    }

    #endregion

    #region Scale

    public static void ScaleTransformUniformly(this Transform transform, float targetScale, float duration, AnimationCurve animationCurve = null, Action onDoneCallback = null)
    {
        Coroutiner.Instance.StartCoroutine(transform.ScaleTransformUniformlyRoutine(targetScale, duration, animationCurve, onDoneCallback));
    }

    public static IEnumerator ScaleTransformUniformlyRoutine(this Transform transform, float targetScale, float duration, AnimationCurve animationCurve = null, Action onDoneCallback = null)
    {
        float elapsedTime = 0f;
        Vector3 start = transform.localScale;
        Vector3 end = Vector3.one * targetScale;
        while (elapsedTime < duration)
        {
            if (!transform)
                yield break;
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(start, end, animationCurve?.Evaluate(progress) ?? progress);
            yield return null;
        }
        transform.localScale = end;
        onDoneCallback?.Invoke();
    }

    #endregion
}