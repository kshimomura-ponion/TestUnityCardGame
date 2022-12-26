using System.Net;
using System.Security.AccessControl;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Service;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.View.HeroSelect;

namespace TestUnityCardGame.Presenter.HeroSelect
{
    public class HeroSelectViewController : SingletonMonoBehaviour<HeroSelectViewController> 
    {

        HeroSelectView heroSelectView;
        [SerializeField] AudioManager audioManager; // Audio Manager
        [SerializeField] EntitiesManager entitiesManager; // Entities Manager

        // 現在用意できているヒーロー、カードの種類の数
        [System.NonSerialized] public int existHeroNum = 6;
        [System.NonSerialized] public int existCardNum = 8;

        // Hero
        [System.NonSerialized] public int hero1ID = 0;
        [System.NonSerialized] public int hero2ID = 0;

        // プレイヤー2はAIか否か
        [System.NonSerialized] public bool isPlayer2AI;

        // Heroの実体
        [System.NonSerialized] public HeroController player1Hero, player2Hero;

        // Player1SelectTransform
        public Transform player1SelectTransform, player2SelectTransform;

        // セレクト終了までの時間管理
        private int maxSeconds = 60;
        private int timeCount;

        //private bool isReady = false;

        protected override void Awake()
        {
            heroSelectView = GetComponent<HeroSelectView>();
        }

        public void Start()
        {
            audioManager.PlayBGM(BGM.SelectHero);

            for(int i = 1; i <= existHeroNum; i++) {
                CreateHeroInfo(i, Player.Player1, player1SelectTransform, false);
                CreateHeroInfo(i, Player.Player2, player2SelectTransform, false);
            }

            // ランダムセレクトパネルを追加
            CreateHeroInfo(0, Player.Player1, player1SelectTransform, true);
            CreateHeroInfo(0, Player.Player2, player2SelectTransform, true);

            heroSelectView.battleStartButton.OnClickAsObservable().Delay(TimeSpan.FromSeconds(0.6f)).Subscribe(_ => OnSelectedHeroes());
    
            StartCoroutine(SelectHeroAndGameStart());
        }

        private void CreateHeroInfo(int id, Player player, Transform panel, bool isRandom)
        {
            HeroInfoController heroInfo = Instantiate(heroSelectView.GetHeroInfoPrefab(), panel, false);
            if (id == 0) id = 1;
            heroInfo.Init(entitiesManager.GetHeroEntity(id), player, isRandom);
        }

        public void ClickedHeroInfoCard(HeroInfoController heroInfo)
        {
            if (heroInfo != null) {
                if (heroInfo.GetOwner() == Player.Player1) {
                    int hero1ID = heroInfo.GetHeroInfoID(); // ランダムの場合は0
 
                    if (hero1ID == 0) {
                        hero1ID = UnityEngine.Random.Range(1, existHeroNum);
                        heroInfo.UpdateModel(entitiesManager.GetHeroEntity(hero1ID));
                    } 
                    // プレイヤーセレクトOKダイアログを表示する
                    heroSelectView.ShowHeroInfoDialog(hero1ID, heroInfo.GetOwner(), heroInfo.model.GetIcon(),heroInfo.model.GetName(),heroInfo.model.GetInfo());
                } else if (heroInfo.GetOwner() == Player.Player2) {
                    int hero2ID = heroInfo.GetHeroInfoID(); // ランダムの場合は0
                    if (hero2ID == 0) {
                        hero2ID = UnityEngine.Random.Range(1,  existHeroNum);
                        heroInfo.UpdateModel(entitiesManager.GetHeroEntity(hero2ID));
                    }
                    // プレイヤーセレクトOKダイアログを表示する
                heroSelectView.ShowHeroInfoDialog(hero2ID, heroInfo.GetOwner(), heroInfo.model.GetIcon(),heroInfo.model.GetName(),heroInfo.model.GetInfo());
                }
            }
        }

        public IEnumerator SelectHeroAndGameStart()
        {
            // カウントダウン開始
            timeCount = maxSeconds;
            heroSelectView.SetUntilEndOfSelectText(timeCount.ToString());

            while (timeCount > 0)
            {
                yield return new WaitForSeconds(1); // 1秒待機
                timeCount--;
                heroSelectView.SetUntilEndOfSelectText(timeCount.ToString());
            }

            // 一定時間過ぎてもボタンが押されなかった場合はプレイヤー1のHeroを勝手に決めてプレイヤー2をAIにする
            hero1ID = UnityEngine.Random.Range(1, existHeroNum);
            hero2ID = UnityEngine.Random.Range(1, existHeroNum);
            isPlayer2AI = true;

            yield return new WaitForSeconds(0.05f);

            OnSelectedHeroes();
        }

        public void OnSelectedHeroes()
        {
            if (hero1ID > 0) {
                // プレイヤー2が決まっていない時はプレイヤー1のHeroを勝手に決めてプレイヤー2をAIにする
                if (hero2ID == 0) {
                    hero2ID = UnityEngine.Random.Range(1, existHeroNum);
                    isPlayer2AI = true;
                }

                audioManager.StopBGM();
                StopAllCoroutines();             
                CleanUp();  // いらないオブジェクトの破棄
                
                var battleData = new BattleData(hero1ID, hero2ID, isPlayer2AI, existHeroNum, existCardNum);

                // バトル画面へ遷移するプッシュアクションを生成
                var pushToBattleAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.Battle, battleData);

                // プッシュアクションのディスパッチ
                MiniUniduxService.Dispatch(pushToBattleAction);

            } else {
                // 警告ダイアログを出す
                heroSelectView.alertDialog.ShowAlertDialog("プレイヤー1を選択してください");
            }
        }

        // いらないオブジェクトの破棄
        void CleanUp()
        {
            HeroInfoController[] hero1InfoList = GetHeroInfoList(Player.Player1);
            foreach (HeroInfoController heroInfo in hero1InfoList)
            {
                Destroy(heroInfo.gameObject);
            }
            HeroInfoController[] hero2InfoList = GetHeroInfoList(Player.Player2);
            foreach (HeroInfoController heroInfo in hero2InfoList)
            {
                Destroy(heroInfo.gameObject);
            }
        }

        public HeroInfoController[] GetHeroInfoList(Player player)
        {
            if (player == Player.Player1) {
                return player1SelectTransform.GetComponentsInChildren<HeroInfoController>();
            } else {
                return player2SelectTransform.GetComponentsInChildren<HeroInfoController>();
            }
        }
    }
}