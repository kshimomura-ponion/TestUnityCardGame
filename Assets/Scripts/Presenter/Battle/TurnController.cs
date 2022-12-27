using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MiniUnidux;
using MiniUnidux.Util;
using TMPro;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Card;
using TestUnityCardGame.View.Battle;

namespace TestUnityCardGame.Presenter.Battle
{
    public class TurnController: SingletonMonoBehaviour<TurnController>
    {
        public Transform canvasTransform;  // canvas transform
        [System.NonSerialized] TurnInfoView turnInfoView;

        // ターン終了までの時間管理
        [SerializeField] TextMeshProUGUI untilEndOfTurnText;
        private int maxSeconds = 10;
        private int timeCount;

        // 敵AI
        [SerializeField] EnemyAI enemyAI;

        protected override void Awake()
        {
            // ターン情報ビューを生成する
            if (turnInfoView == null) {
                turnInfoView = Instantiate(BattleViewController.Instance.GetTurnInfoViewPrefab(), canvasTransform, false);
            }
        }

        public void TurnStart()
        {
            // カウントダウン開始
            StopAllCoroutines();
            StartCoroutine(CountDown());

            // ドラッグイベントそのものをON/OFFする
            SetAllCardsDragAndDropEventEnable();
            
            if (BattleViewController.Instance.isPlayer1Turn) {
                // ターン数を表示する
                int turnNum = BattleViewController.Instance.player1Hero.GetTurnNumber();
                turnInfoView.ShowTurnInfoView(turnNum);
                turnInfoView.StartAnimation();
                BattleViewController.Instance.SetTurnNumText(BattleViewController.Instance.player1Hero.GetTurnNumber().ToString());

                // ターンエンドボタンを押せるようにする
                BattleViewController.Instance.TurnendButtonActivate(true);

                BattleViewController.Instance.player1Hero.AddTurnNumber();  // Player1のターン数を増やす
                OpenPlayerHandsCard(Player.Player1);      // Player1の手札を全てOpenにする
                ClosePlayerHandsCard(Player.Player2);     // Player2の手札を全てCloseにする

                BattleViewController.Instance.UpdateCardSettingsWithManaCost(Player.Player1);   // ドラッグ、スペルカードの攻撃表示の設定
                SetAllCardsInFieldCanAttack(Player.Player1); // モンスターカードの攻撃表示の設定

                PlayerTurn(); // ターン開始
            } else {
                // ターン数を表示する
                var turnNum = BattleViewController.Instance.player2Hero.GetTurnNumber();
                turnInfoView.ShowTurnInfoView(turnNum);
                turnInfoView.StartAnimation();
                BattleViewController.Instance.SetTurnNumText(BattleViewController.Instance.player2Hero.GetTurnNumber().ToString());

                BattleViewController.Instance.player2Hero.AddTurnNumber();  // Player2のターン数を増やす
                OpenPlayerHandsCard(Player.Player2);      // Player2の手札を全てOpenにする
                ClosePlayerHandsCard(Player.Player1);     // Player1の手札を全てCloseにする

                BattleViewController.Instance.UpdateCardSettingsWithManaCost(Player.Player2);   // ドラッグ、スペルカードの攻撃表示の設定
                SetAllCardsInFieldCanAttack(Player.Player2); // モンスターカードの攻撃表示の設定

                if (BattleViewController.Instance.battleData.isPlayer2AI) {
                    // マウスカーソルの無効化
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    BattleViewController.Instance.TurnendButtonActivate(false); // ターンエンドボタンを押せなくする

                    StartCoroutine(enemyAI.EnemyTurn());
                } else {
                    BattleViewController.Instance.TurnendButtonActivate(true); // ターンエンドボタンを押せるようにする
                    
                    PlayerTurn(); // ターン開始
                }
            }
        }

        public IEnumerator CountDown()
        {
            // 情報パネルが過ぎるまで待つ
            yield return new WaitForSeconds(4.0f);

            timeCount = maxSeconds;
            untilEndOfTurnText.text = timeCount.ToString();
            while (timeCount > 0) {
                yield return new WaitForSeconds(1); // 1秒待機
                timeCount--;
                untilEndOfTurnText.text = timeCount.ToString();
            }
            ChangeTurn();
        }

        public void PlayerTurn()
        {
            // マウスカーソルの有効化
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void OpenPlayerHandsCard(Player player) {
            CardController[] playerCardList = {};
            if (player == Player.Player1) {
                Transform player1HandTransform = BattleViewController.Instance.GetPlayer1HandTransform();
                playerCardList = player1HandTransform.GetComponentsInChildren<CardController>();
            } else {
                Transform player2HandTransform = BattleViewController.Instance.GetPlayer2HandTransform();
                playerCardList = player2HandTransform.GetComponentsInChildren<CardController>();
            }

            foreach(CardController card in playerCardList) {
                card.view.SetActiveFrontPanel(true);
            }
        }

        public void ClosePlayerHandsCard(Player player) {
            CardController[] handCardList = {};
            if (player == Player.Player1) {
                Transform player1HandTransform =  BattleViewController.Instance.GetPlayer1HandTransform();
                handCardList = player1HandTransform.GetComponentsInChildren<CardController>();
            } else {
                Transform player2HandTransform = BattleViewController.Instance.GetPlayer2HandTransform();
                handCardList = player2HandTransform.GetComponentsInChildren<CardController>();
            }
            
            foreach(CardController card in handCardList) {
                card.view.SetActiveFrontPanel(false);
            }
        }

        public void ChangeTurn()
        {
            // ターンが変わったらタイマーの文字を元に戻す
            untilEndOfTurnText.text = maxSeconds.ToString();

            BattleViewController.Instance.isPlayer1Turn = !(BattleViewController.Instance.isPlayer1Turn);

            if (BattleViewController.Instance.isPlayer1Turn) {
                // マナコストを+1してからターン開始
                BattleViewController.Instance.player1Hero.AddManaCost(1);
                
                // 手札が保有量を超えなければカードを手札に加える
                BattleViewController.Instance.GiveCardToHand(BattleViewController.Instance.player1Hero, BattleViewController.Instance.GetPlayer1HandTransform(), Player.Player1);
                BattleViewController.Instance.player2Hero.view.SetActiveActivatedPanel(false);
            
                ClosePlayerHandsCard(Player.Player1); // ターンが終わったので手札を隠す
            } else {
                // マナコストを+1してからターン開始
                BattleViewController.Instance.player2Hero.AddManaCost(1);

                // 手札が保有量を超えなければカードを手札に加える
                BattleViewController.Instance.GiveCardToHand(BattleViewController.Instance.player2Hero, BattleViewController.Instance.GetPlayer2HandTransform(), Player.Player2);
                BattleViewController.Instance.player1Hero.view.SetActiveActivatedPanel(false);

                ClosePlayerHandsCard(Player.Player2); // ターンが終わったので手札を隠す
            }
            
            TurnStart();
        }

        // カードのドラッグ&ドロップイベントをON/OFFする
        public void SetAllCardsDragAndDropEventEnable()
        {
            CardController[] player1FieldCardList = BattleViewController.Instance.GetFriendFieldCards(Player.Player1);
            CardController[] player1HandCardList = BattleViewController.Instance.GetMyHandCards(Player.Player1);
            CardController[] player2FieldCardList = BattleViewController.Instance.GetFriendFieldCards(Player.Player2);
            CardController[] player2HandCardList = BattleViewController.Instance.GetMyHandCards(Player.Player2);

 
            if (BattleViewController.Instance.isPlayer1Turn) {
                SetCardsDragAndDropEventEnable(player1FieldCardList, true);
                SetCardsDragAndDropEventEnable(player1HandCardList, true);
                SetCardsDragAndDropEventEnable(player2FieldCardList, false);
                SetCardsDragAndDropEventEnable(player2HandCardList, false);
            } else {
                SetCardsDragAndDropEventEnable(player1FieldCardList, false);
                SetCardsDragAndDropEventEnable(player1HandCardList, false);
                SetCardsDragAndDropEventEnable(player2FieldCardList, true);
                SetCardsDragAndDropEventEnable(player2HandCardList,true);
            }
        }

        public void SetCardsDragAndDropEventEnable(CardController[] cardList, bool isEnable)
            {
                foreach (CardController card in cardList) {
                    card.gameObject.GetComponent<CardMovement>().enabled = isEnable;
                }
        }


        public void SetAllCardsInFieldCanAttack(Player player)
        {
            CardController[] fieldCardList = BattleViewController.Instance.GetFriendFieldCards(player);

            // 攻撃表示の変更
            if (player == Player.Player1) {
                SetCardsInFieldCanAttack(fieldCardList, true, BattleViewController.Instance.player1Hero);
                SetCardsInFieldCanAttack(fieldCardList, false, BattleViewController.Instance.player2Hero);
            } else {
                SetCardsInFieldCanAttack(fieldCardList, false, BattleViewController.Instance.player1Hero);
                SetCardsInFieldCanAttack(fieldCardList, true, BattleViewController.Instance.player2Hero);
            }
        }

        public void SetCardsInFieldCanAttack(CardController[] cardList, bool canAttack, HeroController hero)
        {
            foreach (CardController card in cardList) {
                if (canAttack) {
                    // フィールドに出ているカード（モンスターカード）はターン開始時に決定・手持ちで攻撃表示にできるのはスペルカードのみ
                    if (!card.model.IsSpell() && card.IsFieldCard()) {
                        card.SetAttackable(canAttack);
                    }
                }
            }
        }


        public void AddTurnNumber(HeroController hero)
        {
            hero.AddTurnNumber();
        }
    }
}