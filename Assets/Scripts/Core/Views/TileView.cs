using UnityEngine;
using TMPro;
using DG.Tweening;
using MagicWords.Core.Config;
using MagicWords.Core.Board;

namespace MagicWords.Core.Views
{
    [RequireComponent(typeof(Tile))]
    public class TileView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private TextMeshPro letterText;
        [SerializeField] private ParticleSystem selectionParticles;
        [SerializeField] private ParticleSystem freezeParticles;
        [SerializeField] private ParticleSystem highlightParticles;

        [Header("Configuration")]
        [SerializeField] private TileVisualConfig visualConfig;

        private Vector3 originalScale;
        private Sequence currentAnimation;
        private TileState currentState;

        private void Awake()
        {
            if (visualConfig == null)
            {
                visualConfig = Resources.Load<TileVisualConfig>("Configs/TileVisualConfig");
                if (visualConfig == null)
                {
                    Debug.LogError("TileVisualConfig not found!");
                    return;
                }
            }

            originalScale = transform.localScale;
            SetupComponents();
            StopAllParticles();
        }

        private void SetupComponents()
        {
            if (backgroundRenderer != null)
            {
                backgroundRenderer.sortingOrder = visualConfig.backgroundSortingOrder;
                backgroundRenderer.color = visualConfig.normalColor;
            }

            if (letterText != null)
            {
                letterText.sortingOrder = visualConfig.letterSortingOrder;
                letterText.color = visualConfig.textColor;
                letterText.fontSize = visualConfig.textSize;
                letterText.transform.localPosition = new Vector3(
                    letterText.transform.localPosition.x,
                    visualConfig.textVerticalOffset,
                    letterText.transform.localPosition.z
                );
            }

            SetupParticleSystem(selectionParticles, visualConfig.selectionParticleRate);
            SetupParticleSystem(freezeParticles, visualConfig.freezeParticleRate);
            SetupParticleSystem(highlightParticles, visualConfig.highlightParticleRate);
        }

        private void SetupParticleSystem(ParticleSystem ps, float rate)
        {
            if (ps != null)
            {
                var main = ps.main;
                main.simulationSpace = ParticleSystemSimulationSpace.World;

                var emission = ps.emission;
                emission.rateOverTime = rate;

                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = visualConfig.particleSortingOrder;
                }
            }
        }

        public void SetLetter(char letter)
        {
            if (letterText != null)
            {
                letterText.text = letter.ToString();
            }
        }

        public void PlaySelectAnimation()
        {
            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                .Append(transform.DOScale(originalScale * visualConfig.selectedScaleMultiplier,
                    visualConfig.selectAnimationDuration))
                .Join(backgroundRenderer.DOColor(visualConfig.selectedColor,
                    visualConfig.selectAnimationDuration))
                .SetEase(Ease.OutBack);

            if (selectionParticles != null)
            {
                selectionParticles.Play();
            }
        }

        public void PlayDeselectAnimation()
        {
            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                .Append(transform.DOScale(originalScale, visualConfig.deselectAnimationDuration))
                .Join(backgroundRenderer.DOColor(GetStateColor(currentState),
                    visualConfig.deselectAnimationDuration))
                .SetEase(Ease.OutQuad);

            if (selectionParticles != null)
            {
                selectionParticles.Stop();
            }
        }

        public void PlayInvalidAnimation()
        {
            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                .Append(transform.DOShakePosition(visualConfig.invalidAnimationDuration,
                    visualConfig.invalidAnimationIntensity))
                .Join(backgroundRenderer.DOColor(visualConfig.invalidColor,
                    visualConfig.invalidAnimationDuration * 0.5f))
                .Append(backgroundRenderer.DOColor(GetStateColor(currentState),
                    visualConfig.invalidAnimationDuration * 0.5f));
        }

        public void SetState(TileState state)
        {
            currentState = state;
            Color targetColor = GetStateColor(state);

            if (currentAnimation != null && currentAnimation.IsPlaying())
            {
                currentAnimation.OnComplete(() =>
                {
                    backgroundRenderer.DOColor(targetColor, visualConfig.selectAnimationDuration);
                });
            }
            else
            {
                backgroundRenderer.DOColor(targetColor, visualConfig.selectAnimationDuration);
            }

            UpdateParticles(state);
        }

        private Color GetStateColor(TileState state)
        {
            return state switch
            {
                TileState.Normal => visualConfig.normalColor,
                TileState.Selected => visualConfig.selectedColor,
                TileState.Frozen => visualConfig.frozenColor,
                TileState.Objective => visualConfig.objectiveColor,
                _ => visualConfig.normalColor
            };
        }

        private void UpdateParticles(TileState state)
        {
            StopAllParticles();

            switch (state)
            {
                case TileState.Frozen:
                    if (freezeParticles != null) freezeParticles.Play();
                    break;
                case TileState.Objective:
                    if (highlightParticles != null) highlightParticles.Play();
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