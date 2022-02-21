using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Audio {
    public class SettingMenu : MonoBehaviour {
        [SerializeField] AudioSaveData audioSaveData;
        [SerializeField] Core.AudioManager audioManager;
        [SerializeField] Slider musicVolumeSlider;
        [SerializeField] Toggle musicMuteToggle;
        [SerializeField] Slider sfxVolumeSlider;
        [SerializeField] Toggle sfxMuteToggle;

        private void OnEnable() {
            musicMuteToggle.isOn = audioSaveData.MusicMuted;
            musicVolumeSlider.value = audioSaveData.MusicVolume;
            sfxMuteToggle.isOn = audioSaveData.SfxMuted;
            sfxVolumeSlider.value = audioSaveData.SfxVolume;
        }

        public void UpdateValues() {
            audioSaveData.SetMusicMuted(musicMuteToggle.isOn);
            audioSaveData.SetMusicVolume(musicVolumeSlider.value);
            audioSaveData.SetSfxMuted(sfxMuteToggle.isOn);
            audioSaveData.SetSfxVolume(sfxVolumeSlider.value);

            audioManager.UpdateAudioSettings();
        }
    }
}