using System.IO;
using BepInEx;
using HarmonyLib;
using Ukrainization.API;
using Ukrainization.Patches;
using Ukrainization.Runtime;
using UnityEngine;

namespace Ukrainization
{
    [BepInPlugin(
        UkrainizationTemp.ModGUID,
        UkrainizationTemp.ModName,
        UkrainizationTemp.ModVersion
    )]
    [BepInProcess("BALDI.exe")]
    public class TPPlugin : BaseUnityPlugin
    {
        private Harmony harmonyInstance = null!;
        private const string expectedGameVersion = "1.1a";

        public void Awake()
        {
            API.Logger.Init(this.Logger);
            ConfigManager.Initialize(this, this.Logger);

            API.Logger.Info($"Плагін {UkrainizationTemp.ModName} ініціалізовано.");
            API.Logger.Info(
                $"Текстури: {(ConfigManager.AreTexturesEnabled() ? "Увімкнено" : "Вимкнено")}, "
                    + $"Звуки: {(ConfigManager.AreSoundsEnabled() ? "Увімкнено" : "Вимкнено")}, "
                    + $"Логування: {(ConfigManager.IsLoggingEnabled() ? "Увімкнено" : "Вимкнено")}"
            );

            CreateModDirectories();
            harmonyInstance = new Harmony(UkrainizationTemp.ModGUID);
            harmonyInstance.PatchAll();

            GameUtils.CreateInstance<ModSceneManager>();
            GameUtils.CreateInstance<PostersManager>();
            GameUtils.CreateInstance<UkrainizationController>();
            GameUtils.CreateInstance<LanguageManager>();
            gameObject.AddComponent<ButtonNamesPatch>();
        }

        private void CreateModDirectories()
        {
            string basePath = UkrainizationTemp.GetBasePath();
            GameUtils.InsertDirectory(basePath);

            GameUtils.InsertDirectory(UkrainizationTemp.GetAudioPath());
            GameUtils.InsertDirectory(UkrainizationTemp.GetTexturePath());
            GameUtils.InsertDirectory(UkrainizationTemp.GetPostersPath());

            API.Logger.Info($"Створено директорії для ассетів моду в: {basePath}");
        }

        public void OnDestroy()
        {
            if (harmonyInstance != null)
            {
                harmonyInstance.UnpatchSelf();
                harmonyInstance = null!;
            }
        }
    }
}
