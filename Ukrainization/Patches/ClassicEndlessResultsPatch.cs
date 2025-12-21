using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ukrainization.Patches
{
    [HarmonyPatch(typeof(ClassicEndlessResults), "Initialize")]
    internal class ClassicEndlessResultsPatch
    {
        private static Transform? FindInChildrenIncludingInactive(Transform parent, string path)
        {
            var children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child == parent)
                    continue;
                if (DoesPathMatch(parent, child, path))
                {
                    return child;
                }
            }
            return null;
        }

        private static bool DoesPathMatch(Transform parent, Transform target, string expectedPath)
        {
            if (target == null || parent == null || target == parent)
                return false;
            StringBuilder pathBuilder = new StringBuilder();
            Transform current = target;
            while (current != null && current != parent)
            {
                if (pathBuilder.Length > 0)
                    pathBuilder.Insert(0, "/");
                pathBuilder.Insert(0, current.name);
                current = current.parent;
            }
            if (current != parent)
                return false;
            return pathBuilder.ToString() == expectedPath;
        }

        [HarmonyPostfix]
        private static void Initialize_Postfix(
            ClassicEndlessResults __instance,
            int score,
            int rank
        )
        {
            ApplyLocalizationAndMove(__instance, score);
        }

        private static void ApplyLocalizationAndMove(ClassicEndlessResults instance, int score)
        {
            if (instance == null || instance.scoreTmp == null)
                return;

            TextMeshProUGUI textComponent = instance.scoreTmp as TextMeshProUGUI;
            if (textComponent == null)
                return;

            RectTransform rect = textComponent.GetComponent<RectTransform>();
            if (rect != null)
                rect.anchoredPosition -= new Vector2(0, 20f);

            string localizationKey = GetNotebookLocalizationKey(score);

            TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
            if (localizer == null)
                localizer = textComponent.gameObject.AddComponent<TextLocalizer>();

            localizer.key = localizationKey;
            localizer.RefreshLocalization();

            textComponent.text =
                $"{score} {Singleton<LocalizationManager>.Instance.GetLocalizedText(localizationKey)}";
        }

        private static string GetNotebookLocalizationKey(int number)
        {
            int lastTwoDigits = number % 100;
            int lastDigit = number % 10;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
                return "Ukr_Hud_Notebooks1";

            switch (lastDigit)
            {
                case 1:
                    return "Ukr_Hud_Notebooks2";
                case 2:
                case 3:
                case 4:
                    return "Ukr_Hud_Notebooks3";
                default:
                    return "Ukr_Hud_Notebooks1";
            }
        }
    }
}
