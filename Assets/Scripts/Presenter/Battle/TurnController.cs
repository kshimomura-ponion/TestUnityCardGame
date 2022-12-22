using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MiniUnidux;
using MiniUnidux.Util;
using TMPro;
using TestUnityCardGame.View.Battle;

namespace TestUnityCardGame.Presenter.Battle
{
    public class TurnController: SingletonMonoBehaviour<TurnController>
    {
        [SerializeField] TurnInfoViewController turnInfoViewController;

        // プレイヤー1のターンかどうか識別する
        [System.NonSerialized] public bool isPlayer1Turn;

        // ターン終了までの時間管理
        [SerializeField] TextMeshProUGUI untilEndOfTurnText;
        private int maxSeconds = 10;
        private int timeCount;

        // 敵AI
        [SerializeField] EnemyAI enemyAI;

        // ターン終了フラグ
        bool isTurnEnd;

        private void Awake(){}

        public void TurnStart()
        {
            isTurnEnd = false;
            // カウントダウン開始
            StopAllCoroutines();
            StartCoroutine(CountDown());
            
            if(isPlayer1Turn){
                // ターン数を表示する
                int turnNum = BattleViewController.Instance.player1Hero.GetTurnNumber();
                turnInfoViewController.ShowTurnInfoView(turnNum);
                BattleViewController.Instance.SetTurnNumText(BattleViewController.Instance.player1Hero.GetTurnNumber().ToString());

                // 情報パネルが過ぎるまで待つ
                Invoke("PlayerTurn", 3.5f);
            } else {
                // ターン数を表示する
                var turnNum = BattleViewController.Instance.player2Hero.GetTurnNumber();
                turnInfoViewController.ShowTurnInfoView(turnNum);
                BattleViewController.Instance.SetTurnNumText(BattleViewController.Instance.player2Hero.GetTurnNumber().ToString());

                if(BattleViewController.Instance.battleInitialData.isPlayer2AI == true){
                    StartCoroutine(enemyAI.EnemyTurn());
                    // ターンエンドボタンを押せなくする
                    BattleViewController.Instance.TurnendButtonActivate(false);
                } else {
                    // 情報パネルが過ぎるまで待つ
                    Invoke("PlayerTurn", 3.5f);
                    // ターンエンドボタンを押せるようにする
                    BattleViewController.Instance.TurnendButtonActivate(true);
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
                BattleViewController.Instance.player1Hero.view.SetActiveActivatedPanel(true);
                playerFieldCardList = BattleViewController.Instance.GetFriendFieldCards(Player.Player1);
                
                // カードのコストとPlayerのMana Costを比較してドラッグ可能かどうか判定する
                CardController[] handCardList = BattleViewController.Instance.GetMyHandCards(Player.Player1);
                SettingIsDraggableFromManaCost(handCardList, BattleViewController.Instance.player1Hero);

            } else {
                BattleViewController.Instance.player2Hero.view.SetActiveActivatedPanel(true);
                playerFieldCardList = BattleViewController.Instance.GetFriendFieldCards(Player.Player2);

                // カードのコストとPlayerのMana Costを比較してドラッグ可能かどうか判定する
                CardController[] handCardList = BattleViewController.Instance.GetMyHandCards(Player.Player2);
                SettingIsDraggableFromManaCost(handCardList, BattleViewController.Instance.player2Hero);
            }

            // 攻撃表示に変更
            SettingCanAttackView(playerFieldCardList,true);
            Debug.Log("Playerのターン");

            if(isPlayer1Turn){
                BattleViewController.Instance.player1Hero.AddTurnNumber();  // Player1のターン数を増やす
                OpenPlayerHandsCard(Player.Player1);      // Player2の手札を全てOpenにする
            } else {
                BattleViewController.Instance.player2Hero.AddTurnNumber();  // Player2のターン数を増やす
                OpenPlayerHandsCard(Player.Player2);      // Player2の手札を全てOpenにする
            }
        }

        public void OpenPlayerHandsCard(Player player){
            CardController[] playerCardList = {};
            if(player == Player.Player1){
                Transform player1HandTransform = BattleViewController.Instance.GetPlayer1HandTransform();
                playerCardList = player1HandTransform.GetComponentsInChildren<CardController>();
            } else {
                Transform player2HandTransform = BattleViewController.Instance.GetPlayer2HandTransform();
                playerCardList = player2HandTransform.GetComponentsInChildren<CardController>();
            }

            foreach(CardController card in playerCardList)
            {
                card.view.SetActiveFrontPanel(true);
            }
        }

        public void ClosePlayerHandsCard(Player player){
            CardController[] playerCardList = {};
            if(player == Player.Player1){
                Transform player1HandTransform =  BattleViewController.Instance.GetPlayer1HandTransform();
                playerCardList = player1HandTransform.GetComponentsInChildren<CardController>();
            } else {
                Transform player2HandTransform = BattleViewController.Instance.GetPlayer2HandTransform();
                playerCardList = player2HandTransform.GetComponentsInChildren<CardController>();
            }
            
            foreach(CardController card in playerCardList)
            {
                card.view.SetActiveFrontPanel(false);
            }
        }

        public void ChangeTurn()
        {
            // ターンが変わったらタイマーの文字を元に戻す
            untilEndOfTurnText.text = maxSeconds.ToString();

            isTurnEnd = true;
            Transform player1FieldTransform = BattleViewController.Instance.GetPlayer1FieldTransform();
            CardController[] player1FieldCardList = player1FieldTransform.GetComponentsInChildren<CardController>();
            SettingCanAttackView(player1FieldCardList, false);

            Transform player2FieldTransform = BattleViewController.Instance.GetPlayer2FieldTransform();
            CardController[] player2FieldCardList = player2FieldTransform.GetComponentsInChildren<CardController>();
            SettingCanAttackView(player2FieldCardList, false);

            isPlayer1Turn = !isPlayer1Turn;

            if (isPlayer1Turn) {
                // マナコストを+1してからターン開始
                BattleViewController.Instance.player1Hero.AddManaCost(1);
                // カードを手札に加える
                BattleViewController.Instance.GiveCardToHand(BattleViewController.Instance.player1Hero, BattleViewController.Instance.GetPlayer1HandTransform(), Player.Player1);
                BattleViewController.Instance.player2Hero.view.SetActiveActivatedPanel(false);
            
                ClosePlayerHandsCard(Player.Player1); // ターンが終わったので手札を隠す
            } else {
                // マナコストを+1してからターン開始
                BattleViewController.Instance.player2Hero.AddManaCost(1);
                // カードを手札に加える
                BattleViewController.Instance.GiveCardToHand(BattleViewController.Instance.player2Hero, BattleViewController.Instance.GetPlayer2HandTransform(), Player.Player2);
                BattleViewController.Instance.player1Hero.view.SetActiveActivatedPanel(false);

                ClosePlayerHandsCard(Player.Player2); // ターンが終わったので手札を隠す
            }
            
            TurnStart();
        }

        // カードのコストとPlayerのMana Costを比較してドラッグ可能かどうか判定する
        void SettingIsDraggableFromManaCost(CardController[] cardList, HeroController hero)
        {
            foreach (CardController card in cardList)
            {
                if(card.model.GetManaCost() <= hero.model.GetManaCost()) {
                    card.SetDraggable(true);
                } else {
                    card.SetDraggable(false);
                }
            }
        }

        void SettingCanAttackView(CardController[] cardList, bool canAttack)
        {
            foreach (CardController card in cardList)
            {
                card.SetCanAttack(canAttack);
            }
        }

        public void AddTurnNumber(HeroController hero)
        {
            hero.AddTurnNumber();
        }


        public void CheckHeroHP()
        {
            if (BattleViewController.Instance.player1Hero.model.GetHP() <= 0 || BattleViewController.Instance.player2Hero.model.GetHP() <= 0){
                BattleViewController.Instance.GameOver();
            }
        }
    }
}