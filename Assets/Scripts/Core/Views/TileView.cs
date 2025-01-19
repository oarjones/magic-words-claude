using UnityEngine;
using TMPro;
using DG.Tweening;

namespace MagicWords.Core.Views
{
    public class TileView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private TextMeshPro letterText;
        [SerializeField] private ParticleSystem selectionParticles;
        [SerializeField] private ParticleSystem freezeParticles;
        [SerializeField] private ParticleSystem highlightParticles;

        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color frozenColor;
        [SerializeField] private Color objectiveColor;

        private Vector3 originalScale;
        private Sequence currentAnimation;

        private void Awake()
        {
            originalScale = transform.localScale;
            StopAllParticles();
        }

        public void SetLetter(char letter)
        {
            if (letterText != null)
            {
                letterText.text = letter.ToString();
            }
        }

        public void PlaySelectAnimation(float duration, float scaleMultiplier)
        {
            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                .Append(transform.DOScale(originalScale * scaleMultiplier, duration))
                .Join(backgroundRenderer.DOColor(selectedColor, duration))
                .SetEase(Ease.OutBack);

            if (selectionParticles != null)
            {
                selectionParticles.Play();
            }
        }

        public void PlayDeselectAnimation(float duration)
        {
            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                .Append(transform.DOScale(originalScale, duration))
                .Join(backgroundRenderer.DOColor(normalColor, duration))
                .SetEase(Ease.OutQuad);

            if (selectionParticles != null)
            {
                selectionParticles.Stop();
            }
        }

        public void PlayInvalidAnimation(float duration, float intensity)
        {
            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                .Append(transform.DOShakePosition(duration, intensity))
                .Join(backgroundRenderer.DOColor(Color.red, duration * 0.5f))
                .Append(backgroundRenderer.DOColor(normalColor, duration * 0.5f));
        }

        public void SetState(TileState state, float transitionDuration = 0.3f)
        {
            Color targetColor = state switch
            {
                TileState.Normal => normalColor,
                TileState.Selected => selectedColor,
                TileState.Frozen => frozenColor,
                TileState.Objective => objectiveColor,
                _ => normalColor
            };

            backgroundRenderer.DOColor(targetColor, transitionDuration);

            UpdateParticles(state);
        }

        private void UpdateParticles(TileState state)
        {
            switch (state)
            {
                case TileState.Frozen:
                    if (freezeParticles != null) freezeParticles.Play();
                    break;
                case TileState.Objective:
                    if (highlightParticles != null) highlightParticles.Play();
                    break;
                default:
                    StopAllParticles();
                    break;
            }
        }

        private void StopAllParticles()
        {
            if (selectionParticles != null) selectionParticles.Stop();
            if (freezeParticles != null) freezeParticles.Stop();
            if (highlightParticles != null) highlightParticles.Stop();
        }

        private void KillCurrentAnimation()
        {
            currentAnimation?.Kill();
            currentAnimation = null;
        }

        private void OnDestroy()
        {
            KillCurrentAnimation();
        }
    }
}