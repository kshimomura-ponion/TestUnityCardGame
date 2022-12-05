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

    [SerializeField] TurnInfoAnimation turnInfoAnimation;

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

    void TurnCalc()
    {
        StartCoroutine(turnInfoAnimation.ShowPanel(isPlayerTurn));

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
        // 手札のカードリストを取得
        CardController[] cardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        // 場に出すカードを選択
        CardController card = cardList[0];
        // カードを移動
        card.movement.SetCardTransform(enemyFieldTransform);
        ChangeTurn();
    }
}
