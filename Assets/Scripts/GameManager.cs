using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    // 手札にカードを生成
    [SerializeField] CardController cardPrefab;

    // 手札を置ける場所情報
    [SerializeField] Transform playerHandTransform, enemyHandTransform;
    [SerializeField] Transform playerFieldTransform, enemyFieldTransform;

    // プレイヤーのターンかどうか識別する
    [System.NonSerialized] public bool isPlayerTurn;

    [SerializeField] TurnController turnController;

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
        IEnumerator enumerator = turnController.StartTurn(isPlayerTurn, (result)=> enumerator = result);
        StartCoroutine(enumerator);
    }
}
