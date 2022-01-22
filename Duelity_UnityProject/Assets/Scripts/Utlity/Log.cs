using System.Linq;
using System.Diagnostics;
using UnityEngine;

public static class Log
{
    public const string EDITOR_DEFINE = "UNITY_EDITOR";
    public const string DEVBUILD_DEFINE = "DEVELOPMENT_BUILD";
    public const string STANDALONE_DEFINE = "UNITY_STANDALONE";

    // Log

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void Info(this object @object, params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        UnityEngine.Debug.Log(output, @object as Object);
    }

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void InfoWithColor(this object @object, string color, params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        output = $"<color={color}>{output}</color>";
        UnityEngine.Debug.Log(output, @object as Object);
    }

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void InfoStatic(params object[] args)
    {
        string output = string.Join(",", args.ToArray());
        UnityEngine.Debug.Log(output);
    }

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void InfoStaticWithColor(string color, params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        output = $"<color={color}>{output}</color>";
        UnityEngine.Debug.Log(output);
    }

    // Warning

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void Warning(this object @object, params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        UnityEngine.Debug.LogWarning(output, @object as Object);
    }

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void WarningStatic(params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        UnityEngine.Debug.LogWarning(output);
    }

    // Error

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void Error(this object @object, params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        UnityEngine.Debug.LogError(output, @object as Object);
    }

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE), Conditional(STANDALONE_DEFINE)]
    public static void ErrorBuildIncluded(this object @object, params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        UnityEngine.Debug.LogError(output, @object as Object);
    }

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE)]
    public static void ErrorStatic(params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        UnityEngine.Debug.LogError(output);
    }

    [Conditional(EDITOR_DEFINE), Conditional(DEVBUILD_DEFINE), Conditional(STANDALONE_DEFINE)]
    public static void ErrorStaticBuildIncluded(params object[] args)
    {
        string output = args.Length > 1 ? string.Join(",", args.ToArray()) : args[0].ToString();
        UnityEngine.Debug.LogError(output);
    }
}