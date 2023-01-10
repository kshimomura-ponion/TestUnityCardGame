using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Common;

// https://qiita.com/simanezumi1989/items/681328f30e88737f57b0

namespace TestUnityCardGame.Presenter.Common
{
    public class SoundManager: SingletonMonoBehaviour<SoundManager>
    {
        [SerializeField] AudioSource audioSourceOfBGM;
        [SerializeField] AudioSource audioSourceOfSE;

        [SerializeField] List<BGMSoundData> soundDataListOfBGM;
        [SerializeField] List<SESoundData> soundDataListOfSE;
    
        public float masterVolume = 1;
        public float bgmMasterVolume = 1;
        public float seMasterVolume = 1;
  
        public void PlayBGM(BGM bgm)
        {
            BGMSoundData data = soundDataListOfBGM.Find(data => data.bgm == bgm);
            audioSourceOfBGM.clip = data.audioClip;
            audioSourceOfBGM.volume = data.volume * bgmMasterVolume * masterVolume;
            audioSourceOfBGM.Play();
        }

        public void StopBGM()
        {
            audioSourceOfBGM.Stop();
        }

        public void PlaySE(SE se)
        {
            SESoundData data = soundDataListOfSE.Find(data => data.se == se);
            audioSourceOfSE.volume = data.volume * seMasterVolume * masterVolume;
            audioSourceOfSE.PlayOneShot(data.audioClip);
            audioSourceOfSE.clip = data.audioClip;
            audioSourceOfSE.Play();
        }
    }
}
