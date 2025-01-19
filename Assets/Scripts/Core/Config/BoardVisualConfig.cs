using UnityEngine;

namespace MagicWords.Core.Config
{
    [CreateAssetMenu(fileName = "BoardVisualConfig", menuName = "MagicWords/Board Visual Config")]
    public class BoardVisualConfig : ScriptableObject
    {
        [Header("Board Layout")]
        public float tileSpacing = 1.1f;
        public float tileScale = 1f;
        public float boardScale = 1f;

        [Header("Background")]
        public Color backgroundColor = new Color(0.95f, 0.95f, 0.95f);
        public float backgroundPadding = 0.5f;
        public float backgroundRoundness = 0.1f;

        [Header("Animation")]
        public float boardAppearDuration = 1f;
        public float tileAppearStagger = 0.05f;
        public float initialTileScale = 0.1f;
        public float wordLineWidth = 0.1f;
        public Color wordLineColor = Color.white;

        [Header("Effects")]
        public Color validWordColor = Color.green;
        public Color invalidWordColor = Color.red;
        public float wordEffectDuration = 1f;

        [Header("Camera")]
        public float defaultZoom = 5f;
        public float minZoom = 3f;
        public float maxZoom = 8f;
        public float panSpeed = 5f;
        public float zoomSpeed = 0.5f;

        [Header("Sorting")]
        public int boardBackgroundOrder = -1;
        public int tileBaseOrder = 0;
        public int wordLineOrder = 1;
        public int effectsOrder = 2;
    }
}