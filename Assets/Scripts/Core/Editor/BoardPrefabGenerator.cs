using UnityEngine;
using UnityEditor;
using TMPro;
using MagicWords.Core.Board;
using MagicWords.Core.Views;

#if UNITY_EDITOR
using UnityEngine.UI;

namespace MagicWords.Core.Editor
{
    public class BoardPrefabGenerator : EditorWindow
    {
        [MenuItem("MagicWords/Generate Board Prefab")]
        public static void GenerateBoardPrefab()
        {
            // Create main Board GameObject
            var boardGO = new GameObject("Board");

            // Add required components
            var boardManager = boardGO.AddComponent<BoardManager>();
            var boardGenerator = boardGO.AddComponent<BoardGenerator>();
            var wordValidator = boardGO.AddComponent<WordValidator>();

            // Create containers
            var tilesContainer = CreateContainer("TilesContainer", boardGO.transform);
            var effectsContainer = CreateContainer("EffectsContainer", boardGO.transform);
            var uiContainer = CreateContainer("UIContainer", boardGO.transform);

            // Create board background
            var backgroundGO = CreateBoardBackground();
            backgroundGO.transform.SetParent(boardGO.transform);
            backgroundGO.transform.SetAsFirstSibling();

            // Create word effects
            var wordEffectGO = CreateWordEffect(effectsContainer.transform);
            var scoreEffectGO = CreateScoreEffect(effectsContainer.transform);

            // Create word line renderer
            var wordLineGO = new GameObject("WordLine");
            wordLineGO.transform.SetParent(effectsContainer.transform);
            var lineRenderer = wordLineGO.AddComponent<LineRenderer>();
            ConfigureLineRenderer(lineRenderer);

            // Configure BoardManager references
            var serializedBoard = new SerializedObject(boardManager);

            var tilesContainerProperty = serializedBoard.FindProperty("boardContainer");
            var tilePrefabProperty = serializedBoard.FindProperty("tilePrefab");
            var wordEffectProperty = serializedBoard.FindProperty("wordEffect");
            var scoreEffectProperty = serializedBoard.FindProperty("scoreEffect");
            var lineRendererProperty = serializedBoard.FindProperty("wordLine");

            tilesContainerProperty.objectReferenceValue = tilesContainer;
            tilePrefabProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Board/Tile.prefab");
            wordEffectProperty.objectReferenceValue = wordEffectGO.GetComponent<WordEffect>();
            scoreEffectProperty.objectReferenceValue = scoreEffectGO.GetComponent<ScoreEffect>();
            lineRendererProperty.objectReferenceValue = lineRenderer;

            serializedBoard.ApplyModifiedProperties();

            // Save prefab
            string prefabPath = "Assets/Resources/Prefabs/Board/Board.prefab";

            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(prefabPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Create prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(boardGO, prefabPath);

            // Cleanup
            DestroyImmediate(boardGO);

            Debug.Log($"Board prefab created at {prefabPath}");
        }

        private static GameObject CreateContainer(string name, Transform parent)
        {
            var container = new GameObject(name);
            container.transform.SetParent(parent);
            container.transform.localPosition = Vector3.zero;
            return container;
        }

        private static GameObject CreateBoardBackground()
        {
            var backgroundGO = new GameObject("BoardBackground");
            var spriteRenderer = backgroundGO.AddComponent<SpriteRenderer>();

            // Create a white pixel sprite for the background
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            var sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = -1;

            return backgroundGO;
        }

        private static GameObject CreateWordEffect(Transform parent)
        {
            var wordEffectGO = new GameObject("WordEffect");
            wordEffectGO.transform.SetParent(parent);

            // Add Canvas
            var canvas = wordEffectGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 2;

            // Add Text
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(wordEffectGO.transform);
            var textComponent = textGO.AddComponent<TextMeshProUGUI>();
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 36;

            // Add WordEffect component
            var wordEffect = wordEffectGO.AddComponent<WordEffect>();

            return wordEffectGO;
        }

        private static GameObject CreateScoreEffect(Transform parent)
        {
            var scoreEffectGO = new GameObject("ScoreEffect");
            scoreEffectGO.transform.SetParent(parent);

            // Add ScoreEffect component
            var scoreEffect = scoreEffectGO.AddComponent<ScoreEffect>();

            // Add particle systems
            var particleSystem = scoreEffectGO.AddComponent<ParticleSystem>();
            ConfigureScoreParticleSystem(particleSystem);

            return scoreEffectGO;
        }

        private static void ConfigureLineRenderer(LineRenderer lineRenderer)
        {
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = 0;
            lineRenderer.useWorldSpace = true;
            lineRenderer.sortingOrder = 1;

            // Create a basic material for the line
            var material = new Material(Shader.Find("Sprites/Default"));
            material.color = Color.white;
            lineRenderer.material = material;
        }

        private static void ConfigureScoreParticleSystem(ParticleSystem ps)
        {
            var main = ps.main;
            main.duration = 1f;
            main.loop = false;
            main.startLifetime = 1f;
            main.startSpeed = 2f;
            main.startSize = 0.2f;
            main.maxParticles = 50;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 20;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.2f;
        }
    }
}
#endif