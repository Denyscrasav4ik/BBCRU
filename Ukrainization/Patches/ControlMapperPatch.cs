using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ukrainization.Patches
{
    [HarmonyPatch(typeof(Rewired.UI.ControlMapper.ControlMapper), "Initialize")]
    internal class ControlMapperPatch
    {
        [HarmonyPostfix]
        private static void OnInitializePostfix(Rewired.UI.ControlMapper.ControlMapper __instance)
        {
            if (__instance == null)
                return;

            ApplyStaticCategoryButtonLocalization(__instance);
            ApplyDynamicActionGridLocalization(__instance);
        }

        private static void ApplyStaticCategoryButtonLocalization(
            Rewired.UI.ControlMapper.ControlMapper controlMapper
        )
        {
            ApplyLocalization(
                controlMapper,
                "Canvas/MainPageGroup/MainContent/MainContentInner/SettingAndMapCategoriesGroup/MapCategoriesGroup/ButtonLayoutGroup/DefaultButton/Text",
                "Ukr_Ctrl_GameplayButton"
            );

            ApplyLocalization(
                controlMapper,
                "Canvas/MainPageGroup/MainContent/MainContentInner/SettingAndMapCategoriesGroup/MapCategoriesGroup/ButtonLayoutGroup/MenuButton/Text",
                "Ukr_Ctrl_MenuButton"
            );
        }

        private static void ApplyLocalization(
            Rewired.UI.ControlMapper.ControlMapper controlMapper,
            string path,
            string key
        )
        {
            Transform target = controlMapper.transform.Find(path);
            if (target == null)
                return;

            TextMeshProUGUI text = target.GetComponent<TextMeshProUGUI>();
            if (text == null)
                return;

            var localizer =
                text.gameObject.GetComponent<TextLocalizer>()
                ?? text.gameObject.AddComponent<TextLocalizer>();

            localizer.key = key;
            localizer.RefreshLocalization();
        }

        #region Dynamic Button & Title Localization on Windows

        [HarmonyPatch(typeof(Rewired.UI.ControlMapper.Window), "Enable")]
        private class WindowShowPatch
        {
            [HarmonyPostfix]
            private static void Postfix(Rewired.UI.ControlMapper.Window __instance)
            {
                ApplyDynamicTitleLocalization(__instance);
            }

            private static void ApplyDynamicTitleLocalization(
                Rewired.UI.ControlMapper.Window window
            )
            {
                Transform contentParent = window.transform.Find("Content");
                if (contentParent == null)
                    return;

                TextMeshProUGUI titleText = null;
                foreach (Transform t in contentParent.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "Title Text")
                    {
                        titleText = t.GetComponent<TextMeshProUGUI>();
                        if (titleText != null)
                            break;
                    }
                }

                if (titleText == null)
                    return;

                var localizer =
                    titleText.gameObject.GetComponent<TextLocalizer>()
                    ?? titleText.gameObject.AddComponent<TextLocalizer>();

                string tmpText = titleText.text;

                if (tmpText.Contains("Restore Defaults"))
                    localizer.key = "Ukr_Ctrl_RestoreDefaults";
                else if (tmpText.Contains("Horizontal movement"))
                    localizer.key = "Ukr_Ctrl_Action_1";
                else if (tmpText.Contains("Strafe right"))
                    localizer.key = "Ukr_Ctrl_Action_2";
                else if (tmpText.Contains("Strafe left"))
                    localizer.key = "Ukr_Ctrl_Action_3";
                else if (tmpText.Contains("Vertical movement"))
                    localizer.key = "Ukr_Ctrl_Action_4";
                else if (tmpText.Contains("Move forward"))
                    localizer.key = "Ukr_Ctrl_Action_5";
                else if (tmpText.Contains("Move backward"))
                    localizer.key = "Ukr_Ctrl_Action_6";
                else if (tmpText.Contains("Turn (Joystick)"))
                    localizer.key = "Ukr_Ctrl_Action_7";
                else if (tmpText.Contains("Turn right"))
                    localizer.key = "Ukr_Ctrl_Action_8";
                else if (tmpText.Contains("Turn left"))
                    localizer.key = "Ukr_Ctrl_Action_9";
                else if (tmpText.Contains("Turn (Mouse)"))
                    localizer.key = "Ukr_Ctrl_Action_10";
                else if (tmpText.Contains("Turn right"))
                    localizer.key = "Ukr_Ctrl_Action_11";
                else if (tmpText.Contains("Turn left"))
                    localizer.key = "Ukr_Ctrl_Action_12";
                else if (tmpText.Contains("Interact"))
                    localizer.key = "Ukr_Ctrl_Action_14";
                else if (tmpText.Contains("Use item"))
                    localizer.key = "Ukr_Ctrl_Action_15";
                else if (tmpText.Contains("Run"))
                    localizer.key = "Ukr_Ctrl_Action_16";
                else if (tmpText.Contains("Look back"))
                    localizer.key = "Ukr_Ctrl_Action_17";
                else if (tmpText.Contains("Item select left"))
                    localizer.key = "Ukr_Ctrl_Action_18";
                else if (tmpText.Contains("Item select right"))
                    localizer.key = "Ukr_Ctrl_Action_19";
                else if (tmpText.Contains("Item select axis"))
                    localizer.key = "Ukr_Ctrl_Action_20";
                else if (tmpText.Contains("ItemSelect +"))
                    localizer.key = "Ukr_Ctrl_Action_21";
                else if (tmpText.Contains("ItemSelect -"))
                    localizer.key = "Ukr_Ctrl_Action_22";
                else if (tmpText.Contains("Item 1"))
                    localizer.key = "Ukr_Ctrl_Action_23";
                else if (tmpText.Contains("Item 2"))
                    localizer.key = "Ukr_Ctrl_Action_24";
                else if (tmpText.Contains("Item 3"))
                    localizer.key = "Ukr_Ctrl_Action_25";
                else if (tmpText.Contains("Pause"))
                    localizer.key = "Ukr_Ctrl_Action_26";
                else if (tmpText.Contains("Click"))
                    localizer.key = "Ukr_Ctrl_Action_28";
                else if (tmpText.Contains("Cursor Speed Boost"))
                    localizer.key = "Ukr_Ctrl_Action_29";
                else if (tmpText.Contains("Cursor Horizontal (Joystick)"))
                    localizer.key = "Ukr_Ctrl_Action_30";
                else if (tmpText.Contains("Cursor Horizontal (Joystick) +"))
                    localizer.key = "Ukr_Ctrl_Action_31";
                else if (tmpText.Contains("Cursor Horizontal (Joystick) -"))
                    localizer.key = "Ukr_Ctrl_Action_32";
                else if (tmpText.Contains("Cursor Vertical (Joystick)"))
                    localizer.key = "Ukr_Ctrl_Action_33";
                else if (tmpText.Contains("Cursor Vertical (Joystick) +"))
                    localizer.key = "Ukr_Ctrl_Action_34";
                else if (tmpText.Contains("Cursor Vertical (Joystick) -"))
                    localizer.key = "Ukr_Ctrl_Action_35";
                else if (tmpText.Contains("Cursor Horizontal (Mouse)"))
                    localizer.key = "Ukr_Ctrl_Action_36";
                else if (tmpText.Contains("Cursor Horizontal (Mouse) +"))
                    localizer.key = "Ukr_Ctrl_Action_37";
                else if (tmpText.Contains("Cursor Horizontal (Mouse) -"))
                    localizer.key = "Ukr_Ctrl_Action_38";
                else if (tmpText.Contains("Cursor Vertical (Mouse)"))
                    localizer.key = "Ukr_Ctrl_Action_39";
                else if (tmpText.Contains("Cursor Vertical (Mouse) +"))
                    localizer.key = "Ukr_Ctrl_Action_40";
                else if (tmpText.Contains("Cursor Vertical (Mouse) -"))
                    localizer.key = "Ukr_Ctrl_Action_41";

                localizer.RefreshLocalization();
            }
        }

        #endregion

        private static void ApplyDynamicActionGridLocalization(
            Rewired.UI.ControlMapper.ControlMapper controlMapper
        )
        {
            Transform actionColumn = controlMapper.transform.Find(
                "Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/ScrollRect/InputGridInnerGroup/ActionLabelColumn"
            );

            if (actionColumn == null)
                return;

            int index = -1;
            foreach (Transform child in actionColumn)
            {
                if (!child.name.Contains("InputGridLabel(Clone)"))
                    continue;

                TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                if (text == null)
                    continue;

                var localizer =
                    text.gameObject.GetComponent<TextLocalizer>()
                    ?? text.gameObject.AddComponent<TextLocalizer>();

                localizer.key = $"Ukr_Ctrl_Action_{index + 1}";
                localizer.RefreshLocalization();
                index++;
            }
        }
    }
}
