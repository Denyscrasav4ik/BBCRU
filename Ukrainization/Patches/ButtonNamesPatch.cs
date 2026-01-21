using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ukrainization.Patches
{
    internal class ButtonNamesPatch : MonoBehaviour
    {
        private const string ROOT_PATH =
            "RewiredControlMapper/Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/ScrollRect/InputGridInnerGroup/";
        private const string ROOT_PATH_CLONE =
            "CoreGameManager(Clone)/PauseMenuScreens/RewiredControlMapper/Canvas/MainPageGroup/MainContent/MainContentInner/InputGridGroup/InputGridContainer/Container/ScrollRect/InputGridInnerGroup/";
        private const string WINDOW_PATH =
            "RewiredControlMapper/Canvas/Window/Content/Content Text";
        private const string PLAYTIME_PATH = "TextCanvas";
        private const string CALIB_PATH =
            "RewiredControlMapper/Canvas/CalibrationWindow/Content/InnerContent/LeftGroup/ScrollboxContainer/ScrollArea/Content";
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
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
            { "Left Mouse Button", " Ліва Клавіша Миші" },
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
        private readonly Dictionary<TextMeshProUGUI, string> _textCache =
            new Dictionary<TextMeshProUGUI, string>();
        private bool _rootWasPresent;
        private bool _cloneWasPresent;
        private bool _windowLastExists;
        private bool _calibWasPresent;

        private void Update()
        {
            if (CheckFirstAppearance(ROOT_PATH, ref _rootWasPresent))
                return;
            if (DetectTextChange(ROOT_PATH))
                return;
            if (CheckFirstAppearance(ROOT_PATH_CLONE, ref _cloneWasPresent))
                return;
            if (DetectTextChange(ROOT_PATH_CLONE))
                return;
            if (CheckFirstAppearance(CALIB_PATH, ref _calibWasPresent))
                return;
            if (DetectTextChange(CALIB_PATH))
                return;
            var window = GameObject.Find(WINDOW_PATH);
            bool windowExists = window != null;
            if (windowExists != _windowLastExists)
            {
                _windowLastExists = windowExists;
                RerunAll();
                return;
            }
            TranslateDynamicObject(PLAYTIME_PATH);
            if (window != null)
                TranslateDynamicObject(WINDOW_PATH);
            TranslateDynamicObject(CALIB_PATH);
        }

        private bool CheckFirstAppearance(string rootPath, ref bool wasPresent)
        {
            var root = GameObject.Find(rootPath);
            bool presentNow = root != null && root.activeInHierarchy;
            if (presentNow && !wasPresent)
            {
                wasPresent = true;
                TranslateMainGrid(rootPath == ROOT_PATH_CLONE);
                CacheTexts(root);
                return true;
            }
            wasPresent = presentNow;
            return false;
        }

        private bool DetectTextChange(string rootPath)
        {
            var root = GameObject.Find(rootPath);
            if (root == null)
                return false;
            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (tmp == null)
                    continue;
                var text = tmp.text ?? string.Empty;
                if (_textCache.TryGetValue(tmp, out var last))
                {
                    if (!string.Equals(text, last))
                    {
                        _textCache[tmp] = text;
                        TranslateMainGrid(rootPath == ROOT_PATH_CLONE);
                        return true;
                    }
                }
                else
                {
                    _textCache[tmp] = text;
                }
            }
            return false;
        }

        private void CacheTexts(GameObject root)
        {
            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
                if (tmp != null)
                    _textCache[tmp] = tmp.text ?? string.Empty;
        }

        private void RerunAll()
        {
            TranslateMainGrid(false);
            TranslateMainGrid(true);
            TranslateDynamicObject(CALIB_PATH);
        }

        private void TranslateMainGrid(bool isClone)
        {
            var root = GameObject.Find(isClone ? ROOT_PATH_CLONE : ROOT_PATH);
            if (root == null)
                return;
            ProcessRoot(root);
        }

        private void TranslateDynamicObject(string path)
        {
            var obj = GameObject.Find(path);
            if (obj != null)
                ProcessRoot(obj);
        }

        private void ProcessRoot(GameObject root)
        {
            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
                Apply(tmp);
            foreach (var text in root.GetComponentsInChildren<Text>(true))
                Apply(text);
        }

        private void Apply(TextMeshProUGUI tmp)
        {
            if (tmp == null || string.IsNullOrWhiteSpace(tmp.text))
                return;
            tmp.text = Translate(tmp.text);
        }

        private void Apply(Text text)
        {
            if (text == null || string.IsNullOrWhiteSpace(text.text))
                return;
            text.text = Translate(text.text);
        }

        private string Translate(string input)
        {
            string result = input;

            foreach (var pair in Map)
            {
                result = result.Replace(pair.Key, pair.Value);
            }

            return result;
        }
    }
}
