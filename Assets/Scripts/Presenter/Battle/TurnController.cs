using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MiniUnidux;
using MiniUnidux.Util;
using TMPro;
using TestUnityCardGame.Domain.Service;

namespace TestUnityCardGame
{
    public class TurnController: SingletonMonoBehaviour<TurnController>
    {
        [SerializeField] GameObject turnEndButtonObject;

        [SerializeField] Transform canvasTransform;  // canvas transform

        // ターン情報ビュー
        [SerializeField] TurnInfoView turnInfoViewPrefab;
        TurnInfoView turnInfoView;

        // プレイヤー1のターンかどうか識別する
        [System.NonSerialized] public bool isPlayer1Turn;

        // ターン数（メイン画面）
        [SerializeField] TextMeshProUGUI turnNumText;

        // ターン終了までの時間管理
        [SerializeField] TextMeshProUGUI untilEndOfTurnText;
        private int maxSeconds = 10;
        private int timeCount;

        // 敵AI
        [SerializeField] EnemyAI enemyAI;

        // ターン終了フラグ
        bool isTurnEnd;

        private void Awake()
        {
            // ターン情報ビューを生成する
            if(turnInfoView == null)
            {
                turnInfoView = Instantiate(turnInfoViewPrefab, canvasTransform, false);
            }
       }

        private void OnDestroy()
        {
        if(turnInfoView != null)
            {
                Destroy(turnInfoView.gameObject);
            }
        }
        public void TurnStart()
        {
            isTurnEnd = false;
            // カウントダウン開始
            StopAllCoroutines();
            StartCoroutine(CountDown());
            
            if(isPlayer1Turn){
                // ターン数を表示する
                int turnNum = BattleViewModel.Instance.player1Hero.GetTurnNumber();
                turnInfoView.ShowTurnInfoView(turnNum);
                turnNumText.text = BattleViewModel.Instance.player1Hero.GetTurnNumber().ToString();

                // 情報パネルが過ぎるまで待つ
                Invoke("PlayerTurn", 3.5f);
            } else {
                // ターン数を表示する
                var turnNum = BattleViewModel.Instance.player2Hero.GetTurnNumber();
                turnInfoView.ShowTurnInfoView(turnNum);
                turnNumText.text = BattleViewModel.Instance.player2Hero.GetTurnNumber().ToString();

                if(BattleViewModel.Instance.battleInitialData.isPlayer2AI == true){
                    StartCoroutine(enemyAI.EnemyTurn());
                    // ターンエンドボタンを押せなくする
                    TurnendButtonActivate(false);
                } else {
                    // 情報パネルが過ぎるまで待つ
                    Invoke("PlayerTurn", 3.5f);
                    // ターンエンドボタンを押せるようにする
                    TurnendButtonActivate(true);
                }
            }
        }

        public IEnumerator CountDown()
        {
            // 情報パネルが過ぎるまで待つ
            yield return new WaitForSeconds(4.0f);

            timeCount = maxSeconds;
            untilEndOfTurnText.text = timeCount.ToString();
            while (timeCount > 0)
            {
                yield return new WaitForSeconds(1); // 1秒待機
                timeCount--;
                untilEndOfTurnText.text = timeCount.ToString();
            }
            ChangeTurn();
        }

        public void PlayerTurn()
        {
            CardController[] playerFieldCardList;
            if(isPlayer1Turn){
                BattleViewModel.Instance.player1Hero.view.SetActiveActivatedPanel(true);
                playerFieldCardList = BattleViewModel.Instance.GetFriendFieldCards(PLAYER.PLAYER1);
            } else {
                BattleViewModel.Instance.player2Hero.view.SetActiveActivatedPanel(true);
                playerFieldCardList = BattleViewModel.Instance.GetFriendFieldCards(PLAYER.PLAYER2);
            }

            // 攻撃表示に変更
            SettingCanAttackView(playerFieldCardList,true);
            Debug.Log("Playerのターン");

            if(isPlayer1Turn){
                BattleViewModel.Instance.player1Hero.AddTurnNumber();  // Player1のターン数を増やす
                OpenPlayerHandsCard(PLAYER.PLAYER1);      // Player2の手札を全てOpenにする
            } else {
                BattleViewModel.Instance.player2Hero.AddTurnNumber();  // Player2のターン数を増やす
                OpenPlayerHandsCard(PLAYER.PLAYER2);      // Player2の手札を全てOpenにする
            }
        }

        public void OpenPlayerHandsCard(PLAYER player){
            CardController[] playerCardList = {};
            if(player == PLAYER.PLAYER1){
                playerCardList = BattleViewModel.Instance.player1HandTransform.GetComponentsInChildren<CardController>();
            } else {
                playerCardList = BattleViewModel.Instance.player2HandTransform.GetComponentsInChildren<CardController>();
            }

            foreach(CardController card in playerCardList)
            {
                card.view.SetActiveFrontPanel(true);
            }
        }

        public void ClosePlayerHandsCard(PLAYER player){
            CardController[] playerCardList = {};
            if(player == PLAYER.PLAYER1){
                playerCardList = BattleViewModel.Instance.player1HandTransform.GetComponentsInChildren<CardController>();
            } else {
                playerCardList = BattleViewModel.Instance.player2HandTransform.GetComponentsInChildren<CardController>();
            }
            
            foreach(CardController card in playerCardList)
            {
                card.view.SetActiveFrontPanel(false);
            }
        }
        
        public void CardsBattle(CardController attacker, CardController defender)
        {
            // ダメージを計算し、Viewのダメージ情報パネルを更新する
            defender.view.SetDamageInfoText("-" + defender.Attack(attacker).ToString());
            attacker.CheckAlive();

            attacker.view.SetDamageInfoText("-" + attacker.Attack(defender).ToString());
            defender.CheckAlive();
        }

        public void ChangeTurn()
        {
            // ターンが変わったらタイマーの文字を元に戻す
            untilEndOfTurnText.text = maxSeconds.ToString();

            isTurnEnd = true;

            CardController[] player1FieldCardList = BattleViewModel.Instance.player1FieldTransform.GetComponentsInChildren<CardController>();
            SettingCanAttackView(player1FieldCardList, false);
            CardController[] player2FieldCardList = BattleViewModel.Instance.player2FieldTransform.GetComponentsInChildren<CardController>();
            SettingCanAttackView(player2FieldCardList, false);

            isPlayer1Turn = !isPlayer1Turn;

            if (isPlayer1Turn) {
                // マナコストを+1してからターン開始
                BattleViewModel.Instance.player1Hero.AddManaCost(1);
                // カードを手札に加える
                BattleViewModel.Instance.GiveCardToHand(BattleViewModel.Instance.player1Hero, BattleViewModel.Instance.player1HandTransform, PLAYER.PLAYER1);
                BattleViewModel.Instance.player2Hero.view.SetActiveActivatedPanel(false);
            
                ClosePlayerHandsCard(PLAYER.PLAYER1); // ターンが終わったので手札を隠す
            } else {
                // マナコストを+1してからターン開始
                BattleViewModel.Instance.player2Hero.AddManaCost(1);
                // カードを手札に加える
                BattleViewModel.Instance.GiveCardToHand(BattleViewModel.Instance.player2Hero, BattleViewModel.Instance.player2HandTransform, PLAYER.PLAYER2);
                BattleViewModel.Instance.player1Hero.view.SetActiveActivatedPanel(false);

                ClosePlayerHandsCard(PLAYER.PLAYER2); // ターンが終わったので手札を隠す
            }
            
            TurnStart();
        }

        public void HealToHero(CardController healer)
        {
            if (healer.GetOwner() == PLAYER.PLAYER1){
                BattleViewModel.Instance.player1Hero.Healed(healer);
            } else {
                BattleViewModel.Instance.player2Hero.Healed(healer);
            }
        }

        void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
        {
            foreach (CardController card in fieldCardList)
            {
                card.SetCanAttack(canAttack);
            }
        }

        public void AddTurnNumber(HeroController hero)
        {
            hero.AddTurnNumber();
        }

        public void TurnendButtonActivate(bool activeState)
        {
            turnEndButtonObject.SetActive(activeState);
        }
    }
}