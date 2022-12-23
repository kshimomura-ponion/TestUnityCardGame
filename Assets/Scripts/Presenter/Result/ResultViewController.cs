using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Service;
using TestUnityCardGame.View.Result;

namespace TestUnityCardGame
{
    public class ResultViewController : SingletonMonoBehaviour<ResultViewController>
    {
        [System.NonSerialized] ResultView resultView;

        [SerializeField] AudioManager audioManager; // Audio Manager

        // 前のページから受け渡されるデータ
        public ResultInitialData resultInitialData;

        protected override void Awake()
        {
            resultView = GetComponent<ResultView>();

            // リザルト初期データを受け取る
            resultInitialData = MiniUniduxService.State.Scene.GetData<ResultInitialData>();
        }

        public void Start()
        {   
            // ボタンのセット
            resultView.reselectButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                // これまでのスタック履歴をリセット（クリア）する
                var resetAction = PageActionManager<SceneName>.ActionCreator.Reset();
                // リセットアクションのディスパッチ
                MiniUniduxService.Dispatch(resetAction);

                // 前の画面(ヒーローセレクト画面)に遷移するためにポップアクションの生成
                var returnToHeroSelectAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.HeroSelect);

                // ポップアクションのディスパッチ
                MiniUniduxService.Dispatch(returnToHeroSelectAction);
            })
            .AddTo(this);

            resultView.restartButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                // これまでのスタック履歴をリセット（クリア）する
                var resetAction = PageActionManager<SceneName>.ActionCreator.Reset();
                // リセットアクションのディスパッチ
                MiniUniduxService.Dispatch(resetAction);

                // 前の画面(バトル画面)に遷移するためにポップアクションの生成
                var returnToBattleAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.Battle);
                // ポップアクションのディスパッチ
                MiniUniduxService.Dispatch(returnToBattleAction);
            })
            .AddTo(this);  

            // 音楽を再生
            audioManager.PlayBGM(BGM.Result);

            if (resultInitialData.isPlayer1Win) {
                resultView.SetResultText("Player1 WIN!");
            } else {
                resultView.SetResultText("Player2 WIN!");
            }
        }
    }
}