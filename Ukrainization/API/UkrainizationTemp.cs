using System.IO;
using BepInEx;
using UnityEngine;

namespace Ukrainization.API
{
    public static class UkrainizationTemp
    {
        public const string ModGUID = "Ukrainization";
        public const string ModName = "Baldi's Basics Classic Remastered Ukrainization";
        public const string ModVersion = "1.0.0";

        public static string OverwritesFile = "Overwrites.json";
        public static string PostersFile = "PosterSettings.json";
        public static string SubtitilesFile = "Subtitles_Ukrainian.json";

        public static string GetBasePath()
        {
            return Path.Combine(Paths.PluginPath, "Ukrainization", ModGUID);
        }

        public static string GetAudioPath()
        {
            return Path.Combine(GetBasePath(), "Audios");
        }

        public static string GetTexturePath()
        {
            return Path.Combine(GetBasePath(), "Textures");
        }

        public static string GetPostersPath()
        {
            return Path.Combine(GetBasePath(), "PosterFiles");
        }

        public static void UpdateClipData(SoundObject obj, AudioClip newClip)
        {
            if (obj != null && newClip != null)
            {
                if (obj.name == newClip.name)
                {
                    obj.soundClip = newClip;
                }
            }
        }
    }
}
