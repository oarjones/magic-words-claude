using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MagicWords.Core.Board;
using MagicWords.Core.Config;
using MagicWords.Core.Interfaces;

namespace MagicWords.Core.Board
{
    public class BoardGenerator : MonoBehaviour, IBoardGenerator
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform boardContainer;

        private BoardConfig config;
        private Language currentLanguage;
        private readonly List<ITile> generatedTiles = new();
        private System.Random random;

        public void Initialize(BoardConfig config)
        {
            this.config = config;
            random = new System.Random();
            var dataManager = ServiceLocator.Instance.Get<IDataManager>();
            currentLanguage = dataManager?.LoadData<GameSettings>("settings")?.Language ?? Language.English;
        }

        public ITile[] GenerateBoard()
        {
            generatedTiles.Clear();

            // Calculate board dimensions based on size
            int size = config.boardSize;
            GenerateHexagonalBoard(size);
            AssignNeighbors();
            AssignRandomLetters();

            return generatedTiles.ToArray();
        }

        private void GenerateHexagonalBoard(int size)
        {
            int index = 0;
            // Generate center and surrounding rings
            for (int q = -size + 1; q < size; q++)
            {
                for (int r = -size + 1; r < size; r++)
                {
                    // Axial coordinates constraint for hexagon shape
                    if (Mathf.Abs(-q - r) >= size) continue;

                    Vector2Int position = new(q, r);
                    CreateTile(index++, position);
                }
            }
        }

        private void CreateTile(int index, Vector2Int position)
        {
            var tileObject = Instantiate(tilePrefab, boardContainer);
            var tile = tileObject.GetComponent<Tile>();

            if (tile != null)
            {
                tile.Initialize(index, position, config);
                generatedTiles.Add(tile);
            }
        }

        private void AssignNeighbors()
        {
            foreach (var tile in generatedTiles)
            {
                if (tile is Tile hexTile)
                {
                    var neighbors = GetNeighborsForPosition(hexTile.Position).ToArray();
                    hexTile.SetNeighbors(neighbors);
                }
            }
        }

        private IEnumerable<ITile> GetNeighborsForPosition(Vector2Int position)
        {
            // Hex directions for axial coordinates
            Vector2Int[] directions = new[]
            {
                new Vector2Int(1, 0),   // Right
                new Vector2Int(1, -1),  // Top Right
                new Vector2Int(0, -1),  // Top Left
                new Vector2Int(-1, 0),  // Left
                new Vector2Int(-1, 1),  // Bottom Left
                new Vector2Int(0, 1)    // Bottom Right
            };

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = position + dir;
                var neighbor = generatedTiles.FirstOrDefault(t =>
                    t is Tile tile && tile.Position == neighborPos);

                if (neighbor != null)
                {
                    yield return neighbor;
                }
            }
        }

        private void AssignRandomLetters()
        {
            foreach (var tile in generatedTiles)
            {
                char letter = GetRandomLetter();
                tile.SetLetter(letter);
            }
        }

        private char GetRandomLetter()
        {
            // Get letter based on language and configuration
            var letters = currentLanguage == Language.English ?
                EnglishLetterDistribution : SpanishLetterDistribution;

            float totalWeight = letters.Sum(l => l.weight);
            float randomValue = (float)random.NextDouble() * totalWeight;

            float currentSum = 0;
            foreach (var letter in letters)
            {
                currentSum += letter.weight;
                if (randomValue <= currentSum)
                {
                    return letter.character;
                }
            }

            return 'A'; // Fallback
        }

        public void SetInitialWord(string word)
        {
            if (string.IsNullOrEmpty(word) || generatedTiles.Count == 0) return;

            var startTile = generatedTiles[random.Next(generatedTiles.Count)];
            var currentTile = startTile;

            foreach (char letter in word.ToUpper())
            {
                currentTile.SetLetter(letter);

                // Get available neighbors
                var availableNeighbors = currentTile.GetNeighbors()
                    .Where(n => !word.Contains(n.Letter))
                    .ToList();

                if (availableNeighbors.Count == 0) break;

                // Select random neighbor for next letter
                currentTile = availableNeighbors[random.Next(availableNeighbors.Count)];
            }
        }

        private readonly (char character, float weight)[] EnglishLetterDistribution = {
            ('E', 12.7f), ('T', 9.1f), ('A', 8.2f), ('O', 7.5f), ('I', 7.0f),
            ('N', 6.7f), ('S', 6.3f), ('H', 6.1f), ('R', 6.0f), ('D', 4.3f),
            ('L', 4.0f), ('U', 2.8f), ('C', 2.8f), ('M', 2.4f), ('W', 2.4f),
            ('F', 2.2f), ('G', 2.0f), ('Y', 2.0f), ('P', 1.9f), ('B', 1.5f),
            ('V', 0.98f),('K', 0.77f),('J', 0.15f),('X', 0.15f),('Q', 0.095f),
            ('Z', 0.074f)
        };

        private readonly (char character, float weight)[] SpanishLetterDistribution = {
            ('A', 12.5f), ('E', 13.7f), ('O', 8.7f), ('I', 6.2f), ('S', 7.9f),
            ('N', 6.7f), ('R', 6.9f), ('L', 5.0f), ('T', 4.6f), ('D', 5.9f),
            ('C', 4.7f), ('U', 3.9f), ('M', 3.2f), ('P', 2.5f), ('B', 1.4f),
            ('G', 1.0f), ('V', 0.9f), ('Y', 0.9f), ('Q', 0.9f), ('H', 0.7f),
            ('F', 0.7f), ('Z', 0.5f), ('J', 0.4f), ('Ñ', 0.3f), ('X', 0.2f),
            ('W', 0.01f), ('K', 0.01f)
        };
    }

    [System.Serializable]
    public class GameSettings
    {
        public Language Language;
    }
}