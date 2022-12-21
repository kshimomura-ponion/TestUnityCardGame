/*using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using System.Text;
using System.Linq;

public class GameManager: MonoBehaviour
{

    // Card Prefab
    [SerializeField] CardController cardPrefab;

    // 手札を置ける場所情報
    public Transform player1HandTransform, player2HandTransform;
    public Transform player1FieldTransform, player2FieldTransform;

    // Heroを置ける場所情報
    public Transform player1HeroPanel, player2HeroPanel;

    // ターンコントローラー
    public TurnController turnController;

    // UIコントローラー
    public UIController uiController;

    // Hero選択画面（スタート画面）
    //public SelectHeroController selectHeroController;

    // プレイヤー2はAIか否か
    [System.NonSerialized] public bool isPlayer2AI;

    // Heroの実体
    [System.NonSerialized] public HeroController player1Hero, player2Hero;

    public static GameManager instance;

    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
    }

    void Start()
    {
        // ヒーローセレクト画面以外を非表示にする
        uiController.HideMainView();
        uiController.HideResultView();
        
        // ヒーロー選択
       // uiController.ShowSelectHeroView();
       // StartCoroutine(selectHeroController.SelectHeroAndGameStart());
    }

    public void StartGame()
    {
        // 音楽を再生
        SoundManager.instance.PlayBGM(BGM.BATTLE);
        uiController.ShowMainView();

        // 手札の準備
        SettingInitHand(3);
        
        // リスタートボタンを非表示にし、ターンエンドボタンを表示にする
        uiController.RestartButtonActivate(false);
        uiController.TurnendButtonActivate(true);

        turnController.isPlayer1Turn = true;
        turnController.TurnStart();
    }

    public void SettingInitHand(int initHandNum)
    {
        for (int i = 0; i < initHandNum; i++)
        {
            // 手札置き場（Hand）、FieldともにHorizontalLayoutGroupに属していないとカードが重なってしまう
            // （カードはAlignmentでCenterに置くべき）
            GiveCardToHand(player1Hero, player1HandTransform, PLAYER.PLAYER1);
            GiveCardToHand(player2Hero, player2HandTransform, PLAYER.PLAYER2);
        }
    }

    // デッキからカードを取得
    public void GiveCardToHand(HeroController hero, Transform hand, PLAYER player)
    {
        List<(int, CARDTYPE)> deck = hero.model.GetCardDeck();
        if (deck.Count == 0){
            return;
        }
        // デッキの上から値を削除していく
        (int, CARDTYPE) cardInfo = deck[0];
        hero.model.RemoveCard(0);

        CreateCard(cardInfo, hand, player);
    }

    // カードをインスタンス化
    void CreateCard((int, CARDTYPE) cardInfo, Transform hand, PLAYER player)
    {
        CardController card = Instantiate(cardPrefab, hand, false);

        // UnityEngine.Debug.Log(cardInfo.Item1.ToString());
        // UnityEngine.Debug.Log(cardInfo.Item2.ToString());
        // IDとカードタイプを渡してカードを生成する
        card.Init(cardInfo.Item1, cardInfo.Item2, player);
    }

    // リスタート
    public void Restart()
    {
        SoundManager.instance.PlaySE(SE.OK);
        
        uiController.HideResultView();  // 結果表示画面を非表示にする
        CleanUp();  // いらないオブジェクトの破棄

        StartGame();
    }

    // リセレクト
    public void Reselect()
    {
        SoundManager.instance.PlaySE(SE.OK);
    
        uiController.HideResultView();  // 結果表示画面を非表示にする
        CleanUp();  // いらないオブジェクトの破棄

        // ヒーロー選択
        //uiController.ShowSelectHeroView();
        //StartCoroutine(selectHeroController.SelectHeroAndGameStart());
    }

    // いらないオブジェクトの破棄
    public void CleanUp()
    {
        // handとFiledのカードを削除
        foreach (Transform card in player1HandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in player1FieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in player2HandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in player2FieldTransform)
        {
            Destroy(card.gameObject);
        }
    }

    public void CheckHeroHP()
    {
        if (GameManager.instance.player1Hero.model.GetHP() <= 0 || GameManager.instance.player2Hero.model.GetHP() <= 0){
            GameOver(player1Hero.model.GetHP());
        }
    }

    void GameOver(int hp)
    {
        //音楽を停止
        SoundManager.instance.StopBGM();

        StopAllCoroutines();
        uiController.ShowResultView();
    }

    public CardController[] GetMyHandCards(PLAYER player)
    {
        if(player == PLAYER.PLAYER1){
            return player1HandTransform.GetComponentsInChildren<CardController>();
        } else {
            return player2HandTransform.GetComponentsInChildren<CardController>();
        }
    }

    // 現在ターンが回っているHeroにとって味方のフィールドを取得
    public CardController[] GetFriendFieldCards(PLAYER player)
    {
        if(player == PLAYER.PLAYER1){
            return player1FieldTransform.GetComponentsInChildren<CardController>();
        } else {
            return player2FieldTransform.GetComponentsInChildren<CardController>();
        }
    }

    // 現在ターンが回っているHeroにとって敵のフィールドを取得
    public CardController[] GetOpponentFieldCards(PLAYER player)
    {
        if(player == PLAYER.PLAYER1){
            return player2FieldTransform.GetComponentsInChildren<CardController>();
        } else {
            return player1FieldTransform.GetComponentsInChildren<CardController>();
        }
    }
}
*/