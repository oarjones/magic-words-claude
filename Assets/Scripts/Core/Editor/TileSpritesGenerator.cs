using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace MagicWords.Core.Editor
{
    public class TileSpritesGenerator : EditorWindow
    {
        private const int textureSize = 256;

        [MenuItem("MagicWords/Generate Tile Sprites")]
        public static void GenerateTileSprites()
        {
            // Generate tile hexagon sprite
            var hexagonSprite = GenerateHexagonSprite();
            SaveSpriteToFile(hexagonSprite, "TileHexagon");
        }

        private static Texture2D GenerateHexagonSprite()
        {
            var texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

            // Clear texture
            var clearColors = new Color[textureSize * textureSize];
            for (int i = 0; i < clearColors.Length; i++)
                clearColors[i] = Color.clear;
            texture.SetPixels(clearColors);

            // Calculate hexagon points
            Vector2 center = new Vector2(textureSize / 2, textureSize / 2);
            float radius = textureSize * 0.4f;
            Vector2[] points = new Vector2[6];

            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI / 3f;
                points[i] = center + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );
            }

            // Draw hexagon
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    Vector2 pixel = new Vector2(x, y);
                    if (IsPointInHexagon(pixel, points))
                    {
                        // Add slight gradient
                        float distanceToCenter = Vector2.Distance(pixel, center);
                        float normalizedDistance = distanceToCenter / radius;
                        float gradient = 1f - (normalizedDistance * 0.2f);

                        texture.SetPixel(x, y, new Color(gradient, gradient, gradient, 1));
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private static bool IsPointInHexagon(Vector2 point, Vector2[] hexPoints)
        {
            int j = hexPoints.Length - 1;
            bool inside = false;

            for (int i = 0; i < hexPoints.Length; i++)
            {
                if (((hexPoints[i].y > point.y) != (hexPoints[j].y > point.y)) &&
                    (point.x < (hexPoints[j].x - hexPoints[i].x) * (point.y - hexPoints[i].y)
                    / (hexPoints[j].y - hexPoints[i].y) + hexPoints[i].x))
                {
                    inside = !inside;
                }
                j = i;
            }

            return inside;
        }

        private static void SaveSpriteToFile(Texture2D texture, string name)
        {
            // Ensure directory exists
            string directory = "Assets/Resources/Sprites/Tiles";
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Save texture as PNG
            string path = $"{directory}/{name}.png";
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.Refresh();

            // Configure texture import settings
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 256;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }
    }
}
#endif