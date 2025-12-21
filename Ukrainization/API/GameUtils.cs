using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Ukrainization.API
{
    public static class GameUtils
    {
        public static string? GetFileFrom(
            string[] paths,
            string fileName = "Subtitles_Ukrainian.json"
        )
        {
            foreach (var path in paths)
            {
                if (path.Contains(fileName))
                {
                    return path;
                }
            }
            return null;
        }

        public static T GetAssetFromResources<T>(string includedName = "ObjectName")
            where T : Component
        {
            var foundList = Resources.FindObjectsOfTypeAll<T>();

            foreach (var obj in foundList)
            {
                if (obj.name == includedName)
                {
                    return obj;
                }
            }
            return foundList.First();
        }

        public static void InsertDirectory(string mainPath)
        {
            if (!Directory.Exists(mainPath))
            {
                Directory.CreateDirectory(mainPath);
            }
        }

        public static void CreateInstance<T>()
            where T : MonoBehaviour
        {
            if (GameObject.FindObjectOfType<T>(true) == null)
            {
                T newInstance = GameObject.Instantiate(new GameObject()).AddComponent<T>();
                newInstance.name = typeof(T).Name;
                Logger.Info($"Створено {newInstance.name}");
            }
            else
            {
                throw new System.Exception($"Клас {typeof(T).Name} вже існує!");
            }
        }

        public static T CreateInstanceI<T>()
            where T : MonoBehaviour
        {
            if (GameObject.FindObjectOfType<T>(true) == null)
            {
                T newInstance = GameObject.Instantiate(new GameObject()).AddComponent<T>();
                newInstance.name = typeof(T).Name;
                Logger.Info($"Створено {newInstance.name}");
                return newInstance;
            }
            else
            {
                throw new System.Exception($"Клас {typeof(T).Name} вже існує!");
            }
        }
    }
}
