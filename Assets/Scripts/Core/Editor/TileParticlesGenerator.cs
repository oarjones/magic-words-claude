using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace MagicWords.Core.Editor
{
    public class TileParticlesGenerator : EditorWindow
    {
        [MenuItem("MagicWords/Generate Tile Particles")]
        public static void GenerateTileParticleSystems()
        {
            GenerateSelectionParticles();
            GenerateFreezeParticles();
            GenerateHighlightParticles();
        }

        private static void GenerateSelectionParticles()
        {
            var go = new GameObject("SelectionParticles");
            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 1.0f;
            main.loop = true;
            main.startLifetime = 0.5f;
            main.startSpeed = 1f;
            main.startSize = 0.1f;
            main.maxParticles = 20;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 10;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.5f;

            var col = ps.colorOverLifetime;
            col.enabled = true;

            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0.0f),
                    new GradientColorKey(Color.white, 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(0.0f, 1.0f)
                }
            );
            col.color = grad;

            SaveParticleSystem(go, "SelectionParticles");
        }

        private static void GenerateFreezeParticles()
        {
            var go = new GameObject("FreezeParticles");
            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 1.0f;
            main.loop = true;
            main.startLifetime = 1f;
            main.startSpeed = 0.5f;
            main.startSize = 0.15f;
            main.maxParticles = 30;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 15;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.4f;

            var col = ps.colorOverLifetime;
            col.enabled = true;

            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(0.8f, 0.8f, 1f), 0.0f),
                    new GradientColorKey(new Color(0.8f, 0.8f, 1f), 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(0.0f, 1.0f)
                }
            );
            col.color = grad;

            SaveParticleSystem(go, "FreezeParticles");
        }

        private static void GenerateHighlightParticles()
        {
            var go = new GameObject("HighlightParticles");
            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 1.0f;
            main.loop = true;
            main.startLifetime = 0.75f;
            main.startSpeed = 0.3f;
            main.startSize = 0.12f;
            main.maxParticles = 25;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 12;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.45f;

            var col = ps.colorOverLifetime;
            col.enabled = true;

            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0.0f),
                    new GradientColorKey(new Color(1f, 0.6f, 0.1f), 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(0.0f, 1.0f)
                }
            );
            col.color = grad;

            SaveParticleSystem(go, "HighlightParticles");
        }

        private static void SaveParticleSystem(GameObject go, string name)
        {
            string directory = "Assets/Resources/Prefabs/ParticleSystems";
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            string prefabPath = $"{directory}/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            DestroyImmediate(go);
        }
    }
}
#endif