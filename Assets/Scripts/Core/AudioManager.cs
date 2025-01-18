using UnityEngine;
using MagicWords.Core.Interfaces;
using System.Collections.Generic;

namespace MagicWords.Core
{
    /// <summary>
    /// Manages all game audio including music and sound effects
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        private AudioSource musicSource;
        private AudioSource soundSource;
        private Dictionary<AudioClipId, AudioClip> audioClips;

        private float musicVolume = 1f;
        private float soundVolume = 1f;

        public void Initialize()
        {
            SetupAudioSources();
            LoadAudioClips();
            LoadSettings();
            Debug.Log("AudioManager initialized");
        }

        public void Dispose()
        {
            SaveSettings();
            StopAllAudio();
        }

        private void SetupAudioSources()
        {
            // Setup music source
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;

            // Setup sound source
            soundSource = gameObject.AddComponent<AudioSource>();
            soundSource.playOnAwake = false;
            soundSource.loop = false;
        }

        private void LoadAudioClips()
        {
            audioClips = new Dictionary<AudioClipId, AudioClip>();

            // Load all audio clips from Resources
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
            foreach (AudioClip clip in clips)
            {
                if (System.Enum.TryParse(clip.name, out AudioClipId clipId))
                {
                    audioClips[clipId] = clip;
                }
                else
                {
                    Debug.LogWarning($"Audio clip {clip.name} does not match any AudioClipId");
                }
            }
        }

        private void LoadSettings()
        {
            var dataManager = ServiceLocator.Instance.Get<IDataManager>();
            if (dataManager != null)
            {
                var settings = dataManager.LoadData<AudioSettings>("audio_settings");
                if (settings != null)
                {
                    SetMusicVolume(settings.MusicVolume);
                    SetSoundVolume(settings.SoundVolume);
                }
            }
        }

        private void SaveSettings()
        {
            var dataManager = ServiceLocator.Instance.Get<IDataManager>();
            if (dataManager != null)
            {
                var settings = new AudioSettings
                {
                    MusicVolume = musicVolume,
                    SoundVolume = soundVolume
                };
                dataManager.SaveData("audio_settings", settings);
            }
        }

        public void PlaySound(AudioClipId clipId, float volume = 1f)
        {
            if (audioClips.TryGetValue(clipId, out AudioClip clip))
            {
                soundSource.PlayOneShot(clip, volume * soundVolume);
            }
            else
            {
                Debug.LogWarning($"Audio clip not found for {clipId}");
            }
        }

        public void PlayMusic(AudioClipId clipId, float volume = 1f, bool loop = true)
        {
            if (audioClips.TryGetValue(clipId, out AudioClip clip))
            {
                musicSource.clip = clip;
                musicSource.volume = volume * musicVolume;
                musicSource.loop = loop;
                musicSource.Play();
            }
            else
            {
                Debug.LogWarning($"Music clip not found for {clipId}");
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume;
        }

        public void SetSoundVolume(float volume)
        {
            soundVolume = Mathf.Clamp01(volume);
            soundSource.volume = soundVolume;
        }

        private void StopAllAudio()
        {
            musicSource.Stop();
            soundSource.Stop();
        }

        [System.Serializable]
        private class AudioSettings
        {
            public float MusicVolume = 1f;
            public float SoundVolume = 1f;
        }
    }
}