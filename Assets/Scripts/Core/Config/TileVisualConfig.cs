using UnityEngine;

namespace MagicWords.Core.Config
{
    [CreateAssetMenu(fileName = "TileVisualConfig", menuName = "MagicWords/Tile Visual Config")]
    public class TileVisualConfig : ScriptableObject
    {
        [Header("Colors")]
        public Color normalColor = Color.white;
        public Color selectedColor = new Color(0.4f, 0.8f, 1f);
        public Color frozenColor = new Color(0.8f, 0.8f, 1f);
        public Color objectiveColor = new Color(1f, 0.8f, 0.8f);
        public Color invalidColor = new Color(1f, 0.4f, 0.4f);

        [Header("Text Settings")]
        public Color textColor = Color.black;
        public float textSize = 4f;
        public float textVerticalOffset = 0f;

        [Header("Animation Settings")]
        public float selectAnimationDuration = 0.3f;
        public float deselectAnimationDuration = 0.2f;
        public float invalidAnimationDuration = 0.5f;
        public float selectedScaleMultiplier = 1.2f;
        public float invalidAnimationIntensity = 0.2f;

        [Header("Particle Settings")]
        public float selectionParticleRate = 10f;
        public float freezeParticleRate = 15f;
        public float highlightParticleRate = 12f;

        [Header("Sorting Settings")]
        public int backgroundSortingOrder = 0;
        public int letterSortingOrder = 1;
        public int particleSortingOrder = 2;
    }
}