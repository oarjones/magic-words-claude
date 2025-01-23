// WordDetector.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicWords.Core.Board
{
    public class WordDetector
    {
        private readonly IWordValidator wordValidator;
        private readonly List<ITile> selectedTiles = new();

        public event Action<string, int> OnValidWordFound;
        public event Action<string> OnInvalidWordFound;

        public WordDetector(IWordValidator wordValidator)
        {
            this.wordValidator = wordValidator;
        }

        public void AddTile(ITile tile)
        {
            selectedTiles.Add(tile);
            ValidateCurrentWord();
        }

        public void RemoveTile(ITile tile)
        {
            selectedTiles.Remove(tile);
        }

        public void Clear()
        {
            selectedTiles.Clear();
        }

        private void ValidateCurrentWord()
        {
            string currentWord = GetCurrentWord();
            if (currentWord.Length < 3) return;

            if (wordValidator.ValidateWord(currentWord))
            {
                int score = (int)wordValidator.GetWordScore(currentWord);
                OnValidWordFound?.Invoke(currentWord, score);
                Clear();
            }
            else if (!wordValidator.IsPartialWordValid(currentWord))
            {
                OnInvalidWordFound?.Invoke(currentWord);
                Clear();
            }
        }

        private string GetCurrentWord()
        {
            return string.Join("", selectedTiles.Select(t => t.Letter));
        }
    }
}