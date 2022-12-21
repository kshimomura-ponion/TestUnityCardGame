using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using TestUnityCardGame.Domain.Service;

namespace TestUnityCardGame.View.Title
{
    public class TitleViewModel : MonoBehaviour
    {
        [SerializeField] Button buttonEntry;  // Entry Button

        [SerializeField] AudioManager audioManager; // Audio Manager

        // Start is called before the first frame update
        private void Awake()
        {
            foreach (var scene in _getPermanentScenesWithoutEntryPoint)
            {
                SceneManager.LoadSceneAsync($"{scene}", LoadSceneMode.Additive);
            }
            buttonEntry.OnClickAsObservable().Delay(TimeSpan.FromSeconds(0.6f)).Subscribe(_ => PushedEnter());
        }

        private void Start()
        {
            audioManager.PlayBGM(BGM.TITLE);
        }

        // Common Systemを除いたパーマネントシーンを読み込みたい順番に設定する
        readonly List<SceneName> _getPermanentScenesWithoutEntryPoint = new()
        {
            SceneName.Common
        };

        public void PushedEnter()
        { 
            // ヒーローセレクト画面へ遷移するプッシュアクションを生成
            var pushToHeroSelectAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.HeroSelect);

            // プッシュアクションのディスパッチ
            MiniUniduxService.Dispatch(pushToHeroSelectAction);
            audioManager.StopBGM();    
        }
    }
}
