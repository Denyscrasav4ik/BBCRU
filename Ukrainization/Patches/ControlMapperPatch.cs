using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Rewired.UI.ControlMapper;
using TMPro;
using UnityEngine;

namespace Ukrainization.Patches
{
    [HarmonyPatch(typeof(Rewired.UI.ControlMapper.ControlMapper), "Initialize")]
    internal class ControlMapperPatch
    {
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<
            string,
            string
        >()
        {
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/WindowButtonGroup/DoneButton/TextMeshPro Text",
                "Ukr_Ctrl_Done"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/WindowButtonGroup/RestoreDefaultsButton/TextMeshPro Text",
                "Ukr_Ctrl_RestoreDefaults"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/PlayersGroup/Text",
                "Ukr_Ctrl_Players"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/ControllerGroup/ControllerSettingsGroup/ControllerLabelGroup/ControllerLabel",
                "Ukr_Ctrl_ControllerLabel"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/ControllerGroup/ControllerSettingsGroup/ButtonLayoutGroup/RemoveButton/Text",
                "Ukr_Ctrl_RemoveController"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/ControllerGroup/ControllerSettingsGroup/ButtonLayoutGroup/CalibrateButton/Text",
                "Ukr_Ctrl_CalibrateController"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/ControllerGroup/ControllerSettingsGroup/ButtonLayoutGroup/AssignControllerButton/Text",
                "Ukr_Ctrl_AssignController"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/ControllerGroup/AssignedControllersGroup/Label",
                "Ukr_Ctrl_AssignedControllers"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/ControllerGroup/AssignedControllersGroup/ButtonLayoutGroup/Button(Clone)/Text",
                "Ukr_Ctrl_AssignedControllersButton"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/SettingAndMapCategoriesGroup/SettingsGroup/Label",
                "Ukr_Ctrl_Settings"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/SettingAndMapCategoriesGroup/MapCategoriesGroup/Label",
                "Ukr_Ctrl_MapCategories"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/SettingAndMapCategoriesGroup/MapCategoriesGroup/ButtonLayoutGroup/DefaultButton/Text",
                "Ukr_Ctrl_GameplayButton"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/SettingAndMapCategoriesGroup/MapCategoriesGroup/ButtonLayoutGroup/MenuButton/Text",
                "Ukr_Ctrl_MenuButton"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/HeadersGroup/ActionsHeader/InputGridHeaderLabel(Clone)",
                "Ukr_Ctrl_Actions"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/HeadersGroup/KeybordHeader/InputGridHeaderLabel(Clone)",
                "Ukr_Ctrl_Keyboard"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/HeadersGroup/MouseHeader/InputGridHeaderLabel(Clone)",
                "Ukr_Ctrl_Mouse"
            },
            {
                "Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/HeadersGroup/ControllerHeader/InputGridHeaderLabel(Clone)",
                "Ukr_Ctrl_Controller"
            },
        };

        private static readonly string PersistentLabelPath =
            "Canvas/MainPageGroup/MainContent/MainContentInner/ControllerGroup/ControllerSettingsGroup/ControllerLabelGroup/ControllerNameWrapper/ControllerNameLabel";

        private static readonly List<Transform> InputGridLabels = new List<Transform>();
        private static readonly Dictionary<int, string> InputGridLabelKeys =
            new Dictionary<int, string>();

        [HarmonyPostfix]
        private static void OnInitializePostfix(ControlMapper __instance)
        {
            if (__instance == null)
                return;

            ApplyStaticLocalization(__instance);
            ApplyPersistentLocalization(__instance);
            InitializeInputGridLabelKeys();
            CacheAndLocalizeInputGridLabels(__instance);
        }

        private static void ApplyStaticLocalization(ControlMapper instance)
        {
            foreach (var entry in LocalizationKeys)
            {
                Transform targetTransform = FindInChildrenIncludingInactive(
                    instance.transform,
                    entry.Key
                );
                if (targetTransform != null)
                {
                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        var localizer =
                            textComponent.gameObject.GetComponent<TextLocalizer>()
                            ?? textComponent.gameObject.AddComponent<TextLocalizer>();

                        localizer.key = entry.Value;
                        localizer.RefreshLocalization();
                    }
                }
            }
        }

        private static void ApplyPersistentLocalization(ControlMapper instance)
        {
            Transform labelTransform = FindInChildrenIncludingInactive(
                instance.transform,
                PersistentLabelPath
            );
            if (labelTransform != null)
            {
                var textComponent = labelTransform.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    var persistent =
                        textComponent.gameObject.GetComponent<PersistentTextLocalizer>()
                        ?? textComponent.gameObject.AddComponent<PersistentTextLocalizer>();
                    persistent.key = "Ukr_Ctrl_ControllerName";
                }
            }
        }

        private static void InitializeInputGridLabelKeys()
        {
            InputGridLabelKeys.Clear();
            for (int i = 0; i < 42; i++)
            {
                InputGridLabelKeys[i] = $"Ukr_Ctrl_Action_{i}";
            }
        }

        private static void CacheAndLocalizeInputGridLabels(ControlMapper instance)
        {
            InputGridLabels.Clear();

            Transform parent = FindInChildrenIncludingInactive(
                instance.transform,
                "Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/ScrollRect/InputGridInnerGroup/ActionLabelColumn"
            );

            if (parent == null)
                return;

            for (int i = 0; i < 42; i++)
            {
                if (i >= parent.childCount)
                    break;

                Transform child = parent.GetChild(i);
                if (child != null && child.name.Contains("InputGridLabel"))
                {
                    InputGridLabels.Add(child);

                    TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        var localizer =
                            child.gameObject.GetComponent<TextLocalizer>()
                            ?? child.gameObject.AddComponent<TextLocalizer>();

                        if (InputGridLabelKeys.TryGetValue(i, out string key))
                        {
                            localizer.key = key;
                            localizer.RefreshLocalization();
                        }
                    }
                }
            }
        }

        public static Transform GetInputGridLabelByIndex(int index)
        {
            if (index < 0 || index >= InputGridLabels.Count)
                return null;
            return InputGridLabels[index];
        }

        private static Transform FindInChildrenIncludingInactive(Transform parent, string path)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (DoesPathMatch(parent, child, path))
                    return child;
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

            return pathBuilder.ToString() == expectedPath;
        }

        #region Dynamic Button Localization on Windows

        [HarmonyPatch(typeof(Rewired.UI.ControlMapper.Window), "Enable")]
        private class WindowShowPatch
        {
            [HarmonyPostfix]
            private static void Postfix(Rewired.UI.ControlMapper.Window __instance)
            {
                ApplyDynamicButtonLocalization(__instance);
                ApplyDynamicTitleLocalization(__instance);
            }

            private static void ApplyDynamicButtonLocalization(
                Rewired.UI.ControlMapper.Window window
            )
            {
                Transform contentParent = window.transform.Find("Content");
                if (contentParent == null)
                    return;

                foreach (
                    Transform buttonTransform in contentParent.GetComponentsInChildren<Transform>(
                        true
                    )
                )
                {
                    if (!buttonTransform.name.Contains("Button"))
                        continue;

                    Transform textTransform = buttonTransform.Find("Text");
                    if (textTransform == null)
                        continue;

                    TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
                    if (text == null)
                        continue;

                    var localizer =
                        text.gameObject.GetComponent<TextLocalizer>()
                        ?? text.gameObject.AddComponent<TextLocalizer>();

                    string tmpText = text.text;

                    if (tmpText.Contains("Replace"))
                        localizer.key = "Ukr_Ctrl_ReplaceButton";
                    else if (tmpText.Contains("Remove"))
                        localizer.key = "Ukr_Ctrl_RemoveButton";
                    else if (tmpText.Contains("Cancel"))
                        localizer.key = "Ukr_Ctrl_CancelButton";
                    else if (tmpText.Contains("Calibrate"))
                        localizer.key = "Ukr_Ctrl_CalibrateButton";
                    else if (tmpText.Contains("Yes"))
                        localizer.key = "Ukr_Ctrl_YesButton";
                    else if (tmpText.Contains("No"))
                        localizer.key = "Ukr_Ctrl_NoButton";

                    localizer.RefreshLocalization();
                }
            }
        }

        private static void ApplyDynamicTitleLocalization(Rewired.UI.ControlMapper.Window window)
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
            else if (tmpText.Contains("Сalibrate Controller"))
                localizer.key = "Ukr_Ctrl_Calibrate_Title";
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

        #endregion
    }

    public class PersistentTextLocalizer : MonoBehaviour
    {
        public string key;
        private TextMeshProUGUI textComponent;
        private TextLocalizer localizer;

        private void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            localizer = GetComponent<TextLocalizer>() ?? gameObject.AddComponent<TextLocalizer>();
            localizer.key = key;
        }

        private void LateUpdate()
        {
            if (textComponent == null || localizer == null)
                return;

            if (textComponent.text == "None" || textComponent.text == "Жодний")
            {
                localizer.RefreshLocalization();
            }
        }
    }
}
