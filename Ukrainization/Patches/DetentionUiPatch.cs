using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using Ukrainization.API;
using UnityEngine;

namespace Ukrainization.Patches
{
    internal class DetentionUiPatch
    {
        private static HashSet<int> initializedInstances = new HashSet<int>();

        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<
            string,
            string
        >()
        {
            { "MainText", "Ukr_DetentionText" },
        };

        [HarmonyPatch(typeof(DetentionUi), "Initialize")]
        private static class InitializePatch
        {
            [HarmonyPostfix]
            private static void Postfix(DetentionUi __instance)
            {
                int instanceId = __instance.GetInstanceID();

                if (!initializedInstances.Contains(instanceId))
                {
                    ApplyLocalizationToDetentionUi(__instance.transform);
                    initializedInstances.Add(instanceId);
                }
            }

            private static void ApplyLocalizationToDetentionUi(Transform rootTransform)
            {
                if (rootTransform == null)
                    return;

                foreach (var kvp in LocalizationKeys)
                {
                    string path = kvp.Key;
                    string localizationKey = kvp.Value;

                    Transform targetTransform = rootTransform.Find(path);
                    if (targetTransform == null)
                        continue;

                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent == null)
                        continue;

                    TextLocalizer existingLocalizer = textComponent.GetComponent<TextLocalizer>();
                    if (existingLocalizer != null)
                        Object.DestroyImmediate(existingLocalizer);

                    TextLocalizer localizer =
                        textComponent.gameObject.AddComponent<TextLocalizer>();
                    localizer.key = localizationKey;
                    localizer.RefreshLocalization();

                    RectTransform rect = textComponent.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.sizeDelta = new Vector2(
                            rect.sizeDelta.x + 200f,
                            rect.sizeDelta.y + 150f
                        );
                        rect.anchoredPosition = new Vector2(
                            rect.anchoredPosition.x,
                            rect.anchoredPosition.y - 63.5f
                        );
                    }

                    textComponent.enableWordWrapping = true;
                    textComponent.overflowMode = TextOverflowModes.Overflow;
                    textComponent.margin = new Vector4(10, 10, 10, 10);
                    textComponent.enableAutoSizing = false;
                }

                Transform timeTransform = rootTransform.Find("Time");
                if (timeTransform != null)
                {
                    RectTransform timeRect = timeTransform.GetComponent<RectTransform>();
                    if (timeRect != null)
                    {
                        timeRect.anchoredPosition = new Vector2(
                            timeRect.anchoredPosition.x - 30f,
                            timeRect.anchoredPosition.y
                        );
                    }
                }
            }
        }
    }
}
