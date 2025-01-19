using MagicWords.Core.Board;
using MagicWords.Core.Config;
using UnityEngine;

namespace MagicWords.Core.PowerUps
{
    public class ChangeCellPowerUp : PowerUpBase
    {
        private Tile selectedTile;

        public override bool Activate()
        {
            if (!base.Activate()) return false;

            // Enable tile selection mode
            var boardManager = FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                // TODO: Enable tile selection mode in BoardManager
                return true;
            }

            return false;
        }

        public override void Deactivate()
        {
            if (!IsActive) return;

            // Disable tile selection mode
            var boardManager = FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                // TODO: Disable tile selection mode in BoardManager
            }

            base.Deactivate();
        }

        public void OnTileSelected(Tile tile)
        {
            if (!IsActive || tile == null) return;

            selectedTile = tile;
            Deactivate();
        }
    }

    public class ChangeLetterPowerUp : PowerUpBase
    {
        private Tile targetTile;

        public override bool Activate()
        {
            if (!base.Activate()) return false;

            // Get current tile from BoardManager
            var boardManager = FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                // TODO: Get current tile and show letter selection UI
                return true;
            }

            return false;
        }

        public override void Deactivate()
        {
            if (!IsActive) return;

            targetTile = null;
            base.Deactivate();
        }

        public void OnLetterSelected(char letter)
        {
            if (!IsActive || targetTile == null) return;

            // Change tile letter
            targetTile.SetLetter(letter);
            Deactivate();
        }
    }

    public class FreezeTrapPowerUp : PowerUpBase
    {
        private float freezeDuration;

        public override void Initialize(PowerUpConfig config)
        {
            base.Initialize(config);
            freezeDuration = config.duration;
        }

        public override bool Activate()
        {
            if (!base.Activate()) return false;

            // Enable trap placement mode
            var boardManager = FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                // TODO: Enable trap placement mode in BoardManager
                return true;
            }

            return false;
        }

        public override void Deactivate()
        {
            if (!IsActive) return;

            // Disable trap placement mode
            var boardManager = FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                // TODO: Disable trap placement mode in BoardManager
            }

            base.Deactivate();
        }

        public void OnTileSelected(Tile tile)
        {
            if (!IsActive || tile == null) return;

            // Set freeze trap on tile
            tile.SetState(TileState.Frozen);
            // TODO: Start freeze timer

            Deactivate();
        }
    }
}