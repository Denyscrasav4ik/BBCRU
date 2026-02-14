using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ukrainization.Patches
{
    internal class ButtonNamesPatch
    {
        private const string ROOT_PATH =
            "Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/ScrollRect/InputGridInnerGroup";
        private const string WINDOW_PATH = "Content/Content Text";
        private const string CALIB_PATH =
            "Content/InnerContent/LeftGroup/ScrollboxContainer/ScrollArea/Content";

        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            { "Space", "Пробіл" },
            { "Caps Lock", "Капс Лок" },
            { "Tab", "Таб" },
            { "ESC", "ЕСК" },
            { "Return", "Ентер" },
            { "Up Arrow", "Стрілка Вгору" },
            { "Down Arrow", "Стрілка Вниз" },
            { "Left Arrow", "Стрілка Вліво" },
            { "Right Arrow", "Стрілка Вправо" },
            { "Arrow", "Стрілка" },
            { "Control", "Контрол" },
            { "Alt", "Альт" },
            { "Left Command", "Ліва Команда" },
            { "Right Command", "Права Команда" },
            { "Delete", "Деліт" },
            { "Insert", "Інсерт" },
            { "Pause", "Пауза" },
            { "Home", "Хом" },
            { "End", "Енд" },
            { "Keypad", "Клавіатурна" },
            { "Backspace", "Бекспейс" },
            { "Numlock", "Намлок" },
            { "Back Quote", "Апостроф" },
            { "Left Mouse Button", "Ліва Клавіша Миші" },
            { "Right Mouse Button", "Права Клавіша Миші" },
            { "Mouse Button", "Клавіша Миші" },
            { "Mouse Button 3", "Клавіша Миші 3" },
            { "Mouse Wheel", "Коліщатко Миші" },
            { "Mouse", "Миша" },
            { "Wheel", "Коліщатко" },
            { "Horizontal", "По Горизонталі" },
            { "Vertical", "По Вертикалі" },
            { "Shoulder", "Плече" },
            { "Right Stick Button", "Кнопка Правого Стіку" },
            { "Left Stick Button", "Кнопка Лівого Стіку" },
            { "Stick", "Стік" },
            { "Trigger", "Тригер" },
            { "Start", "Старт" },
            { "Back", "Назад" },
            { "Button", "Кнопка" },
            { "Left", "Лівий" },
            { "Right", "Правий" },
            { "Up", "Вгору" },
            { "Down", "Вниз" },
            { "Shift", "Шифт" },
        };

        private static void TranslateRoot(string path)
        {
            var root = GameObject.Find(path);
            if (root == null)
                return;

            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (tmp != null && !string.IsNullOrWhiteSpace(tmp.text))
                    tmp.text = Translate(tmp.text);
            }
        }

        private static string Translate(string input)
        {
            string result = input;
            foreach (var pair in Map)
                result = result.Replace(pair.Key, pair.Value);
            return result;
        }

        [HarmonyPatch(typeof(Rewired.UI.ControlMapper.ControlMapper), "PopulateInputFields")]
        private static class ControlMapperPatch
        {
            private static void Postfix()
            {
                TranslateRoot(ROOT_PATH);
            }
        }

        [HarmonyPatch(typeof(Rewired.UI.ControlMapper.Window), "Enable")]
        private static class WindowPatch
        {
            private static void Postfix()
            {
                TranslateRoot(WINDOW_PATH);
            }
        }

        [HarmonyPatch(typeof(Rewired.UI.ControlMapper.CalibrationWindow), "RefreshControls")]
        private static class CalibrationPatch
        {
            private static void Postfix()
            {
                TranslateRoot(CALIB_PATH);
            }
        }
    }
}
