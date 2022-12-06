using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager: MonoBehaviour
{
    // 手札にカードを生成
    [SerializeField] CardController cardPrefab;

    // 手札を置ける場所情報
    [SerializeField] Transform playerHandTransform, enemyHandTransform;
    [SerializeField] Transform playerFieldTransform, enemyFieldTransform;

    // ターン情報パネル
    [SerializeField] GameObject turnInfoPanel;
    [SerializeField] Transform turnInfoTransform;
    [SerializeField] TextMeshProUGUI turnInfoText;

    float turnInfoPanelXDestination = 0f;

    // プレイヤーのターンかどうか識別する
    [System.NonSerialized] public bool isPlayerTurn;

	// シングルトン化（どこからでもアクセス可能にする）
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        SettingInitHand(3);
        isPlayerTurn = true;

        TurnCalc();
    }


    void SettingInitHand(int initHandNum)
    {
        for (int i = 0; i < initHandNum; i++)
        {
            // 手札置き場（Hand）、FieldともにHorizontalLayoutGroupに属していないとカードが重なってしまう
            // （カードはAlignmentでCenterに置くべき）
            CreateCard(playerHandTransform);
            CreateCard(enemyHandTransform);
        }
    }

    // カードをインスタンス化
    void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(1);
    }

    public void TurnCalc()
    {
        if(isPlayerTurn == true) {
            turnInfoText.text = "Your Turn";
        } else {
            turnInfoText.text = "Enemy Turn";
        }
        EnablePanel();
        turnInfoPanelXDestination = 0.0f;
        TurnInfoXAxisTransForm();
        turnInfoPanelXDestination = 2000.0f;
        Invoke("TurnInfoXAxisTransForm", 2.0f);

        Invoke("DisablePanel", 3.0f);
        Invoke("SetFirstPosition", 4.0f);

        if(isPlayerTurn){
            PlayerTurn();
        } else {
            EnemyTurn();
        }

    }

    void SetFirstPosition()
    {
        // 元の場所に戻す
        turnInfoTransform.DOLocalMove(new Vector3(-1800.0f,0f,0f), 0f);
    }

    void TurnInfoXAxisTransForm(){
        turnInfoTransform.DOLocalMove(new Vector3(turnInfoPanelXDestination,0f,0f), 1.5f);
    }

    void EnablePanel(){
        turnInfoPanel.SetActive(true);
    }

    void DisablePanel(){
        turnInfoPanel.SetActive(false);
    }
    public void PlayerTurn()
    {
        Debug.Log("Playerのターン");

    }
    
    public void EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        // 手札のカードリストを取得
        CardController[] cardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        // 場に出すカードを選択
        CardController card = cardList[0];
        // カードを移動
        card.movement.SetCardTransform(enemyFieldTransform);

        // 攻撃① フィールドのカードリストを取得
        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃② pick player's defender cards.
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();

        if(fieldCardList.Length > 0 && playerFieldCardList.Length > 0){
            // 攻撃③ pick enemy's attacker cards.
            CardController attacker = fieldCardList[0];
            
            CardController defender = playerFieldCardList[0];
            // 攻撃④ start combat
            CardsBattle(attacker, defender);
        }
        Invoke("ChangeTurn", 5.0f);
    }

    public void CardsBattle(CardController attacker, CardController defender)
    {
        // ダメージを計算し、Viewのダメージ情報パネルを更新する
        defender.view.damageInfoText.text = "-" + defender.Attack(attacker).ToString();
        attacker.CheckAlive();

        attacker.view.damageInfoText.text = "-" + attacker.Attack(defender).ToString();
        defender.CheckAlive();
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        TurnCalc();
    }

}
