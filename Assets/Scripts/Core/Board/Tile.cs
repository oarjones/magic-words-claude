using UnityEngine;
using DG.Tweening;
using TMPro;
using MagicWords.Core.Config;

namespace MagicWords.Core.Board
{
    public class Tile : MonoBehaviour, ITile
    {
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private TextMeshPro letterText;
        [SerializeField] private ParticleSystem selectionEffect;
        [SerializeField] private ParticleSystem freezeEffect;

        private int index;
        private Vector2Int position;
        private char letter;
        private TileState currentState;
        private bool isSelected;
        private BoardConfig config;
        private ITile[] neighbors;

        public int Index => index;
        public Vector2Int Position => position;
        public char Letter => letter;
        public TileState State => currentState;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            if (selectionEffect != null) selectionEffect.Stop();
            if (freezeEffect != null) freezeEffect.Stop();
        }

        public void Initialize(int index, Vector2Int position, BoardConfig config)
        {
            this.index = index;
            this.position = position;
            this.config = config;
            currentState = TileState.Normal;
            isSelected = false;

            transform.localScale = Vector3.one * config.tileScale;
        }

        public void SetLetter(char letter)
        {
            this.letter = letter;
            if (letterText != null)
            {
                letterText.text = letter.ToString();
            }
        }

        public void Select()
        {
            if (isSelected) return;

            isSelected = true;
            transform.DOScale(Vector3.one * config.selectedTileScale, config.tileSelectAnimationDuration)
                .SetEase(Ease.OutBack);

            if (selectionEffect != null)
            {
                selectionEffect.Play();
            }
        }

        public void Deselect()
        {
            if (!isSelected) return;

            isSelected = false;
            transform.DOScale(Vector3.one * config.tileScale, config.tileDeselectAnimationDuration)
                .SetEase(Ease.OutQuad);

            if (selectionEffect != null)
            {
                selectionEffect.Stop();
            }
        }

        public void SetState(TileState state)
        {
            if (currentState == state) return;

            currentState = state;
            UpdateVisuals();

            switch (state)
            {
                case TileState.Frozen:
                    if (freezeEffect != null) freezeEffect.Play();
                    break;
                case TileState.Normal:
                    if (freezeEffect != null) freezeEffect.Stop();
                    break;
            }
        }

        private void UpdateVisuals()
        {
            Color targetColor = Color.white;
            switch (currentState)
            {
                case TileState.Frozen:
                    targetColor = new Color(0.8f, 0.8f, 1f);
                    break;
                case TileState.Objective:
                    targetColor = new Color(1f, 0.8f, 0.8f);
                    break;
            }

            if (backgroundRenderer != null)
            {
                backgroundRenderer.color = targetColor;
            }
        }

        public void SetNeighbors(ITile[] neighbors)
        {
            this.neighbors = neighbors;
        }

        public ITile[] GetNeighbors()
        {
            return neighbors;
        }

        public void PlayInvalidAnimation()
        {
            transform.DOShakePosition(config.invalidWordShakeDuration, 0.1f, 10, 90, false, true);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}