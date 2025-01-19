using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace MagicWords.Core.Views
{
    public class WordEffect : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI wordText;
        [SerializeField] private ParticleSystem validWordParticles;
        [SerializeField] private ParticleSystem scoreParticles;

        [Header("Settings")]
        [SerializeField] private float displayDuration = 2f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private float scaleMultiplier = 1.5f;
        [SerializeField] private float floatDistance = 1f;

        private Vector3 originalScale;
        private Vector3 originalPosition;
        private Sequence currentAnimation;

        private void Awake()
        {
            originalScale = transform.localScale;
            originalPosition = transform.position;
            gameObject.SetActive(false);
        }

        public void ShowValidWord(string word, int score, Vector3 position)
        {
            gameObject.SetActive(true);
            transform.position = position;

            wordText.text = $"{word}\n+{score}";
            wordText.alpha = 0f;
            transform.localScale = originalScale;

            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                // Fade and scale in
                .Append(wordText.DOFade(1f, fadeInDuration))
                .Join(transform.DOScale(originalScale * scaleMultiplier, fadeInDuration))
                .SetEase(Ease.OutBack)

                // Float up
                .Join(transform.DOMove(position + Vector3.up * floatDistance, displayDuration)
                    .SetEase(Ease.OutQuad))

                // Fade out
                .Append(wordText.DOFade(0f, fadeOutDuration))
                .Join(transform.DOScale(originalScale, fadeOutDuration))
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false));

            if (validWordParticles != null)
            {
                validWordParticles.Play();
            }

            if (score > 0 && scoreParticles != null)
            {
                scoreParticles.Play();
            }
        }

        public void ShowInvalidWord(string word, Vector3 position)
        {
            gameObject.SetActive(true);
            transform.position = position;

            wordText.text = word;
            wordText.alpha = 0f;
            transform.localScale = originalScale;

            KillCurrentAnimation();

            currentAnimation = DOTween.Sequence()
                .Append(wordText.DOFade(1f, fadeInDuration))
                .Join(transform.DOScale(originalScale * 0.8f, fadeInDuration))
                .Join(transform.DOShakePosition(0.5f, 0.1f))
                .AppendInterval(displayDuration * 0.5f)
                .Append(wordText.DOFade(0f, fadeOutDuration))
                .OnComplete(() => gameObject.SetActive(false));
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