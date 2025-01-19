using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MagicWords.Core.Board;
using System.Text.RegularExpressions;
using MagicWords.Core.Config;
using MagicWords.Core.Interfaces;

namespace MagicWords.Core.Board
{
    public class WordValidator : MonoBehaviour, IWordValidator
    {
        private HashSet<string> dictionary;
        private HashSet<string> prefixes;
        private Dictionary<char, float> letterComplexity;
        private BoardConfig config;
        private GameConfig gameConfig;
        private Language currentLanguage;

        private void Awake()
        {
            InitializeLetterComplexity();
        }

        public void Initialize(BoardConfig boardConfig, GameConfig gameConfig)
        {
            this.config = boardConfig;
            this.gameConfig = gameConfig;

            var dataManager = ServiceLocator.Instance.Get<IDataManager>();
            currentLanguage = dataManager?.LoadData<GameSettings>("settings")?.Language ?? Language.English;

            LoadDictionary();
        }

        private void LoadDictionary()
        {
            dictionary = new HashSet<string>();
            prefixes = new HashSet<string>();

            string dictionaryPath = $"dictionary/{(currentLanguage == Language.English ? "en" : "es")}";
            var dictionaryText = Resources.Load<TextAsset>(dictionaryPath);

            if (dictionaryText != null)
            {
                string[] words = dictionaryText.text.Split(new[] { '\n', '\r' },
                    System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    string cleanWord = CleanWord(word);
                    if (IsValidDictionaryWord(cleanWord))
                    {
                        dictionary.Add(cleanWord);
                        GeneratePrefixes(cleanWord);
                    }
                }
            }
            else
            {
                Debug.LogError($"Dictionary file not found: {dictionaryPath}");
            }
        }

        private void GeneratePrefixes(string word)
        {
            for (int i = 1; i <= word.Length; i++)
            {
                prefixes.Add(word.Substring(0, i));
            }
        }

        private string CleanWord(string word)
        {
            // Remove diacritics and convert to uppercase
            string normalized = word.Normalize(System.Text.NormalizationForm.FormD);
            var cleaned = normalized.Where(c => System.Char.GetUnicodeCategory(c) !=
                System.Globalization.UnicodeCategory.NonSpacingMark);

            return new string(cleaned.ToArray()).ToUpper().Trim();
        }

        private bool IsValidDictionaryWord(string word)
        {
            // Exclude words that are too short
            if (word.Length < gameConfig.minWordLength) return false;

            // Exclude words with invalid characters
            if (!Regex.IsMatch(word, "^[A-ZÑÁÉÍÓÚ]+$")) return false;

            // Exclude proper nouns (words starting with capital letters)
            // Note: At this point all letters are uppercase, so this check should be done before ToUpper()

            return true;
        }

        public bool ValidateWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;

            string cleanWord = CleanWord(word);

            // Check minimum length
            if (cleanWord.Length < gameConfig.minWordLength) return false;

            // Check if word exists in dictionary
            return dictionary.Contains(cleanWord);
        }

        public bool IsPartialWordValid(string partialWord)
        {
            if (string.IsNullOrEmpty(partialWord)) return false;

            string cleanPartial = CleanWord(partialWord);
            return prefixes.Contains(cleanPartial);
        }

        private void InitializeLetterComplexity()
        {
            letterComplexity = new Dictionary<char, float>
            {
                // Common letters have lower complexity
                {'A', 0.2f}, {'E', 0.2f}, {'I', 0.2f}, {'O', 0.2f}, {'U', 0.2f},
                {'S', 0.3f}, {'N', 0.3f}, {'R', 0.3f}, {'L', 0.3f}, {'T', 0.3f},
                {'D', 0.4f}, {'C', 0.4f}, {'M', 0.4f}, {'P', 0.4f},
                
                // Less common letters have higher complexity
                {'B', 0.6f}, {'G', 0.6f}, {'V', 0.6f}, {'Y', 0.6f},
                {'Q', 0.7f}, {'H', 0.7f}, {'F', 0.7f}, {'Z', 0.8f},
                {'J', 0.8f}, {'Ñ', 0.8f}, {'X', 0.9f}, {'W', 1.0f}, {'K', 1.0f}
            };
        }

        public float GetWordScore(string word)
        {
            if (string.IsNullOrEmpty(word)) return 0f;

            string cleanWord = CleanWord(word);
            if (!dictionary.Contains(cleanWord)) return 0f;

            // Base score
            float score = gameConfig.baseWordScore;

            // Word length multiplier
            float lengthMultiplier = 1 + ((cleanWord.Length - gameConfig.minWordLength) *
                gameConfig.wordLengthMultiplier);

            // Letter complexity multiplier
            float complexitySum = cleanWord.Sum(c => letterComplexity.GetValueOrDefault(c, 0.5f));
            float averageComplexity = complexitySum / cleanWord.Length;
            float complexityMultiplier = 1 + (averageComplexity * gameConfig.letterComplexityMultiplier);

            return score * lengthMultiplier * complexityMultiplier;
        }
    }
}