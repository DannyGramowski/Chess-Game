using Chess.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Core {
    public class AudioManager : SingletonPersistent<AudioManager> {
        [SerializeField] AudioSource musicPlayer;
        [SerializeField] AudioSaveData audioSaveData;

        private new void Awake() {
            base.Awake();
            UpdateAudioSettings();

            if (PlayerPrefs.HasKey("music volume")) {
                audioSaveData.SetMusicVolume(PlayerPrefs.GetFloat("music volume"));
                audioSaveData.SetSfxVolume(PlayerPrefs.GetFloat("sfx volume"));
                audioSaveData.SetMusicMuted(PlayerPrefs.GetInt("music muted") == 1 ? true : false);
                audioSaveData.SetSfxMuted(PlayerPrefs.GetInt("sfx muted") == 1 ? true : false);
                UpdateAudioSettings();
            }

        }

        public void SetMusic(AudioClip music) {
            musicPlayer.clip = music;
            UpdateMusicSettings();
            musicPlayer.Play();
        }

        public void UpdateAudioSettings() {
            UpdateMusicSettings();
        }

        public void UpdateMusicSettings() {
            musicPlayer.volume = audioSaveData.MusicVolume;
            musicPlayer.mute = audioSaveData.MusicMuted;
        }

        public void OnDisable() {
            PlayerPrefs.SetFloat("music volume", audioSaveData.MusicVolume);
            PlayerPrefs.SetFloat("sfx volume", audioSaveData.SfxVolume);
            PlayerPrefs.SetInt("music muted", audioSaveData.MusicMuted ? 1 : 0);
            PlayerPrefs.SetInt("sfx muted", audioSaveData.SfxMuted ? 1 : 0);

        }
    }
}
