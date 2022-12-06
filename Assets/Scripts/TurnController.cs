using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TurnController : MonoBehaviour
{
    [SerializeField] Transform turnInfoTransform;
    [SerializeField] TextMeshProUGUI turnInfoText;
    //[System.NonSerialized] public GameManager gameManager;

    // 手札を置ける場所情報
    [SerializeField] Transform playerHandTransform, enemyHandTransform;
    [SerializeField] Transform playerFieldTransform, enemyFieldTransform;

    float xDestination = 0f;

    // ターン情報パネル
    [SerializeField] GameObject turnInfoPanel;

    // プレイヤー名（使用するかは不明）
    [System.NonSerialized] public string playerName = "";

    // プレイヤー情報
    /*[SerializeField] Image playerIcon;
    [SerializeField] TextMeshProUGUI playerHP;

    // 敵情報
    [SerializeField] Image enemyIcon;
    [SerializeField] TextMeshProUGUI enemyHP;*/

    [System.NonSerialized] public bool isRunning = false;

    // シングルトン化（どこからでもアクセス可能にする）
    public static TurnController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        turnInfoTransform.DOLocalMove(new Vector3(-1800.0f,0f,0f), 0f);
    }
    // 元のコードのようにGameManagerに返そうとしたが、コルーチンの終了がどうしてもメインより遅くなるため断念
    // ターン情報パネルのアニメーションだけでなく各ターン全般のBehavierという扱いに変更した
    public IEnumerator StartTurn(bool isPlayerTurn, UnityEngine.Events.UnityAction<IEnumerator> callback)
    { 
        isRunning = true;
        if(isPlayerTurn == true) {
            turnInfoText.text = "Your Turn";
        } else {
            turnInfoText.text = "Enemy Turn";
        }
        var showPanel = ShowPanel();
        var coroutine1 = StartCoroutine(showPanel);
        yield return coroutine1;
        var hidePanel = HidePanel();
        var coroutine2 = StartCoroutine(hidePanel);
        yield return coroutine2;

        bool isRunning1 = (bool)showPanel.Current;
        bool isRunning2 = (bool)hidePanel.Current;

        if(isPlayerTurn){
            PlayerTurn();
        } else {
            EnemyTurn();
        }
        isRunning = false;
        callback(null);
        yield break;
    }

    public IEnumerator ShowPanel()
    {
        bool isRunning = true;
        
        EnablePanel();
        xDestination = 0.0f;
        TurnInfoXAxisTransForm();
        xDestination = 2000.0f;
        Invoke("TurnInfoXAxisTransForm", 2.0f);
        yield return new WaitForSeconds(4.0f);

        isRunning = false;
        yield return isRunning;
    }

    public IEnumerator HidePanel()
    {
        bool isRunning = true;

        CancelInvoke();
        DisablePanel();
        SetFirstPosition();
        yield return new WaitForSeconds(0.01f);

        isRunning = false;
        yield return isRunning;
    }

    void SetFirstPosition()
    {
        // 元の場所に戻す
        turnInfoTransform.DOLocalMove(new Vector3(-1800.0f,0f,0f), 0f);
    }

    void TurnInfoXAxisTransForm(){
        turnInfoTransform.DOLocalMove(new Vector3(xDestination,0f,0f), 1.5f);
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
        defender.view.damageInfoText.text = "-" + defender.model.Attack(attacker).ToString();
        attacker.CheckAlive();

        attacker.view.damageInfoText.text = "-" + attacker.model.Attack(defender).ToString();
        defender.CheckAlive();
    }

    public void ChangeTurn()
    {
        GameManager.instance.isPlayerTurn = !(GameManager.instance.isPlayerTurn);
        GameManager.instance.TurnCalc();
    }
}
