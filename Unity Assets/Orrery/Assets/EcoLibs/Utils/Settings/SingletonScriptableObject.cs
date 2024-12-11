// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils.Settings
{
    using System.Linq;
    using UnityEngine;

    /// <summary> ScriptableObject to be used as Singleton style </summary>
    /// <typeparam name="T"> Should always be the object itself </typeparam>
    /// for a example check <see cref="Eco.LocalizationTools.MissingLocalizations"/>
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        protected virtual string Path { get; } // path to save asset
        static T instance = null;              // local instance
        public static T Instance               // public access to local instance
        {
            get
            {
                if (!instance) instance = LoadOrCreate();
                return instance;
            }
        }

        private static T LoadOrCreate()
        {
            var inst = Resources.LoadAll<T>(string.Empty).FirstOrDefault() ?? Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(); // picks first asset found
            if (!inst)                                                          // if asset is not found, creates a new one and saves
            {
#if UNITY_EDITOR
                inst = CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(inst, inst.Path);
                UnityEditor.AssetDatabase.SaveAssets();
#else
                throw new System.Exception($"Missing ScriptableObject {nameof(T)}!");   // throw error if SO is missing in standalone   
#endif
            }

            return inst;
        }
    }
}
