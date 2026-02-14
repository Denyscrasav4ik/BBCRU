using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ukrainization.Patches
{
    [HarmonyPatch(typeof(InputManager))]
    [HarmonyPatch(
        "GetInputButtonName",
        new Type[] { typeof(string), typeof(bool), typeof(string[]) }
    )]
    internal static class InputManagerPatch
    {
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

        static void Postfix(ref string __result)
        {
            if (string.IsNullOrEmpty(__result))
                return;

            foreach (var pair in Map)
            {
                __result = __result.Replace(pair.Key, pair.Value);
            }
        }
    }
}
