using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ukrainization.Patches
{
    [HarmonyPatch]
    internal class HudManagerPatch
    {
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<
            string,
            string
        >()
        {
            { "Notebook Text", "Ukr_Hud_Notebooks" },
        };

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HudManager), "UpdateText")]
        public static void HudManager_InitPostfix(HudManager __instance)
        {
            ApplyHudLocalization(__instance);
            UpdateNotebookTextLocalizer();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BaseGameManager), "CollectNotebooks")]
        public static void CollectNotebooks_Postfix(BaseGameManager __instance)
        {
            UpdateNotebookTextLocalizer();
        }

        private static void ApplyHudLocalization(HudManager hud)
        {
            if (hud == null)
                return;

            Transform hudTransform = hud.transform;

            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;

                Transform? targetTransform = FindInChildrenIncludingInactive(
                    hudTransform,
                    relativePath
                );
                if (targetTransform != null)
                {
                    TMP_Text? textComponent = targetTransform.GetComponent<TMP_Text>();
                    if (textComponent != null)
                    {
                        TextLocalizer? localizer = textComponent.GetComponent<TextLocalizer>();
                        if (localizer == null)
                            localizer = textComponent.gameObject.AddComponent<TextLocalizer>();

                        localizer.key = localizationKey;
                        localizer.RefreshLocalization();
                    }
                }
            }
        }

        private static void UpdateNotebookTextLocalizer()
        {
            HudManager hud = Singleton<CoreGameManager>.Instance.GetHud(0);
            if (hud == null)
                return;

            int notebookTotal = Singleton<BaseGameManager>.Instance.NotebookTotal;
            int foundNotebooks = Singleton<BaseGameManager>.Instance.FoundNotebooks;

            string localizationKey = GetNotebookLocalizationKey(foundNotebooks);

            FieldInfo? textBoxField = typeof(HudManager).GetField(
                "textBox",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (textBoxField == null)
                return;

            TMP_Text[]? textBoxArray = textBoxField.GetValue(hud) as TMP_Text[];
            if (textBoxArray != null && textBoxArray.Length > 0 && textBoxArray[0] != null)
            {
                TMP_Text targetText = textBoxArray[0];

                TextLocalizer? localizer = targetText.GetComponent<TextLocalizer>();
                if (localizer == null)
                    localizer = targetText.gameObject.AddComponent<TextLocalizer>();

                localizer.key = localizationKey;
                localizer.RefreshLocalization();

                bool endlessMode =
                    Object.FindObjectOfType<PartyEndlessManager>() != null
                    || Object.FindObjectOfType<ClassicEndlessManager>() != null
                    || Object.FindObjectOfType<DemoEndlessManager>() != null;

                targetText.text = endlessMode
                    ? $"{foundNotebooks} {targetText.text}"
                    : $"{foundNotebooks}/{notebookTotal} {targetText.text}";

                targetText.SetAllDirty();
            }
        }

        private static Transform? FindInChildrenIncludingInactive(Transform parent, string path)
        {
            var children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child == parent)
                    continue;
                if (DoesPathMatch(parent, child, path))
                    return child;
            }
            return null;
        }

        private static bool DoesPathMatch(Transform parent, Transform target, string expectedPath)
        {
            if (target == null || parent == null || target == parent)
                return false;

            System.Text.StringBuilder pathBuilder = new System.Text.StringBuilder();
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
