using UnityEngine;

namespace MagicWords.Core.Interfaces
{
    /// <summary>
    /// Interface base para todos los servicios del juego
    /// </summary>
    public interface IGameService
    {
        void Initialize();
        void Dispose();
    }

    /// <summary>
    /// Interface para gestionar el estado del juego
    /// </summary>
    public interface IGameStateManager : IGameService
    {
        GameState CurrentState { get; }
        void ChangeState(GameState newState);
        bool IsState(GameState state);
    }

    /// <summary>
    /// Interface para la gestión de datos persistentes
    /// </summary>
    public interface IDataManager : IGameService
    {
        T LoadData<T>(string key) where T : class;
        void SaveData<T>(string key, T data) where T : class;
        void DeleteData(string key);
        void ClearAllData();
    }

    /// <summary>
    /// Interface para la gestión de audio
    /// </summary>
    public interface IAudioManager : IGameService
    {
        void PlaySound(AudioClipId clipId, float volume = 1f);
        void PlayMusic(AudioClipId clipId, float volume = 1f, bool loop = true);
        void StopMusic();
        void SetMusicVolume(float volume);
        void SetSoundVolume(float volume);
    }

    /// <summary>
    /// Interface para la gestión de Firebase
    /// </summary>
    public interface IFirebaseManager : IGameService
    {
        bool IsInitialized { get; }
        void SignIn(string email, string password);
        void SignOut();
        void CreateUser(string email, string password, string username);
        string GetCurrentUserId();
    }
}