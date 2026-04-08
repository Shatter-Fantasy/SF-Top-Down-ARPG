using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SF.AudioModule
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public List<AudioChannelSettings> AudioChannelSettingsList = new();

        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<AudioManager>();

                    // If no AudioManager was found in the scene make one than set it as the instance for the AudioManager.
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("Audio Manager", typeof(AudioManager));
                        Instantiate(go);
                        _instance = go.GetComponent<AudioManager>();
                    }
                }

                return _instance;
            }
            set
            {
                if (_instance == null)
                    _instance = value;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                // We destroy the Audio Manager Gameobject to make sure we clean up the Audio Source components as well.
                // If we don't than the music will persist from scene to new scene.
                Destroy(_instance.gameObject);
            }

            _instance = this;
        }

        /// <summary>
        /// Plays a sound effect through the cached Audio Source in the Audio Manager.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="audioChannelType"></param>
        public void PlayOneShot(AudioClip audioClip, AudioChannelType audioChannelType = AudioChannelType.SFX)
        {
            if (_instance == null)
                return;

            var audioChannel = GetAudioChannel(audioChannelType);
            audioChannel.AudioSource.PlayOneShot(audioClip, audioChannel.Volume);
        }

        public static AudioChannelSettings GetAudioChannel(AudioChannelType audioChannelType)
        {
            return _instance.AudioChannelSettingsList
                         .FirstOrDefault(setting => setting.AudioChannelType == audioChannelType);
        }
        
        public static float GetChannelVolume(AudioChannelType audioChannelType)
        {
            return _instance.AudioChannelSettingsList
                         .FirstOrDefault(setting => setting.AudioChannelType == audioChannelType)
                         .Volume;
        }

        [Serializable]
        public struct AudioChannelSettings
        {
            public float Volume;
            public AudioSource AudioSource;
            public AudioChannelType AudioChannelType;

            public AudioChannelSettings(float volume = .75f, 
                AudioChannelType audioChannelType = AudioChannelType.Music)
            {
                Volume           = volume;
                AudioChannelType = audioChannelType;
                AudioSource = null;
            }
        }
        
        public enum AudioChannelType : ushort
        {
            Master = 0,
            Music = 1,
            SFX = 2,
            UI = 4,
            Other = 8,
        }
    }
}
