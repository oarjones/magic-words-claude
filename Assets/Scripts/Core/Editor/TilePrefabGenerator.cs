using UnityEngine;

using TMPro;
using MagicWords.Core.Views;
using MagicWords.Core.Board;

#if UNITY_EDITOR
using UnityEditor;

namespace MagicWords.Core.Editor
{
    public class TilePrefabGenerator : EditorWindow
    {
        [MenuItem("MagicWords/Generate Tile Prefab")]
        public static void GenerateTilePrefab()
        {
            // Create main tile GameObject
            var tileGO = new GameObject("Tile");

            // Add required components
            var tileComponent = tileGO.AddComponent<Tile>();
            var tileView = tileGO.AddComponent<TileView>();
            var boxCollider = tileGO.AddComponent<BoxCollider2D>();

            // Create background
            var backgroundGO = new GameObject("Background");
            backgroundGO.transform.SetParent(tileGO.transform);
            backgroundGO.transform.localPosition = Vector3.zero;
            var backgroundSprite = backgroundGO.AddComponent<SpriteRenderer>();
            backgroundSprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/Tiles/TileHexagon.png");
            backgroundSprite.sortingOrder = 0;

            // Create text
            var textGO = new GameObject("Letter");
            textGO.transform.SetParent(tileGO.transform);
            textGO.transform.localPosition = new Vector3(0, 0, -0.1f);
            var tmpText = textGO.AddComponent<TextMeshPro>();
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.fontSize = 4;
            tmpText.color = Color.black;
            tmpText.sortingOrder = 1;

            // Create particle systems container
            var particlesGO = new GameObject("Particles");
            particlesGO.transform.SetParent(tileGO.transform);
            particlesGO.transform.localPosition = Vector3.zero;

            // Add particle systems
            var selectionParticles = InstantiateParticleSystem("SelectionParticles", particlesGO.transform);
            var freezeParticles = InstantiateParticleSystem("FreezeParticles", particlesGO.transform);
            var highlightParticles = InstantiateParticleSystem("HighlightParticles", particlesGO.transform);

            // Configure BoxCollider2D
            boxCollider.size = new Vector2(1f, 1f);
            boxCollider.isTrigger = true;

            // Configure TileView references
            var serializedObject = new SerializedObject(tileView);
            var backgroundProperty = serializedObject.FindProperty("backgroundRenderer");
            var letterTextProperty = serializedObject.FindProperty("letterText");
            var selectionParticlesProperty = serializedObject.FindProperty("selectionParticles");
            var freezeParticlesProperty = serializedObject.FindProperty("freezeParticles");
            var highlightParticlesProperty = serializedObject.FindProperty("highlightParticles");

            backgroundProperty.objectReferenceValue = backgroundSprite;
            letterTextProperty.objectReferenceValue = tmpText;
            selectionParticlesProperty.objectReferenceValue = selectionParticles;
            freezeParticlesProperty.objectReferenceValue = freezeParticles;
            highlightParticlesProperty.objectReferenceValue = highlightParticles;

            serializedObject.ApplyModifiedProperties();

            // Save prefab
            string prefabPath = "Assets/Resources/Prefabs/Board/Tile.prefab";

            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(prefabPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Create prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(tileGO, prefabPath);

            // Cleanup
            DestroyImmediate(tileGO);

            Debug.Log($"Tile prefab created at {prefabPath}");
        }

        private static ParticleSystem InstantiateParticleSystem(string name, Transform parent)
        {
            string prefabPath = $"Assets/Resources/Prefabs/ParticleSystems/{name}.prefab";
            GameObject particlesPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (particlesPrefab != null)
            {
                var instance = PrefabUtility.InstantiatePrefab(particlesPrefab, parent) as GameObject;
                instance.transform.localPosition = Vector3.zero;
                return instance.GetComponent<ParticleSystem>();
            }

            Debug.LogError($"Particle system prefab not found at {prefabPath}");
            return null;
        }
    }
}
#endif