using BepInEx.Configuration;
using BepInEx.Logging;
using Ukrainization.API;

namespace Ukrainization.API
{
    public static class ConfigManager
    {
        public static ConfigEntry<bool> EnableTextures { get; private set; } = null!;
        public static ConfigEntry<bool> EnableSounds { get; private set; } = null!;
        public static ConfigEntry<bool> EnableLogging { get; private set; } = null!;
        private static ManualLogSource _logger = null!;

        public static void Initialize(BepInEx.BaseUnityPlugin plugin, ManualLogSource logger)
        {
            _logger = logger;

            EnableTextures = plugin.Config.Bind(
                "Main",
                "EnableTextures",
                true,
                "Увімкнути заміну текстур"
            );

            EnableSounds = plugin.Config.Bind(
                "Main",
                "EnableSounds",
                true,
                "Увімкнути заміну звуків"
            );

            EnableLogging = plugin.Config.Bind(
                "Main",
                "EnableLogging",
                false,
                "Увімкнути логування"
            );

            _logger.LogInfo(
                $"Конфігурацію завантажено. Текстури: {EnableTextures.Value}, Звуки: {EnableSounds.Value}, Логування: {EnableLogging.Value}"
            );
        }

        public static bool AreTexturesEnabled()
        {
            return EnableTextures?.Value ?? false;
        }

        public static bool AreSoundsEnabled()
        {
            return EnableSounds?.Value ?? false;
        }

        public static bool IsLoggingEnabled()
        {
            return EnableLogging?.Value ?? true;
        }
    }
}
