using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TurnController: MonoBehaviour
{
    // ターン情報パネル
    [SerializeField] GameObject turnInfoPanel;

    // プレイヤー1のターンかどうか識別する
    [System.NonSerialized] public bool isPlayer1Turn;

    // ターン数（ターン情報パネル内）
    [SerializeField] TextMeshProUGUI turnNumInfoText;

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

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void TurnStart()
    {
        EnableTurnInfoPanel();
        Invoke("DisableTurnInfoPanel", 2.5f);

        // カウントダウン開始
        StopAllCoroutines();
        StartCoroutine(CountDown());

        isTurnEnd = false;

        if(isPlayer1Turn){
            // ターン数を表示する
            ShowTurnNumber(gameManager.player1Hero);

            // 情報パネルが過ぎるまで待つ
            Invoke("PlayerTurn", 3.5f);

            // ターンエンドボタンを押せるようにする
            gameManager.uiManager.turnendButtonActivate(true);
        } else {
            // ターン数を表示する
            ShowTurnNumber(gameManager.player2Hero);

            StartCoroutine(enemyAI.EnemyTurn());

            // ターンエンドボタンを押せなくする
            gameManager.uiManager.turnendButtonActivate(false);
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
            gameManager.player1Hero.view.SetActiveSelectablePanel(true);
            playerFieldCardList = gameManager.GetFriendFieldCards(PLAYER.PLAYER1);
        } else {
            gameManager.player2Hero.view.SetActiveSelectablePanel(true);
            playerFieldCardList = gameManager.GetFriendFieldCards(PLAYER.PLAYER2);
        }

        // 攻撃表示に変更
        SettingCanAttackView(playerFieldCardList,true);
        Debug.Log("Playerのターン");

        if(isPlayer1Turn){
            gameManager.player1Hero.AddTurnNumber();  // Player1のターン数を増やす
            OpenPlayerHandsCard(PLAYER.PLAYER1);      // Player2の手札を全てOpenにする
        } else {
            gameManager.player2Hero.AddTurnNumber();  // Player2のターン数を増やす
            OpenPlayerHandsCard(PLAYER.PLAYER2);      // Player2の手札を全てOpenにする
        }
    }

    public void OpenPlayerHandsCard(PLAYER player){
        CardController[] playerCardList = {};
        if(player == PLAYER.PLAYER1){
            playerCardList = gameManager.player1HandTransform.GetComponentsInChildren<CardController>();
        } else {
            playerCardList = gameManager.player2HandTransform.GetComponentsInChildren<CardController>();
        }

        foreach(CardController card in playerCardList)
        {
            card.view.SetActiveFrontPanel(true);
        }
    }

    public void ClosePlayerHandsCard(PLAYER player){
        CardController[] playerCardList = {};
        if(player == PLAYER.PLAYER1){
            playerCardList = gameManager.player1HandTransform.GetComponentsInChildren<CardController>();
        } else {
            playerCardList = gameManager.player2HandTransform.GetComponentsInChildren<CardController>();
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

        CardController[] player1FieldCardList = gameManager.player1FieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(player1FieldCardList, false);
        CardController[] player2FieldCardList = gameManager.player2FieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(player2FieldCardList, false);

        isPlayer1Turn = !isPlayer1Turn;

        if (isPlayer1Turn) {
            // マナコストを+1してからターン開始
            gameManager.player1Hero.AddManaCost(1);
            // カードを手札に加える
            gameManager.GiveCardToHand(gameManager.player1Hero, gameManager.player1HandTransform, PLAYER.PLAYER1);
            gameManager.player2Hero.view.SetActiveSelectablePanel(false);
        
            ClosePlayerHandsCard(PLAYER.PLAYER1); // ターンが終わったので手札を隠す
        } else {
            // マナコストを+1してからターン開始
            gameManager.player2Hero.AddManaCost(1);
            // カードを手札に加える
            gameManager.GiveCardToHand(gameManager.player2Hero, gameManager.player2HandTransform, PLAYER.PLAYER2);
            gameManager.player1Hero.view.SetActiveSelectablePanel(false);

            ClosePlayerHandsCard(PLAYER.PLAYER2); // ターンが終わったので手札を隠す
        }
        
        TurnStart();
    }

    public void HealToHero(CardController healer)
    {
        if (healer.GetOwner() == PLAYER.PLAYER1){
            gameManager.player1Hero.Healed(healer);
        } else {
            gameManager.player2Hero.Healed(healer);
        }
    }

    void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
    {
        foreach (CardController card in fieldCardList)
        {
            card.SetCanAttack(canAttack);
        }
    }

    void ShowTurnNumber(HeroController hero){
        turnNumInfoText.text = "Turn " + hero.GetTurnNumber().ToString();
        turnNumText.text = hero.GetTurnNumber().ToString();
    }

    public void AddTurnNumber(HeroController hero)
    {
        hero.AddTurnNumber();
    }

    void EnableTurnInfoPanel(){
        turnInfoPanel.SetActive(true);
    }

    void DisableTurnInfoPanel(){
        turnInfoPanel.SetActive(false);
    }
}
