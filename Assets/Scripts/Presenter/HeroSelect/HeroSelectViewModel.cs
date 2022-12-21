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
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Service;
using TMPro;

namespace TestUnityCardGame
{
    public class HeroSelectViewModel : SingletonMonoBehaviour<HeroSelectViewModel> 
    {
        [SerializeField] Button battleStartButton;  // button of battle start

        [SerializeField] AudioManager audioManager; // Audio Manager

        [SerializeField] EntitiesManager entitiesManager; // Entities Manager

        [SerializeField] Transform canvasTransform;  // canvas transform

        [SerializeField] HeroInfoDialog heroInfoDialogPrefab; // Hero Infomantion Dialog Prefab  
        private HeroInfoDialog heroInfoDialog = null;

        [SerializeField] AlertDialog alertDialogPrefab; // Alert Dialog Prefab  
        private AlertDialog alertDialog = null;

        // Hero Prefab
        [SerializeField] HeroController heroPrefab;
        [SerializeField] HeroInfoController heroInfoPrefab;

        // Player1SelectTransform
        [SerializeField] Transform player1SelectTransform, player2SelectTransform;

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

        // セレクト終了までの時間管理
        [SerializeField] TextMeshProUGUI untilEndOfSelectText;
        private int maxSeconds = 60;
        private int timeCount;

        private bool isReady = false;

        private void Awake()
        {
            for(int i = 1; i <= existHeroNum; i++){
                CreateHeroInfo(i, PLAYER.PLAYER1, player1SelectTransform, false);
                CreateHeroInfo(i, PLAYER.PLAYER2, player2SelectTransform, false);
            }

            // ランダムセレクトパネルを追加
            CreateHeroInfo(0, PLAYER.PLAYER1, player1SelectTransform, true);
            CreateHeroInfo(0, PLAYER.PLAYER2, player2SelectTransform, true);

            battleStartButton.OnClickAsObservable().Delay(TimeSpan.FromSeconds(0.6f)).Subscribe(_ => OnSelectedHeroes());
    
            StartCoroutine(SelectHeroAndGameStart());

            // ヒーロー情報ダイアログを生成する
            if(heroInfoDialog == null)
            {
                heroInfoDialog = Instantiate(heroInfoDialogPrefab, canvasTransform, false);
            }

            // 警告ダイアログを生成する
            if(alertDialog == null)
            {
                alertDialog = Instantiate(alertDialogPrefab, canvasTransform, false);
            }
        }

        private void Start()
        {
            audioManager.PlayBGM(BGM.SELECTHERO);
        }

        private void CreateHeroInfo(int id, PLAYER player, Transform panel, bool isRandom)
        {
            HeroInfoController heroInfo = Instantiate(heroInfoPrefab, panel, false);
            if(id == 0) id = 1;
            heroInfo.Init(entitiesManager.GetHeroEntity(id), player, isRandom);
        }

        public void ClickedHeroInfoCard(HeroInfoController heroInfo)
        {
            if(heroInfo != null) {
                if(heroInfo.GetOwner() == PLAYER.PLAYER1){
                    int hero1ID = heroInfo.GetHeroInfoID(); // ランダムの場合は0
 
                    if(hero1ID == 0){
                        hero1ID = UnityEngine.Random.Range(1, HeroSelectViewModel.Instance.existHeroNum);
                        heroInfo.UpdateModel(entitiesManager.GetHeroEntity(hero1ID));
                    } 
                    // プレイヤーセレクトOKダイアログを表示する
                    heroInfoDialog.ShowHeroInfoDialog(hero1ID, heroInfo.GetOwner(), heroInfo.model.GetIcon(),heroInfo.model.GetName(),heroInfo.model.GetInfo());
                } else if(heroInfo.GetOwner() == PLAYER.PLAYER2){
                    int hero2ID = heroInfo.GetHeroInfoID(); // ランダムの場合は0
                    if(hero2ID == 0){
                        hero2ID = UnityEngine.Random.Range(1,  HeroSelectViewModel.Instance.existHeroNum);
                        heroInfo.UpdateModel(entitiesManager.GetHeroEntity(hero2ID));
                    }
                    // プレイヤーセレクトOKダイアログを表示する
                heroInfoDialog.ShowHeroInfoDialog(hero2ID, heroInfo.GetOwner(), heroInfo.model.GetIcon(),heroInfo.model.GetName(),heroInfo.model.GetInfo());
                }
            }
        }

        public IEnumerator SelectHeroAndGameStart()
        {
            // カウントダウン開始
            timeCount = maxSeconds;
            untilEndOfSelectText.text = timeCount.ToString();

            while (timeCount > 0)
            {
                yield return new WaitForSeconds(1); // 1秒待機
                timeCount--;
                untilEndOfSelectText.text = timeCount.ToString();
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
            if(hero1ID > 0) {
                // プレイヤー2が決まっていない時はプレイヤー1のHeroを勝手に決めてプレイヤー2をAIにする
                if(hero2ID == 0){
                    hero2ID = UnityEngine.Random.Range(1, existHeroNum);
                    isPlayer2AI = true;
                }

                audioManager.StopBGM();
                StopAllCoroutines();             
                CleanUp();  // いらないオブジェクトの破棄
                
                var battleInitialData = new BattleInitialData(hero1ID, hero2ID, isPlayer2AI);

                // バトル画面へ遷移するプッシュアクションを生成
                var pushToBattleAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.Battle, battleInitialData);

                // プッシュアクションのディスパッチ
                MiniUniduxService.Dispatch(pushToBattleAction);

            } else {
                // 警告ダイアログを出す
                alertDialog.ShowAlertDialog("プレイヤー1を選択してください");
            }
        }

        // いらないオブジェクトの破棄
        void CleanUp()
        {
            HeroInfoController[] hero1InfoList = GetHeroInfoList(PLAYER.PLAYER1);
            foreach (HeroInfoController heroInfo in hero1InfoList)
            {
                Destroy(heroInfo.gameObject);
            }
            HeroInfoController[] hero2InfoList = GetHeroInfoList(PLAYER.PLAYER2);
            foreach (HeroInfoController heroInfo in hero2InfoList)
            {
                Destroy(heroInfo.gameObject);
            }

            if(alertDialog != null)
            {
                Destroy(alertDialog.gameObject);
            }

            if(heroInfoDialog != null)
            {
                Destroy(heroInfoDialog.gameObject);
            }
        }


        public HeroInfoController[] GetHeroInfoList(PLAYER player)
        {
            if(player == PLAYER.PLAYER1){
                return player1SelectTransform.GetComponentsInChildren<HeroInfoController>();
            } else {
                return player2SelectTransform.GetComponentsInChildren<HeroInfoController>();
            }
        }
    }
}