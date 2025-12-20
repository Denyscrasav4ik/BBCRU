using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Rewired.UI.ControlMapper;

namespace Ukrainization.Patches
{
    [HarmonyPatch]
    public static class LanguageDataPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LanguageData), nameof(LanguageData.Initialize))]
        public static void LanguageData_Initialize_Postfix(LanguageData __instance)
        {
            __instance.SetPrivateField("_yes", "Так");
            __instance.SetPrivateField("_no", "Ні");
            __instance.SetPrivateField("_add", "Додати");
            __instance.SetPrivateField("_replace", "Замінити");
            __instance.SetPrivateField("_remove", "Видалити");
            __instance.SetPrivateField("_swap", "Поміняти");
            __instance.SetPrivateField("_cancel", "Скасувати");
            __instance.SetPrivateField("_none", "Нічого");
            __instance.SetPrivateField("_okay", "Гаразд");
            __instance.SetPrivateField("_done", "Готово");
            __instance.SetPrivateField("_default", "За замовчуванням");
            __instance.SetPrivateField("_assignControllerWindowTitle", "Вибір контролера");
            __instance.SetPrivateField(
                "_assignControllerWindowMessage",
                "Натисніть будь-яку кнопку або рухайте вісі для призначення контролера."
            );
            __instance.SetPrivateField(
                "_controllerAssignmentConflictWindowTitle",
                "Конфлікт призначення контролера"
            );
            __instance.SetPrivateField(
                "_controllerAssignmentConflictWindowMessage",
                "{0} вже призначено {1}. Хочете призначити його для {2}?"
            );
            __instance.SetPrivateField(
                "_elementAssignmentPrePollingWindowMessage",
                "Спочатку відцентруйте або обнуліть всі стіки та вісі і натисніть будь-яку кнопку або дочекайтесь завершення таймера."
            );
            __instance.SetPrivateField(
                "_joystickElementAssignmentPollingWindowMessage",
                "Тепер натисніть кнопку або рухайте вісь для призначення до {0}."
            );
            __instance.SetPrivateField(
                "_joystickElementAssignmentPollingWindowMessage_fullAxisFieldOnly",
                "Тепер рухайте вісь для призначення до {0}."
            );
            __instance.SetPrivateField(
                "_keyboardElementAssignmentPollingWindowMessage",
                "Натисніть клавішу для призначення до {0}. Модифікатори можна також використовувати. Щоб призначити лише модифікатор, утримуйте його 1 секунду."
            );
            __instance.SetPrivateField(
                "_mouseElementAssignmentPollingWindowMessage",
                "Натисніть кнопку миші або рухайте вісь для призначення до {0}."
            );
            __instance.SetPrivateField(
                "_mouseElementAssignmentPollingWindowMessage_fullAxisFieldOnly",
                "Рухайте вісь для призначення до {0}."
            );
            __instance.SetPrivateField(
                "_elementAssignmentConflictWindowMessage",
                "Конфлікт призначення"
            );
            __instance.SetPrivateField(
                "_elementAlreadyInUseBlocked",
                "{0} вже використовується, замінити не можна."
            );
            __instance.SetPrivateField(
                "_elementAlreadyInUseCanReplace",
                "{0} вже використовується. Хочете замінити?"
            );
            __instance.SetPrivateField(
                "_elementAlreadyInUseCanReplace_conflictAllowed",
                "{0} вже використовується. Замінити або додати призначення?"
            );
            __instance.SetPrivateField("_mouseAssignmentConflictWindowTitle", "Призначення миші");
            __instance.SetPrivateField(
                "_mouseAssignmentConflictWindowMessage",
                "Миша вже призначена {0}. Хочете призначити її {1}?"
            );
            __instance.SetPrivateField(
                "_calibrateControllerWindowTitle",
                "Калібрування контролера"
            );
            __instance.SetPrivateField("_calibrateAxisStep1WindowTitle", "Калібрування нуля");
            __instance.SetPrivateField(
                "_calibrateAxisStep1WindowMessage",
                "Встановіть {0} в центр або нуль і натисніть кнопку або дочекайтесь таймера."
            );
            __instance.SetPrivateField("_calibrateAxisStep2WindowTitle", "Калібрування діапазону");
            __instance.SetPrivateField(
                "_calibrateAxisStep2WindowMessage",
                "Пройдіть весь діапазон {0} і натисніть кнопку або дочекайтесь таймера."
            );
            __instance.SetPrivateField(
                "_inputBehaviorSettingsWindowTitle",
                "Налаштування чутливості"
            );
            __instance.SetPrivateField("_restoreDefaultsWindowTitle", "Відновити за замовчуванням");
            __instance.SetPrivateField(
                "_restoreDefaultsWindowMessage_onePlayer",
                "Це відновить конфігурацію вводу за замовчуванням. Ви впевнені?"
            );
            __instance.SetPrivateField(
                "_restoreDefaultsWindowMessage_multiPlayer",
                "Це відновить конфігурацію вводу за замовчуванням для всіх гравців. Ви впевнені?"
            );
            __instance.SetPrivateField("_actionColumnLabel", "Дії");
            __instance.SetPrivateField("_keyboardColumnLabel", "Клавіатура");
            __instance.SetPrivateField("_mouseColumnLabel", "Миша");
            __instance.SetPrivateField("_controllerColumnLabel", "Контролер");
            __instance.SetPrivateField("_removeControllerButtonLabel", "Видалити");
            __instance.SetPrivateField("_calibrateControllerButtonLabel", "Калібрувати");
            __instance.SetPrivateField("_assignControllerButtonLabel", "Призначити контролер");
            __instance.SetPrivateField("_inputBehaviorSettingsButtonLabel", "Чутливість");
            __instance.SetPrivateField("_doneButtonLabel", "Готово");
            __instance.SetPrivateField("_restoreDefaultsButtonLabel", "Відновити за замовчуванням");
            __instance.SetPrivateField("_playersGroupLabel", "Гравці:");
            __instance.SetPrivateField("_controllerSettingsGroupLabel", "Контролер:");
            __instance.SetPrivateField("_assignedControllersGroupLabel", "Призначені контролери:");
            __instance.SetPrivateField("_settingsGroupLabel", "Налаштування:");
            __instance.SetPrivateField("_mapCategoriesGroupLabel", "Категорії:");
            __instance.SetPrivateField("_calibrateWindow_deadZoneSliderLabel", "Мертва зона:");
            __instance.SetPrivateField("_calibrateWindow_zeroSliderLabel", "Нуль:");
            __instance.SetPrivateField("_calibrateWindow_sensitivitySliderLabel", "Чутливість:");
            __instance.SetPrivateField("_calibrateWindow_invertToggleLabel", "Інвертувати");
            __instance.SetPrivateField("_calibrateWindow_calibrateButtonLabel", "Калібрувати");

            var modifierKeysType = typeof(LanguageData).GetNestedType(
                "ModifierKeys",
                BindingFlags.NonPublic
            );
            var modifierKeysInstance = Activator.CreateInstance(modifierKeysType);
            modifierKeysType.GetField("control").SetValue(modifierKeysInstance, "Контрол");
            modifierKeysType.GetField("alt").SetValue(modifierKeysInstance, "Альт");
            modifierKeysType.GetField("shift").SetValue(modifierKeysInstance, "Шифт");
            modifierKeysType.GetField("command").SetValue(modifierKeysInstance, "Команда");
            modifierKeysType.GetField("separator").SetValue(modifierKeysInstance, " + ");
            __instance.SetPrivateField("_modifierKeys", modifierKeysInstance);

            __instance.SetPrivateField("_initialized", true);
        }

        public static void SetPrivateField(this object obj, string fieldName, object value)
        {
            var field = obj.GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(obj, value);
        }
    }
}
