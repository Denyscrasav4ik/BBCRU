using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Ukrainization.API;
using UnityEngine;

namespace Ukrainization.Runtime
{
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager instance = null!;

        public Dictionary<string, string> languageData = new Dictionary<string, string>();
        public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        public Dictionary<int, Texture2D> textureAssets = new Dictionary<int, Texture2D>();
        public List<Texture2D> allTextures = new List<Texture2D>();

        private string basePath = null!;
        private string texturesPath = null!;
        private string audiosPath = null!;

        public string? GetKeyData(string key)
        {
            return languageData.ContainsKey(key) ? languageData[key] : null;
        }

        public AudioClip? GetClip(string key)
        {
            return audioClips.ContainsKey(key) ? audioClips[key] : null;
        }

        public Texture2D? GetTexture2D(int hashcode)
        {
            return textureAssets.ContainsKey(hashcode) ? textureAssets[hashcode] : null;
        }

        public bool ContainsData(string key)
        {
            return languageData.ContainsKey(key);
        }

        private void UpdateTexts()
        {
            if (languageData == null || languageData.Count == 0)
                return;

            if (Singleton<LocalizationManager>.Instance != null)
            {
                FieldInfo localText = AccessTools.Field(
                    typeof(LocalizationManager),
                    "localizedText"
                );
                localText.SetValue(Singleton<LocalizationManager>.Instance, languageData);
            }
        }

        private void OnLocalizationChanged()
        {
            UpdateTexts();
        }

        public void UpdateAudio()
        {
            SoundObject[] allSounds = Resources.FindObjectsOfTypeAll<SoundObject>();
            foreach (SoundObject soundObject in allSounds)
            {
                API.Logger.Info("Завантажено звук: " + soundObject.soundClip.name);
                AudioClip? newClip = GetClip(soundObject.soundClip.name);
                if (newClip != null)
                {
                    UkrainizationTemp.UpdateClipData(soundObject, newClip);
                }
            }
        }

        public void Start()
        {
            instance = this;
            languageData = new Dictionary<string, string>();
            ModSceneManager.instance.onMenuSceneLoadOnce += UpdateAudio;
            ModSceneManager.instance.onMenuSceneLoadOnce += LoadTextures;
            ModSceneManager.instance.onMenuSceneLoadOnce += ApplyTextures;
            ModSceneManager.instance.onAnySceneLoad += OnLocalizationChanged;
            DontDestroyOnLoad(gameObject);

            allTextures = Resources.FindObjectsOfTypeAll<Texture2D>().ToList();

            basePath = UkrainizationTemp.GetBasePath();
            GameUtils.InsertDirectory(basePath);
            LoadLanguageData();
            OnLocalizationChanged();

            API.Logger.Info($"Завантажені дані мови із {basePath}");
        }

        private void LoadLanguageData()
        {
            audiosPath = Path.Combine(basePath, "Audios");
            texturesPath = Path.Combine(basePath, "Textures");
            GameUtils.InsertDirectory(audiosPath);
            GameUtils.InsertDirectory(texturesPath);

            API.Logger.Info(
                "Завантаження даних (Відсутність Subtitles_Ukrainian.json призведе до порожніх даних)"
            );
            LoadLanguageAudio(audiosPath);
            LoadLanguageSubtitles(basePath);
        }

        private void LoadLanguageAudio(string audiosPath)
        {
            string[] oggs = Directory.GetFiles(audiosPath, "*.ogg");
            string[] wavs = Directory.GetFiles(audiosPath, "*.wav");
            List<string> customClips = new List<string>();
            customClips.AddRange(oggs);
            customClips.AddRange(wavs);

            foreach (var clip in customClips)
            {
                if (File.Exists(clip))
                {
                    string clipName = Path.GetFileNameWithoutExtension(clip);
                    API.Logger.Info($"Завантажений звук: {clipName}!");
                    AudioClip? fileClip = AssetLoader.AudioClipFromFile(clip);
                    if (fileClip != null)
                    {
                        audioClips[clipName] = fileClip;
                        API.Logger.Info($"Завантажений звук: {clipName}!");
                    }
                }
            }
        }

        private void LoadLanguageSubtitles(string baseFolder)
        {
            string subtitle = Path.Combine(baseFolder, UkrainizationTemp.SubtitilesFile);
            if (File.Exists(subtitle))
            {
                string fileData = File.ReadAllText(subtitle);
                LocalizationData rawJson = JsonUtility.FromJson<LocalizationData>(fileData);
                for (int i = 0; i < rawJson.items.Length; i++)
                {
                    languageData[rawJson.items[i].key] = rawJson.items[i].value;
                }
            }
            else
            {
                API.Logger.Error($"{subtitle} не знайдено!");
            }
        }

        public void LoadTextures()
        {
            allTextures = Resources.FindObjectsOfTypeAll<Texture2D>().ToList();
            string[] pngs = Directory.GetFiles(texturesPath, "*.png");

            foreach (var pngPath in pngs)
            {
                string nameToSearch = Path.GetFileNameWithoutExtension(pngPath).Trim();
                Texture2D? targetTex = allTextures.FirstOrDefault(x => x.name == nameToSearch);

                if (targetTex == null)
                {
                    API.Logger.Warning($"Не знайдена відповідна текстура для: {nameToSearch}");
                    continue;
                }
                Texture2D? generatedTex = AssetLoader.AttemptConvertTo(
                    AssetLoader.TextureFromFile(pngPath),
                    targetTex.format
                );

                if (generatedTex != null && !textureAssets.ContainsKey(targetTex.GetHashCode()))
                {
                    textureAssets[targetTex.GetHashCode()] = generatedTex;
                    API.Logger.Info($"Завантажена текстура: {targetTex.name}");
                }
                else
                {
                    API.Logger.Warning($"Текстура {targetTex.name} вже завантажена.");
                }
            }
        }

        private void ApplyTextures()
        {
            allTextures = Resources.FindObjectsOfTypeAll<Texture2D>().ToList();
            var textureLookup = allTextures.ToDictionary(x => x.GetHashCode(), x => x);

            foreach (var kvp in textureAssets)
            {
                if (kvp.Value == null)
                {
                    API.Logger.Warning(
                        $"Текстура для ключа {kvp.Key} має значення null, пропускаємо"
                    );
                    continue;
                }

                if (textureLookup.TryGetValue(kvp.Key, out Texture2D targetTexture))
                {
                    if (targetTexture != null)
                    {
                        Graphics.CopyTexture(kvp.Value, targetTexture);
                    }
                    else
                    {
                        API.Logger.Warning($"Цільова текстура для ключа {kvp.Key} є null");
                    }
                }
                else
                {
                    API.Logger.Warning($"Не знайдена цільова текстура для ключа {kvp.Key}");
                }
            }
            API.Logger.Info($"Скопійовано {textureAssets.Count} текстур.");
        }

        internal void RegisterClip(string clipName, AudioClip loadedClip)
        {
            throw new NotImplementedException();
        }
    }
}
