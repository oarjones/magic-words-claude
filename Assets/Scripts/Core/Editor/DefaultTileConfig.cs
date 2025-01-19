using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using MagicWords.Core.Config;

namespace MagicWords.Core.Editor
{
    public class DefaultTileConfig
    {
        [MenuItem("MagicWords/Create Default Tile Config")]
        public static void CreateDefaultConfig()
        {
            var config = ScriptableObject.CreateInstance<TileVisualConfig>();

            // Set default values
            config.normalColor = Color.white;
            config.selectedColor = new Color(0.4f, 0.8f, 1f, 1f);
            config.frozenColor = new Color(0.8f, 0.8f, 1f, 1f);
            config.objectiveColor = new Color(1f, 0.8f, 0.8f, 1f);
            config.invalidColor = new Color(1f, 0.4f, 0.4f, 1f);

            config.textColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            config.textSize = 4f;
            config.textVerticalOffset = 0f;

            config.selectAnimationDuration = 0.3f;
            config.deselectAnimationDuration = 0.2f;
            config.invalidAnimationDuration = 0.5f;
            config.selectedScaleMultiplier = 1.2f;
            config.invalidAnimationIntensity = 0.2f;

            config.selectionParticleRate = 10f;
            config.freezeParticleRate = 15f;
            config.highlightParticleRate = 12f;

            config.backgroundSortingOrder = 0;
            config.letterSortingOrder = 1;
            config.particleSortingOrder = 2;

            // Create asset
            string configPath = "Assets/Resources/Configs";
            if (!System.IO.Directory.Exists(configPath))
            {
                System.IO.Directory.CreateDirectory(configPath);
            }

            AssetDatabase.CreateAsset(config, $"{configPath}/TileVisualConfig.asset");
            AssetDatabase.SaveAssets();

            Debug.Log("Default TileVisualConfig created");
        }
    }
}
#endif