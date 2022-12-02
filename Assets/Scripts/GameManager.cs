using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    // 手札にカードを生成
    [SerializeField] CardController cardPrefab;

    // 手札を置ける場所情報
    [SerializeField] Transform playerHandTransform, enemyHandTransform;

    // プレイヤーのターンかどうか識別する
    bool isPlayerTurn;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        SettingInitHand(5);
        isPlayerTurn = true;
        TurnCalc();
    }

    void SettingInitHand(int initHandNum)
    {
        for (int i = 0; i < initHandNum; i++)
        {
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

    void TurnCalc()
    {
        if(isPlayerTurn)
        {
            PlayerTurn();
        } else {
            EnemyTurn();
        }
    }
    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        TurnCalc();
    }

    void PlayerTurn()
    {
        Debug.Log("Playerのターン");
    }
    void EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        ChangeTurn();
    }
}
