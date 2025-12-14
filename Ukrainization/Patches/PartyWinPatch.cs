using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ukrainization.Patches
{
    [HarmonyPatch(typeof(ClassicPartyWin), "Initialize")]
    internal class PartyWinPatch
    {
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<
            string,
            string
        >()
        {
            { "Canvas/Fault", "Ukr_PartyWin_FaultText" },
            { "Canvas/Fault (1)", "Ukr_PartyWin_Fault1Text" },
            { "Canvas/Fault (2)", "Ukr_PartyWin_Fault2Text" },
        };

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
        [HarmonyPatch(typeof(ClassicPartyWin), "Initialize")]
        private static void Initialize_Postfix(ClassicPartyWin __instance)
        {
            ApplyLocalization(__instance);
        }

        private static void ApplyLocalization(ClassicPartyWin partyWinInstance)
        {
            if (partyWinInstance == null)
                return;

            Transform partyWinTransform = partyWinInstance.transform;

            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;

                Transform? targetTransform = FindInChildrenIncludingInactive(
                    partyWinTransform,
                    relativePath
                );
                if (targetTransform != null)
                {
                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                            localizer.key = localizationKey;
                        }
                        else if (localizer.key != localizationKey)
                        {
                            localizer.key = localizationKey;
                            localizer.RefreshLocalization();
                        }
                    }
                }
            }
        }
    }
}
