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
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
        {
            { "Space", "Пробіл" },
            { "Caps Lock", "Замок Капсу" },
            { "Tab", "Таб" },
            { "ESC", "ЕСК" },
            { "Return", "Ентер" },
            { "Left Shift", "Лівий Шифт" },
            { "Right Shift", "Правий Шифт" },
            { "Left Control", "Лівий Контрол" },
            { "Right Control", "Правий Контрол" },
            { "Left Alt", "Лівий Альт" },
            { "Right Alt", "Правий Альт" },
            { "Up Arrow", "Стрілка Вгору" },
            { "Down Arrow", "Стрілка Вниз" },
            { "Left Arrow", "Стрілка Вліво" },
            { "Right Arrow", "Стрілка Вправо" },
            { "Left Mouse Button", "Ліва Клавіша Миші" },
            { "Right Mouse Button", "Права Клавіша Миші" },
            { "Mouse Button 3", "Клавіша Миші 3" },
            { "Mouse Wheel", "Коліщатко Миші" },
            { "Mouse Wheel Up", "Коліщатко Миші Вверх" },
            { "Mouse Wheel Down", "Коліщатко Миші Вниз" },
            { "Mouse Horizontal", "Миша По Горизонталі" },
            { "Mouse Vertical", "Миша По Вертикалі" },
            { "Mouse Up", "Миша Вверх" },
            { "Mouse Down", "Миша Вниз" },
            { "Mouse Left", "Миша Вліво" },
            { "Mouse Right", "Миша Вправо" },
            { "Left Shoulder", "Ліве Плече" },
            { "Right Shoulder", "Праве Плече" },
            { "Right Stick Button", "Кнопка Правого Стіку" },
            { "Left Stick Button", "Кнопка Лівого Стіку" },
            { "Right Stick X", "Правий Стік X" },
            { "Left Stick X", "Лівий Стік X" },
            { "Right Stick Y", "Правий Стік Y" },
            { "Left Stick Y", "Лівий Стік Y" },    
            { "Left Trigger", "Лівий Тригер" },
            { "Right Trigger", "Правий Тригер" },
            { "Start", "Старт" },
            { "Back", "Назад" },
        };
        private readonly Dictionary<TextMeshProUGUI, string> _textCache =
            new Dictionary<TextMeshProUGUI, string>();
        private bool _rootWasPresent;
        private bool _cloneWasPresent;
        private bool _windowLastExists;
        private static readonly Regex UkrainianRegex = new Regex(
            @"[\u0400-\u04FF]",
            RegexOptions.Compiled
        );

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
            var window = GameObject.Find(WINDOW_PATH);
            bool windowExists = window != null;
            if (windowExists != _windowLastExists)
            {
                _windowLastExists = windowExists;
                RerunAll();
                return;
            }
            TranslateDynamicObject(PLAYTIME_PATH, ignoreCyrillic: true);
            if (window != null)
                TranslateDynamicObject(WINDOW_PATH);
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
        }

        private void TranslateMainGrid(bool isClone)
        {
            var root = GameObject.Find(isClone ? ROOT_PATH_CLONE : ROOT_PATH);
            if (root == null)
                return;
            ProcessRoot(root);
        }

        private void TranslateDynamicObject(string path, bool ignoreCyrillic = false)
        {
            var obj = GameObject.Find(path);
            if (obj != null)
                ProcessRoot(obj, ignoreCyrillic);
        }

        private void ProcessRoot(GameObject root, bool ignoreCyrillic = false)
        {
            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
                Apply(tmp, ignoreCyrillic);

            foreach (var text in root.GetComponentsInChildren<Text>(true))
                Apply(text, ignoreCyrillic);
        }

        private void Apply(TextMeshProUGUI tmp, bool ignoreCyrillic = false)
        {
            if (tmp == null || string.IsNullOrWhiteSpace(tmp.text))
                return;

            if (!ignoreCyrillic && UkrainianRegex.IsMatch(tmp.text))
                return;

            tmp.text = Translate(tmp.text);
        }

        private void Apply(Text text, bool ignoreCyrillic = false)
        {
            if (text == null || string.IsNullOrWhiteSpace(text.text))
                return;

            if (!ignoreCyrillic && UkrainianRegex.IsMatch(text.text))
                return;

            text.text = Translate(text.text);
        }

        private string Translate(string input)
        {
            var result = input;
            foreach (var pair in Map)
            {
                var pattern = @"\b" + Regex.Escape(pair.Key) + @"\b";
                result = Regex.Replace(result, pattern, pair.Value);
            }
            return result;
        }
    }
}
