using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using MagicWords.Core.Config;

namespace MagicWords.Core.Board
{
    public class BoardManager : MonoBehaviour, IBoard
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform boardContainer;

        private BoardConfig config;
        private Dictionary<Vector2Int, Tile> tiles = new();
        private List<ITile> selectedTiles = new();
        private IBoardGenerator boardGenerator;
        private IWordValidator wordValidator;

        public event Action<ITile> OnTileSelected;

        private void Awake()
        {
            boardGenerator = GetComponent<IBoardGenerator>();
            wordValidator = GetComponent<IWordValidator>();
        }

        public void Initialize(BoardConfig config)
        {
            this.config = config;
            boardGenerator?.Initialize(config);
            ClearBoard();
        }

        public void GenerateBoard()
        {
            ClearBoard();

            var generatedTiles = boardGenerator.GenerateBoard();
            foreach (var tile in generatedTiles)
            {
                if (tile is Tile hexTile)
                {
                    tiles[hexTile.Position] = hexTile;
                }
            }

            ArrangeTilesVisually();
        }

        public void ClearBoard()
        {
            // Destroy existing tiles
            foreach (var tile in tiles.Values)
            {
                if (tile is MonoBehaviour mb)
                {
                    Destroy(mb.gameObject);
                }
            }

            tiles.Clear();
            selectedTiles.Clear();
        }

        private void ArrangeTilesVisually()
        {
            foreach (var tile in tiles.Values)
            {
                if (tile is Tile hexTile)
                {
                    Vector3 position = CalculateTilePosition(hexTile.Position);
                    hexTile.transform.position = position;
                }
            }
        }

        private Vector3 CalculateTilePosition(Vector2Int position)
        {
            float xOffset = config.tileSpacing * 0.75f;
            float yOffset = config.tileSpacing * 0.866f;

            float x = position.x * xOffset;
            float y = position.y * yOffset;

            // Offset every other row
            if (position.y % 2 != 0)
            {
                x += xOffset * 0.5f;
            }

            return new Vector3(x, y, 0);
        }

        public ITile GetTile(Vector2Int position)
        {
            return tiles.TryGetValue(position, out var tile) ? tile : null;
        }

        public ITile GetTile(int index)
        {
            return tiles.Values.FirstOrDefault(t => t.Index == index);
        }

        public bool ValidateMove(ITile from, ITile to)
        {
            if (from == null || to == null) return false;

            // Check if tiles are neighbors
            var neighbors = from.GetNeighbors();
            if (!neighbors.Contains(to)) return false;

            // Check if tile is already selected
            if (selectedTiles.Contains(to)) return false;

            // If there are selected tiles, validate partial word
            if (selectedTiles.Count > 0)
            {
                string currentWord = GetCurrentWord() + to.Letter;
                if (!wordValidator.IsPartialWordValid(currentWord))
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetSelection()
        {
            foreach (var tile in selectedTiles)
            {
                tile.Deselect();
            }
            selectedTiles.Clear();
        }

        private string GetCurrentWord()
        {
            return string.Join("", selectedTiles.Select(t => t.Letter));
        }

        public bool TrySelectTile(ITile tile)
        {
            if (tile == null) return false;

            // If it's the first tile
            if (selectedTiles.Count == 0)
            {
                SelectTile(tile);
                return true;
            }

            // Validate move
            var lastSelectedTile = selectedTiles[^1];
            if (!ValidateMove(lastSelectedTile, tile))
            {
                return false;
            }

            SelectTile(tile);
            return true;
        }

        private void SelectTile(ITile tile)
        {
            tile.Select();
            selectedTiles.Add(tile);
            OnTileSelected?.Invoke(tile);
        }

        public bool ValidateCurrentWord()
        {
            string word = GetCurrentWord();
            if (string.IsNullOrEmpty(word)) return false;

            return wordValidator.ValidateWord(word);
        }
    }
}