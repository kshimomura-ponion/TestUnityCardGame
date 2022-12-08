using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://qiita.com/simanezumi1989/items/681328f30e88737f57b0

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;

    [SerializeField] List<BGMSoundData> bgmSoundDataList;
    [SerializeField] List<SESoundData> seSoundDataList;
  
    public float masterVolume = 1;
    public float bgmMasterVolume = 1;
    public float seMasterVolume = 1;

    public static SoundController Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    public void PlayBGM(BGMSoundData.BGM bgm)
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

    public void PlaySE(SESoundData.SE se)
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
