using UnityEngine;
using MagicWords.Core.Interfaces;
using System.Linq;

namespace MagicWords.Core
{
    /// <summary>
    /// Controlador principal del juego. Gestiona el estado global y la inicialización de sistemas.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }

        private void InitializeGame()
        {
            Debug.Log("Initializing game systems...");

            // Inicializar servicios principales
            InitializeServices();

            // Cargar configuración
            LoadConfiguration();

            // Inicializar Firebase
            InitializeFirebase();

            Debug.Log("Game initialization completed");
        }

        private void InitializeServices()
        {
            // Registrar servicios principales en el ServiceLocator
            var services = new IGameService[]
            {
                gameObject.AddComponent<GameStateManager>(),
                gameObject.AddComponent<DataManager>(),
                gameObject.AddComponent<AudioManager>(),
                gameObject.AddComponent<FirebaseManager>()
            };

            foreach (var service in services)
            {
                service.Initialize();
                ServiceLocator.Instance.Register(service);
            }
        }

        private void LoadConfiguration()
        {
            // Cargar configuraciones desde ScriptableObjects
            // TODO: Implementar carga de configuración
        }

        private void InitializeFirebase()
        {
            var firebaseManager = ServiceLocator.Instance.Get<IFirebaseManager>();
            if (firebaseManager != null && !firebaseManager.IsInitialized)
            {
                // TODO: Inicializar Firebase
            }
        }

        private void OnApplicationQuit()
        {
            // Limpiar servicios y guardar datos necesarios
            var services = FindObjectsOfType<MonoBehaviour>().OfType<IGameService>();
            foreach (var service in services)
            {
                service.Dispose();
            }

            ServiceLocator.Instance.Clear();
        }

        public void StartNewGame(GameMode mode, GameDifficulty difficulty)
        {
            var stateManager = ServiceLocator.Instance.Get<IGameStateManager>();
            stateManager?.ChangeState(GameState.Loading);

            // TODO: Implementar lógica de inicio de juego
        }

        public void PauseGame()
        {
            var stateManager = ServiceLocator.Instance.Get<IGameStateManager>();
            stateManager?.ChangeState(GameState.Paused);
        }

        public void ResumeGame()
        {
            var stateManager = ServiceLocator.Instance.Get<IGameStateManager>();
            stateManager?.ChangeState(GameState.Playing);
        }

        public void EndGame()
        {
            var stateManager = ServiceLocator.Instance.Get<IGameStateManager>();
            stateManager?.ChangeState(GameState.GameOver);
        }
    }
}