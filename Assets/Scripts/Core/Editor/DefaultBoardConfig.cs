using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using MagicWords.Core.Config;

namespace MagicWords.Core.Editor
{
    public class DefaultBoardConfig
    {
        [MenuItem("MagicWords/Create Default Board Config")]
        public static void CreateDefaultConfig()
        {
            var config = ScriptableObject.CreateInstance<BoardVisualConfig>();

            // Set default values
            config.tileSpacing = 1.1f;
            config.tileScale = 1f;
            config.boardScale = 1f;

            config.backgroundColor = new Color(0.95f, 0.95f, 0.95f);
            config.backgroundPadding = 0.5f;
            config.backgroundRoundness = 0.1f;

            config.boardAppearDuration = 1f;
            config.tileAppearStagger = 0.05f;
            config.initialTileScale = 0.1f;
            config.wordLineWidth = 0.1f;
            config.wordLineColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);

            config.validWordColor = new Color(0.4f, 1f, 0.4f);
            config.invalidWordColor = new Color(1f, 0.4f, 0.4f);
            config.wordEffectDuration = 1f;

            config.defaultZoom = 5f;
            config.minZoom = 3f;
            config.maxZoom = 8f;
            config.panSpeed = 5f;
            config.zoomSpeed = 0.5f;

            config.boardBackgroundOrder = -1;
            config.tileBaseOrder = 0;
            config.wordLineOrder = 1;
            config.effectsOrder = 2;

            // Create asset
            string configPath = "Assets/Resources/Configs";
            if (!System.IO.Directory.Exists(configPath))
            {
                System.IO.Directory.CreateDirectory(configPath);
            }

            AssetDatabase.CreateAsset(config, $"{configPath}/BoardVisualConfig.asset");
            AssetDatabase.SaveAssets();

            Debug.Log("Default BoardVisualConfig created");
        }
    }
}
#endif