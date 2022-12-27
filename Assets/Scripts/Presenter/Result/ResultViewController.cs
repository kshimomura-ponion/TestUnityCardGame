using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Service;
using TestUnityCardGame.Domain.Sound;
using TestUnityCardGame.View.Result;

namespace TestUnityCardGame
{
    public class ResultViewController : SingletonMonoBehaviour<ResultViewController>
    {
        [System.NonSerialized] ResultView resultView;

        [SerializeField] AudioManager audioManager; // Audio Manager

        // 前のページから受け渡されるデータ
        public ResultData resultData;

        protected override void Awake()
        {
            resultView = GetComponent<ResultView>();

            // リザルト初期データを受け取る
            resultData = MiniUniduxService.State.Scene.GetData<ResultData>();

            // AIターン中に飛ばされることがあるためマウスカーソルを有効化しておく
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Start()
        {   
            // ボタンのセット
            resultView.reselectButton.OnClickAsObservable()
                .First()
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

            resultView.restartButton.OnClickAsObservable()
                .First()
                .Subscribe(_ =>
                {
                    // これまでのスタック履歴をリセット（クリア）する
                    var resetAction = PageActionManager<SceneName>.ActionCreator.Reset();
                    // リセットアクションのディスパッチ
                    MiniUniduxService.Dispatch(resetAction);

                    // 前の画面(バトル画面)に遷移するためにポップアクションの生成
                    var battleData = new BattleData(resultData.hero1ID, resultData.hero2ID, resultData.isPlayer2AI, resultData.existHeroNum, resultData.existCardNum);

                    var returnToBattleAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.Battle, battleData);

                    // ポップアクションのディスパッチ
                    MiniUniduxService.Dispatch(returnToBattleAction);
                })
                .AddTo(this);  

            // 音楽を再生
            audioManager.PlayBGM(BGM.Result);

            if (resultData.isPlayer1Win) {
                resultView.SetResultText("Player1 WIN!");
            } else {
                resultView.SetResultText("Player2 WIN!");
            }
        }
    }
}