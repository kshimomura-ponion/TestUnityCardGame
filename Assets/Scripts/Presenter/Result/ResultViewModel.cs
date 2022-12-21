using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MiniUnidux.SceneTransition;
using TMPro;
using TestUnityCardGame.Domain.Service;

namespace TestUnityCardGame
{
    public class ResultViewModel : MonoBehaviour
    {
        // リスタートボタン
        [SerializeField] Button restartButton;

        // リセレクトボタン
        [SerializeField] Button reselectButton;

        // 結果表示テキスト
        [SerializeField] TextMeshProUGUI resultText;

        // 前のページから受け渡されるデータ
        public ResultInitialData resultInitialData;

    public void Awake()
        {
            // リザルト初期データを受け取る
            resultInitialData = MiniUniduxService.State.Scene.GetData<ResultInitialData>();

            reselectButton
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

            restartButton
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
        }

        public void Start()
        {   
            if(resultInitialData.isPlayer1Win){
                resultText.text = "Player1 WIN!";
            }
            else
            {
                resultText.text = "Player2 WIN!";
            }
        }
    }
}