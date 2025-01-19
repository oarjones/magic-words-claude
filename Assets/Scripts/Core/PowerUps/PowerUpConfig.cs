using UnityEditor;
using UnityEngine;

namespace MagicWords.Core.PowerUps
{
    [CreateAssetMenu(fileName = "PowerUpConfig", menuName = "MagicWords/Power Up Config")]
    public class PowerUpConfig : ScriptableObject
    {
        [Header("Basic Settings")]
        public PowerUpType type;
        public string displayName;
        public string description;
        public Sprite icon;

        [Header("Usage Settings")]
        public int initialUses = 3;
        public float cooldownTime = 30f;
        public float duration = 10f;

        [Header("Visual Settings")]
        public Color activeColor = Color.white;
        public Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);
        public Color cooldownColor = new Color(0.5f, 0.5f, 0.5f);

        [Header("Effect Settings")]
        public ParticleSystem activationEffect;
        public ParticleSystem deactivationEffect;
        public AudioClip activationSound;
        public AudioClip deactivationSound;
    }

    // Generator for default power-up configurations
#if UNITY_EDITOR
    public class DefaultPowerUpConfigs
    {
        [UnityEditor.MenuItem("MagicWords/Create Default PowerUp Configs")]
        public static void CreateDefaultConfigs()
        {
            CreatePowerUpConfig("ChangeCellPowerUp", PowerUpType.ChangeCell, 3, 30f, 0f);
            CreatePowerUpConfig("ChangeLetterPowerUp", PowerUpType.ChangeLetter, 3, 30f, 0f);
            CreatePowerUpConfig("FreezeTrapPowerUp", PowerUpType.FreezeTrap, 2, 45f, 10f);
        }

        private static void CreatePowerUpConfig(string name, PowerUpType type,
            int uses, float cooldown, float duration)
        {
            var config = ScriptableObject.CreateInstance<PowerUpConfig>();

            config.type = type;
            config.displayName = ObjectNames.NicifyVariableName(type.ToString());
            config.description = GetDefaultDescription(type);
            config.initialUses = uses;
            config.cooldownTime = cooldown;
            config.duration = duration;

            string path = $"Assets/Resources/Configs/PowerUps/{name}.asset";

            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            UnityEditor.AssetDatabase.CreateAsset(config, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        private static string GetDefaultDescription(PowerUpType type)
        {
            return type switch
            {
                PowerUpType.ChangeCell => "Move to any adjacent cell",
                PowerUpType.ChangeLetter => "Change the current cell's letter",
                PowerUpType.FreezeTrap => "Place a freeze trap that stops opponents",
                _ => "No description available"
            };
        }
    }
#endif
}