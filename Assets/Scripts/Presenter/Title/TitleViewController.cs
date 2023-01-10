using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Common;
using TestUnityCardGame.Presenter.Common;
using TestUnityCardGame.Domain.Service;
using TestUnityCardGame.View.Title;

namespace TestUnityCardGame.Presenter.Title
{
    public class TitleViewController : SingletonMonoBehaviour<TitleViewController>
    {
        TitleView titleView;

        private SoundManager soundManager;

        // Start is called before the first frame update
        protected override void Awake()
        {
            titleView = GetComponent<TitleView>();

            foreach (var scene in _getPermanentScenesWithoutEntryPoint) {
                SceneManager.LoadSceneAsync($"{scene}", LoadSceneMode.Additive);
            }
        }

        public void Start()
        {

            // CommonからSoundManagerを取得する
            soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");

            if (soundManager != null) soundManager.PlayBGM(BGM.Title);

            titleView.buttonEntry.OnClickAsObservable()
                .First()
                .Delay(TimeSpan.FromSeconds(0.6f))
                .Subscribe(_ => PushedEnter())
                .AddTo(this);
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
            soundManager.StopBGM();    
        }
    }
}
