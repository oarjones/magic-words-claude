using UnityEngine;
using System;
using MagicWords.Core.Interfaces;
using System.Collections.Generic;

namespace MagicWords.Core
{
    /// <summary>
    /// Manages persistent data and game configuration
    /// </summary>
    public class DataManager : MonoBehaviour, IDataManager
    {
        private const string SAVE_PREFIX = "MAGICWORDS_";
        private readonly Dictionary<string, object> cache = new();

        public void Initialize()
        {
            LoadDefaultSettings();
            Debug.Log("DataManager initialized");
        }

        public void Dispose()
        {
            SaveAllData();
            cache.Clear();
        }

        public T LoadData<T>(string key) where T : class
        {
            string fullKey = SAVE_PREFIX + key;

            // Check cache first
            if (cache.TryGetValue(fullKey, out object cachedData))
            {
                return cachedData as T;
            }

            // If not in cache, load from PlayerPrefs
            string json = PlayerPrefs.GetString(fullKey, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                T data = JsonUtility.FromJson<T>(json);
                cache[fullKey] = data;
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading data for key {key}: {e.Message}");
                return null;
            }
        }

        public void SaveData<T>(string key, T data) where T : class
        {
            if (data == null)
            {
                Debug.LogError($"Attempted to save null data for key {key}");
                return;
            }

            string fullKey = SAVE_PREFIX + key;

            try
            {
                string json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(fullKey, json);
                cache[fullKey] = data;
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving data for key {key}: {e.Message}");
            }
        }

        public void DeleteData(string key)
        {
            string fullKey = SAVE_PREFIX + key;
            PlayerPrefs.DeleteKey(fullKey);
            cache.Remove(fullKey);
            PlayerPrefs.Save();
        }

        public void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            cache.Clear();
            PlayerPrefs.Save();
            LoadDefaultSettings();
        }

        private void LoadDefaultSettings()
        {
            // Load or create default settings if they don't exist
            if (!PlayerPrefs.HasKey(SAVE_PREFIX + "settings"))
            {
                var defaultSettings = new GameSettings
                {
                    MusicVolume = 0.7f,
                    SoundVolume = 1.0f,
                    Language = Language.English,
                    FirstTimePlayer = true
                };

                SaveData("settings", defaultSettings);
            }
        }

        private void SaveAllData()
        {
            foreach (var item in cache)
            {
                string json = JsonUtility.ToJson(item.Value);
                PlayerPrefs.SetString(item.Key, json);
            }
            PlayerPrefs.Save();
        }

        [Serializable]
        private class GameSettings
        {
            public float MusicVolume;
            public float SoundVolume;
            public Language Language;
            public bool FirstTimePlayer;
        }
    }
}