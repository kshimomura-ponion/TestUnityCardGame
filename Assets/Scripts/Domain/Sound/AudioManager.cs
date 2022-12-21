using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://qiita.com/simanezumi1989/items/681328f30e88737f57b0

namespace TestUnityCardGame
{
    public class AudioManager: ScriptableObject
    {
        public AudioSource bgmAudioSource{get; set;}
        public AudioSource seAudioSource{get; set;}

        [SerializeField] List<BGMSoundData> bgmSoundDataList;
        [SerializeField] List<SESoundData> seSoundDataList;
    
        public float masterVolume = 1;
        public float bgmMasterVolume = 1;
        public float seMasterVolume = 1;
  
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

        public void PlaySEOK()
        {
            PlaySE(SE.OK);
        }

        public void PlaySECancel()
        {
            PlaySE(SE.CANCEL);
        }

        public void PlaySE(SE se)
        {
            SESoundData data = seSoundDataList.Find(data => data.se == se);
            seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
            seAudioSource.PlayOneShot(data.audioClip);
            //seAudioSource.clip = data.audioClip;
            //seAudioSource.Play();
        }

        /*public void StopSE()
        {
            seAudioSource.Stop();
        }*/
    }
}
