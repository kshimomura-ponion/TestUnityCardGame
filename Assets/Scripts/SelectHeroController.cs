using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SelectHeroController : MonoBehaviour
{
    // Hero Prefab
    [SerializeField] HeroController heroPrefab;

    // 現在用意できているヒーロー、カードの種類の数
    private static int existHeroNum = 5;
    private static int existCardNum = 8;

    // Hero
    [System.NonSerialized] public int hero1ID = 0;
    [System.NonSerialized] public int hero2ID = 0;

    // セレクト終了までの時間管理
    [SerializeField] TextMeshProUGUI untilEndOfSelectText;
    private int maxSeconds = 10;
    private int timeCount;


    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

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
        gameManager.isPlayer2AI = true;

        yield return new WaitForSeconds(0.05f);
        OnSelectedHeroes();
        yield return new WaitForSeconds(0.05f);
    }

    public void OnSelectedHeroes(){
        // プレイヤーセレクト画面を閉じる
        gameManager.uiManager.HideSelectHeroView();

        if(hero1ID > 0) {
            // プレイヤー2が決まっていない時はプレイヤー1のHeroを勝手に決めてプレイヤー2をAIにする
            if(hero2ID == 0){
                hero2ID = UnityEngine.Random.Range(1, existHeroNum);
                gameManager.isPlayer2AI = true;

                // ゲーム開始
                SettingHeroes(hero1ID, hero2ID);
                gameManager.StartGame();
            } else {
                // 警告ダイアログを出す
                gameManager.uiManager.ShowAlertDialog("プレイヤー1を選択してください");
            }

        }
    }
    void SettingHeroes(int id1, int id2)
    {
        hero1ID = id1;
        hero2ID = id2;
        gameManager.player1Hero = Instantiate(heroPrefab, gameManager.player1HeroPanel, false);
        gameManager.player2Hero = Instantiate(heroPrefab, gameManager.player2HeroPanel, false);

        // デッキ 1~8のカードIDから16枚をランダムに生成する
        List<int> player1Deck = new List<int>();
        List<int> player2Deck = new List<int>();
        for(int i = 0; i < 16; i++)
        {
            var idx = UnityEngine.Random.Range(1, existCardNum);
            player1Deck.Add(idx);
            idx = UnityEngine.Random.Range(1, existCardNum);
            player2Deck.Add(idx);
        }
        // UnityEngine.Debug.Log(string.Join(",", player1Deck.Select(n => n.ToString())));
        // UnityEngine.Debug.Log(string.Join(",", player2Deck.Select(n => n.ToString())));

        // ここで各Heroの持つカード情報を整理しておく
        gameManager.player1Hero.Init(hero1ID, player1Deck, PLAYER.PLAYER1);
        gameManager.player2Hero.Init(hero2ID, player2Deck, PLAYER.PLAYER2);
    }

}
