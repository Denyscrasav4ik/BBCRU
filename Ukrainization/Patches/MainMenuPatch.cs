using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using Ukrainization.API;
using UnityEngine;

namespace Ukrainization.Patches
{
    internal class MainMenuPatch
    {
        private static bool initialized = false;

        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<
            string,
            string
        >()
        {
            { "Version", "Ukr_MainMenu_Version" },
        };

        [HarmonyPatch(typeof(MainMenu), "Start")]
        private static class MainMenuStartPatch
        {
            [HarmonyPostfix]
            private static void Postfix(MainMenu __instance)
            {
                if (!initialized)
                {
                    ApplyLocalizationToMainMenu(__instance.transform);
                    initialized = true;
                }
            }
        }

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "Title" && value && !initialized)
                {
                    ApplyLocalizationToMainMenu(__instance.transform);
                    initialized = true;
                }
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                initialized = false;
            }
        }

        private static void ApplyLocalizationToMainMenu(Transform rootTransform)
        {
            if (rootTransform == null)
                return;

            foreach (var kvp in LocalizationKeys)
            {
                string path = kvp.Key;
                string localizationKey = kvp.Value;

                Transform targetTransform = rootTransform.Find(path);
                if (targetTransform != null)
                {
                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        TextLocalizer existingLocalizer =
                            textComponent.GetComponent<TextLocalizer>();
                        if (existingLocalizer != null)
                        {
                            Object.DestroyImmediate(existingLocalizer);
                        }

                        TextLocalizer localizer =
                            textComponent.gameObject.AddComponent<TextLocalizer>();
                        localizer.key = localizationKey;
                        localizer.RefreshLocalization();
                    }
                }
            }
        }
    }
}
