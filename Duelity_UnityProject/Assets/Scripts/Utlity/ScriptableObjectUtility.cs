#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Duelity.Utility
{
    public static class ScriptableObjectUtility
    {
        public static T CreateAsset<T>(string fullPath) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("CreateAsset fullPath IsNullOrEmpty!");
            T asset = ScriptableObject.CreateInstance<T>();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        /// <summary>
        ///  Creates new asset of type T with the given name and saves it into the same folder as the parent Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// No file extension required
        /// <param name="parentObject"></param>
        /// <returns></returns>
        public static T CreateAsset<T>(string assetName, UnityEngine.Object parentObject) where T : ScriptableObject
        {
            string parentObjectAssetPath = AssetDatabase.GetAssetPath(parentObject);
            string folderPath = parentObjectAssetPath.Replace($"{parentObject.name}{Path.GetExtension(parentObjectAssetPath)}", "");
            string newAssetPath = $"{folderPath}{assetName}.asset";
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("CreateAsset assetName IsNullOrEmpty!");
            if (parentObject == false)
                throw new ArgumentNullException("CreateAsset parentObject is null!");

            T asset = ScriptableObject.CreateInstance<T>();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        public static UnityEngine.Object CreateAsset(Type type, string fullPath)
        {
            if (type == null)
                throw new ArgumentNullException("CreateAsset type is null!");
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("CreateAsset fullPath IsNullOrEmpty!");

            UnityEngine.Object asset = ScriptableObject.CreateInstance(type);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }
    }
}

#endif