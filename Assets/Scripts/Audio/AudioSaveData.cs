using System.Collections;
using UnityEngine;

namespace Chess.Audio {
    [CreateAssetMenu(fileName = "new audio save data", menuName = "Create Audio Save Data")]

    public class AudioSaveData : ScriptableObject {
        bool musicMuted;
        float musicVolume;

        bool sfxMuted;
        float sfxVolume;

        public bool MusicMuted => musicMuted;
        public float MusicVolume => musicVolume;
        public bool SfxMuted => sfxMuted;
        public float SfxVolume => sfxVolume;


        public void SetMusicMuted(bool musicMuted) {
            this.musicMuted = musicMuted;
        }
        public void SetMusicVolume(float musicVolume) {
            this.musicVolume = musicVolume;
        }
        public void SetSfxMuted(bool sfxMuted) {
            this.sfxMuted = sfxMuted;
        }
        public void SetSfxVolume(float sfxVolume) {
            this.sfxVolume = sfxVolume;
        }
    }
}