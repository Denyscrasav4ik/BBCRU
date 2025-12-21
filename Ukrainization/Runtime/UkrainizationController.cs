using Ukrainization.API;
using UnityEngine;

namespace Ukrainization.Runtime
{
    public class UkrainizationController : MonoBehaviour
    {
        private static UkrainizationController instance = null!;
        public AssetManager ModAssets = new AssetManager();
        public static UkrainizationController Instance => instance;

        public void Start()
        {
            instance = this;
            ModAssets = new AssetManager();
            DontDestroyOnLoad(gameObject);
        }

        public void Load()
        {
            Singleton<LocalizationManager>.Instance.LoadLocalizedText(
                "Subtitles_Ukrainian.json",
                default(Language)
            );
        }
    }
}
