using UnityEngine;
using System;
using System.Collections.Generic;

namespace MagicWords.Core.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MagicWords/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Game Settings")]
        public float gameRoundDuration = 300f;
        public int minWordLength = 3;
        public float tileSelectionTimeout = 20f;

        [Header("Scoring")]
        public int baseWordScore = 100;
        public float letterComplexityMultiplier = 1.5f;
        public float wordLengthMultiplier = 1.2f;

        [Header("Power-Ups")]
        public PowerUpConfig[] powerUpConfigs;
    }

    [CreateAssetMenu(fileName = "BoardConfig", menuName = "MagicWords/Board Config")]
    public class BoardConfig : ScriptableObject
    {
        [Header("Board Generation")]
        public int boardSize = 4;
        public float letterChangeInterval = 0f;
        public float letterComplexity = 0.5f;

        [Header("Visual Settings")]
        public float tileScale = 1f;
        public float selectedTileScale = 1.2f;
        public float tileSpacing = 0.1f;

        [Header("Animation")]
        public float tileSelectAnimationDuration = 0.3f;
        public float tileDeselectAnimationDuration = 0.2f;
        public float invalidWordShakeDuration = 0.5f;
    }

    [Serializable]
    public class PowerUpConfig
    {
        public PowerUpType type;
        public string displayName;
        public string description;
        public Sprite icon;
        public int initialCount;
        public float cooldown;
        public float duration;
        public float effectValue;
    }

    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "MagicWords/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        public List<DifficultyLevel> levels;
    }

    [Serializable]
    public class DifficultyLevel
    {
        public GameDifficulty difficulty;
        public float timeMultiplier = 1f;
        public float scoreMultiplier = 1f;
        public int minWordLength = 3;
        public float letterComplexity = 0.5f;
        public bool allowPowerUps = true;
        public int maxPowerUpsPerGame = 3;
    }

    [CreateAssetMenu(fileName = "LocalizationConfig", menuName = "MagicWords/Localization Config")]
    public class LocalizationConfig : ScriptableObject
    {
        public Language defaultLanguage = Language.English;
        public TextAsset[] dictionaryFiles;
        public LocalizedText[] localizedTexts;
    }

    [Serializable]
    public class LocalizedText
    {
        public string key;
        public string englishText;
        public string spanishText;

        public string GetText(Language language)
        {
            return language == Language.English ? englishText : spanishText;
        }
    }
}