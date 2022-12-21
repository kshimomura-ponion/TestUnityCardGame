using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniUnidux.Util;

// https://qiita.com/simanezumi1989/items/681328f30e88737f57b0

namespace TestUnityCardGame
{
    [CreateAssetMenu]
    public class SoundManager: SingletonMonoBehaviour<SoundManager>
    {
        [SerializeField] AudioSource bgmAudioSource;
        [SerializeField] AudioSource seAudioSource;

        [SerializeField] List<BGMSoundData> bgmSoundDataList;
        [SerializeField] List<SESoundData> seSoundDataList;
    
        public float masterVolume = 1;
        public float bgmMasterVolume = 1;
        public float seMasterVolume = 1;

        public static SoundManager instance;

        private void Awake()
        {
            if(instance == null){
                instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
  
        public void PlayBGM(BGM bgm)
        {
            BGMSoundData data = bgmSoundDataList.Find(data => data.bgm == bgm);
            bgmAudioSource.clip = data.audioClip;
            bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
            bgmAudioSource.Play();
        }

        public void StopBGM()
        {
            bgmAudioSource.Stop();
        }

        public void PlaySE(SE se)
        {
            SESoundData data = seSoundDataList.Find(data => data.se == se);
            seAudioSource.clip = data.audioClip;
            seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
            seAudioSource.Play();
        }

        public void StopSE()
        {
            seAudioSource.Stop();
        }
    }
}
