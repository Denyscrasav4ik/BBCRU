using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TMPro;
using Ukrainization.API;
using UnityEngine;
using UnityEngine.UI;
using Logger = Ukrainization.API.Logger;

namespace Ukrainization.Patches
{
    [HarmonyPatch(typeof(ClassicBasementManager), "BeginPlay", new System.Type[] { })]
    internal class ClassicBedroomPatch
    {
        private static Texture2D? sheetTexture = null;
        private static Sprite[] ukrainianLetters = new Sprite[33];
        private static bool isInitialized = false;
        private static bool prefabChangesDone = false;

        // Дані спрайт-листа українського алфавіту
        private static readonly string[] AlphabetSheetData = new string[]
        {
            "А;61;366;65;43",
            "Б;131;367;58;45",
            "В;196;367;56;44",
            "Г;259;367;55;43",
            "Ґ;320;367;56;50",
            "Д;379;359;72;52",
            "Е;61;313;61;46",
            "Є;126;314;57;43",
            "Ж;187;314;83;47",
            "З;274;314;53;42",
            "И;334;314;69;44",
            "І;410;314;41;42",
            "Ї;44;258;46;52",
            "Й;92;257;69;56",
            "К;171;258;56;43",
            "Л;229;257;73;43",
            "М;309;257;86;44",
            "Н;401;256;67;45",
            "О;50;211;73;43",
            "П;135;212;73;43",
            "Р;216;197;52;58",
            "С;274;211;57;43",
            "Т;337;211;59;43",
            "У;399;197;63;57",
            "Ф;13;134;76;57",
            "Х;93;148;70;44",
            "Ц;168;142;73;50",
            "Ч;246;149;58;43",
            "Ш;311;149;88;43",
            "Щ;405;141;94;51",
            "Ь;141;96;54;42",
            "Ю;200;95;108;44",
            "Я;312;96;59;43",
        };

        [HarmonyPostfix]
        private static void Postfix(ClassicBasementManager __instance)
        {
            if (!ConfigManager.AreTexturesEnabled())
            {
                // Logger.Info("Заміна букв у спальні вимкнена у конфігурації");
                return;
            }

            try
            {
                if (!isInitialized)
                {
                    LoadUkrainianAlphabet();
                    isInitialized = true;
                }

                if (sheetTexture == null || ukrainianLetters[0] == null)
                {
                    // Logger.Warning("Український алфавіт не завантажено, пропускаємо заміну букв");
                    return;
                }

                RoomController bedroom = __instance.Ec.rooms[6];
                Transform wallLetters = bedroom.objectObject.transform.Find("WallLetters(Clone)");

                if (wallLetters == null)
                {
                    // Logger.Warning("WallLetters(Clone) не знайдено у спальні");
                    return;
                }

                SpriteRenderer[] englishABCSorted =
                    wallLetters.GetComponentsInChildren<SpriteRenderer>();
                Array.Sort(englishABCSorted, (x, y) => string.Compare(x.name, y.name));

                // Logger.Info($"Знайдено {englishABCSorted.Length} англійських букв, замінюємо на українські");

                for (int i = 0; i < englishABCSorted.Length && i < ukrainianLetters.Length; i++)
                {
                    if (ukrainianLetters[i] != null)
                    {
                        englishABCSorted[i].sprite = ukrainianLetters[i];
                    }
                }

                SpriteRenderer[] additionalLetters = new SpriteRenderer[7];
                for (int i = 0; i < additionalLetters.Length; i++)
                {
                    additionalLetters[i] = UnityEngine.Object.Instantiate(
                        englishABCSorted[englishABCSorted.Length - 1]
                    );
                    additionalLetters[i]
                        .transform.SetParent(
                            englishABCSorted[englishABCSorted.Length - 1].transform.parent
                        );
                    additionalLetters[i].sprite = ukrainianLetters[26 + i];
                    additionalLetters[i].name = new string(
                        additionalLetters[i].sprite.name[
                            additionalLetters[i].sprite.name.Length - 1
                        ],
                        1
                    );
                }

                SetupLetter(
                    additionalLetters[5],
                    new Vector3(2f, 2.2727f, 14.95f),
                    new Vector3(0f, 0f, 11.1309f),
                    new Color(1f, 0f, 0f, 1f)
                ); // Щ
                SetupLetter(
                    additionalLetters[1],
                    new Vector3(5f, 3f, 14.95f),
                    Vector3.zero,
                    new Color(0f, 1f, 1f, 1f)
                ); // Я
                SetupLetter(
                    additionalLetters[3],
                    new Vector3(7.6253f, 2.6436f, 14.95f),
                    Vector3.zero,
                    new Color(0.64f, 0.4976f, 0.766f, 1f)
                ); // Ь
                SetupLetter(
                    additionalLetters[0],
                    new Vector3(10f, 3f, 14.95f),
                    Vector3.zero,
                    new Color(0.4137f, 0.7f, 1f, 1f)
                ); // Ю
                SetupLetter(
                    additionalLetters[6],
                    new Vector3(12.2546f, 2.3164f, 14.95f),
                    new Vector3(0f, 0f, 335.6689f),
                    new Color(0.1137f, 0.1176f, 0.6906f, 1f)
                ); // Ъ
                SetupLetter(
                    additionalLetters[2],
                    new Vector3(15f, 3f, 14.95f),
                    new Vector3(0f, 0f, 348.052f),
                    new Color(0.5937f, 0.3176f, 0.2f, 1f)
                ); // Ы
                SetupLetter(
                    additionalLetters[4],
                    new Vector3(18.3128f, 2.3818f, 14.95f),
                    new Vector3(0f, 0f, 11.42f),
                    new Color(0.54f, 0f, 0.472f, 1f)
                ); // Ю

                // Logger.Info("Українські букви успішно розміщено у спальні Балді");
            }
            catch (Exception)
            {
                // Logger.Error($"Помилка при заміні букв у спальні");
            }

            if (!prefabChangesDone)
            {
                try
                {
                    ApplyBookPrefabLocalization(__instance);
                    prefabChangesDone = true;
                }
                catch (Exception)
                {
                    // Logger.Error($"Помилка при модифікації префабів книг");
                }
            }
        }

        private static void LoadUkrainianAlphabet()
        {
            try
            {
                string texturePath = Path.Combine(
                    UkrainizationTemp.GetTexturePath(),
                    "Alphabet_Ua_Sheet.png"
                );
                sheetTexture = AssetLoader.TextureFromFile(texturePath, TextureFormat.ARGB32);

                if (sheetTexture == null)
                {
                    // Logger.Error($"Не вдалося завантажити Alphabet_Ua_Sheet.png з {texturePath}");
                    // Logger.Info("Помістіть файл Alphabet_Ua_Sheet.png у папку Textures");
                    return;
                }

                sheetTexture.filterMode = FilterMode.Point;
                sheetTexture.Apply();
                sheetTexture.name = "Alphabet_Ua_Sheet";

                // Logger.Info($"Текстура українського алфавіту завантажена: {sheetTexture.width}x{sheetTexture.height}");
                // Logger.Info($"Обробляємо {AlphabetSheetData.Length} букв українського алфавіту");

                for (int i = 0; i < AlphabetSheetData.Length && i < ukrainianLetters.Length; i++)
                {
                    string line = AlphabetSheetData[i];

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] splits = line.Split(new char[] { ';' });

                    if (splits.Length < 5)
                    {
                        // Logger.Warning($"Некоректний рядок даних: {line}");
                        continue;
                    }

                    string letterName = splits[0];
                    int x = int.Parse(splits[1]);
                    int y = int.Parse(splits[2]);
                    int width = int.Parse(splits[3]);
                    int height = int.Parse(splits[4]);

                    Sprite ukrainianLetter = Sprite.Create(
                        sheetTexture,
                        new Rect(x, y, width, height),
                        new Vector2(0.5f, 0.5f),
                        40f
                    );
                    ukrainianLetter.name = $"Alphabet_Ua_{letterName}";
                    ukrainianLetters[i] = ukrainianLetter;
                }

                // Logger.Info($"Створено {ukrainianLetters.Length} спрайтів українських букв");
            }
            catch (Exception)
            {
                // Logger.Error("Помилка при завантаженні українського алфавіту");
            }
        }

        private static void SetupLetter(
            SpriteRenderer letter,
            Vector3 localPosition,
            Vector3 localEuler,
            Color color
        )
        {
            letter.transform.localPosition = localPosition;
            letter.transform.localEulerAngles = localEuler;
            letter.color = color;
        }

        private static void ApplyBookPrefabLocalization(ClassicBasementManager instance)
        {
            try
            {
                Type cmpType = instance.GetType();
                FieldInfo bookPreField = cmpType.GetField(
                    "bookPre",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

                if (bookPreField == null)
                {
                    // Logger.Warning("Поле bookPre не знайдено у ClassicBasementManager");
                    return;
                }

                ClassicMathBook[]? bookPre = bookPreField.GetValue(instance) as ClassicMathBook[];

                if (bookPre == null || bookPre.Length == 0)
                {
                    // Logger.Warning("Масив bookPre порожній або null");
                    return;
                }

                // Logger.Info($"Знайдено {bookPre.Length} префабів книг, застосовуємо українізацію");

                for (int i = 0; i < bookPre.Length; i++)
                {
                    if (bookPre[i] != null)
                    {
                        ApplyLocalizationToBookPrefab(bookPre[i]);
                    }
                }

                // Logger.Info("Українізація застосована до всіх префабів книг");
            }
            catch (Exception)
            {
                // Logger.Error("Помилка при застосуванні українізації до префабів книг");
            }
        }

        private static void ApplyLocalizationToBookPrefab(ClassicMathBook book)
        {
            try
            {
                Transform canvasChild = book.transform.GetChild(0);
                if (canvasChild == null)
                    return;

                Transform coverText = canvasChild.Find("CoverText");
                if (coverText == null)
                    return;

                Transform titleTransform = coverText.Find("TitleTMP");
                if (titleTransform != null)
                {
                    ApplyTextLocalizer(titleTransform, "Ukr_ClassicBook_Title", 26f);
                }

                Transform subTransform = coverText.Find("SubTMP");
                if (subTransform != null)
                {
                    string bookIdentifier = GetBookIdentifierFromName(book.name);
                    ApplyTextLocalizer(subTransform, $"Ukr_ClassicBook_{bookIdentifier}");
                }

                Transform authorTransform = coverText.Find("AuthorTMP");
                if (authorTransform != null)
                {
                    ApplyTextLocalizer(authorTransform, "Ukr_ClassicBook_Author");
                }

                Transform insideText = canvasChild.Find("InsideText");
                if (insideText != null)
                {
                    TMP_Text[] insideTexts = insideText.GetComponentsInChildren<TMP_Text>();
                    for (int i = 0; i < insideTexts.Length && i <= 5; i++)
                    {
                        ApplyTextLocalizer(insideTexts[i].transform, $"Ukr_ClassicBook_Text_{i}");
                    }
                }

                // Logger.Info($"Українізація застосована до книги: {book.name}");
            }
            catch (Exception)
            {
                // Logger.Error($"Помилка при українізації книги {book.name}");
            }
        }

        private static void ApplyTextLocalizer(
            Transform textTransform,
            string localizationKey,
            float? fontSize = null
        )
        {
            if (textTransform == null)
                return;

            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent == null)
                return;

            TextLocalizer existingLocalizer = textComponent.GetComponent<TextLocalizer>();
            if (existingLocalizer != null)
            {
                // Logger.Info($"TextLocalizer вже існує для {textTransform.name}");
                return;
            }

            TextLocalizer localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
            localizer.key = localizationKey;

            if (fontSize.HasValue)
            {
                textComponent.fontSize = fontSize.Value;
            }

            localizer.RefreshLocalization();

            // Logger.Info($"TextLocalizer додано: {textTransform.name} -> {localizationKey}");
        }

        private static string GetBookIdentifierFromName(string bookName)
        {
            if (bookName.Contains("Preschool"))
                return "Preschool";
            if (bookName.Contains("Kindergarten"))
                return "Kindergarten";
            if (bookName.Contains("College"))
                return "College";
            if (bookName.Contains("ClassicBook_10"))
                return "10";
            if (bookName.Contains("ClassicBook_11"))
                return "11";
            if (bookName.Contains("ClassicBook_12"))
                return "12";
            if (bookName.Contains("ClassicBook_1"))
                return "1";
            if (bookName.Contains("ClassicBook_2"))
                return "2";
            if (bookName.Contains("ClassicBook_3"))
                return "3";
            if (bookName.Contains("ClassicBook_4"))
                return "4";
            if (bookName.Contains("ClassicBook_5"))
                return "5";
            if (bookName.Contains("ClassicBook_6"))
                return "6";
            if (bookName.Contains("ClassicBook_7"))
                return "7";
            if (bookName.Contains("ClassicBook_8"))
                return "8";
            if (bookName.Contains("ClassicBook_9"))
                return "9";

            return "Default";
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                prefabChangesDone = false;
            }
        }
    }
}
