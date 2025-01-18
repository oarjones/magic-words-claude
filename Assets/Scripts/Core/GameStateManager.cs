using UnityEngine;
using MagicWords.Core.Interfaces;
using System;

namespace MagicWords.Core
{
    /// <summary>
    /// Manages game state transitions and state-specific behavior
    /// </summary>
    public class GameStateManager : MonoBehaviour, IGameStateManager
    {
        private GameState currentState = GameState.None;

        // Event triggered when game state changes
        public event Action<GameState> OnStateChanged;

        public GameState CurrentState => currentState;

        public void Initialize()
        {
            ChangeState(GameState.MainMenu);
            Debug.Log("GameStateManager initialized");
        }

        public void Dispose()
        {
            // Clean up any state-specific resources
            OnStateChanged = null;
        }

        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;

            // Exit current state
            ExitCurrentState();

            // Update state
            var oldState = currentState;
            currentState = newState;

            // Enter new state
            EnterNewState();

            // Notify listeners
            OnStateChanged?.Invoke(newState);

            Debug.Log($"Game State changed from {oldState} to {newState}");
        }

        public bool IsState(GameState state)
        {
            return currentState == state;
        }

        private void ExitCurrentState()
        {
            switch (currentState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 1f;
                    break;
            }
        }

        private void EnterNewState()
        {
            switch (currentState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
            }
        }
    }
}