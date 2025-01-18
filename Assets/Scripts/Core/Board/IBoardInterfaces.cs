using UnityEngine;
using System;
using MagicWords.Core.Config;

namespace MagicWords.Core.Board
{
    public interface IBoard
    {
        void Initialize(BoardConfig config);
        void GenerateBoard();
        void ClearBoard();
        ITile GetTile(Vector2Int position);
        ITile GetTile(int index);
        bool ValidateMove(ITile from, ITile to);
        void ResetSelection();
        event Action<ITile> OnTileSelected;
    }

    public interface ITile
    {
        int Index { get; }
        Vector2Int Position { get; }
        char Letter { get; }
        TileState State { get; }
        bool IsSelected { get; }
        void SetLetter(char letter);
        void Select();
        void Deselect();
        void SetState(TileState state);
        ITile[] GetNeighbors();
    }

    public interface IBoardGenerator
    {
        void Initialize(BoardConfig config);
        ITile[] GenerateBoard();
        void SetInitialWord(string word);
    }

    public interface IWordValidator
    {
        bool ValidateWord(string word);
        bool IsPartialWordValid(string partialWord);
        float GetWordScore(string word);
    }
}