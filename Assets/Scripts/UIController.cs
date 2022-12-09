using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
     // ターンエンドボタン
    [SerializeField] GameObject turnendButton;

    // リスタートボタン
    [SerializeField] GameObject restartButton;

    // リセレクトボタン
    [SerializeField] GameObject reselectButton;

    // ゲームスタートボタン
    [SerializeField] GameObject gameStartButton;

    // Hero選択画面（スタート画面）
    [SerializeField] GameObject selectHeroView;

    // メインのゲーム画面
    [SerializeField] GameObject mainView;

    // 結果表示パネル
    [SerializeField] GameObject resultView;
    [SerializeField] TextMeshProUGUI resultText;

    // 警告画面
    [SerializeField] GameObject alertDialog; 
    [SerializeField] TextMeshProUGUI alertText;
    [SerializeField] GameObject alertOKButton; 
    [System.NonSerialized] public bool isPushedAlertOKButton = false;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void RestartButtonActivate(bool isActive)
    {
        restartButton.SetActive(isActive);
        restartButton.GetComponent<Button>().interactable = isActive;
    }
    
    public void TurnendButtonActivate(bool isActive)
    {
        turnendButton.SetActive(isActive);
        turnendButton.GetComponent<Button>().interactable = isActive;
    }

    public void ReselectButtonActivate(bool isActive)
    {
        reselectButton.SetActive(isActive);
        reselectButton.GetComponent<Button>().interactable = isActive;
    }

    public void GameStartButtonActivate(bool isActive)
    {
        gameStartButton.SetActive(isActive);
        gameStartButton.GetComponent<Button>().interactable = isActive;
    }

    // Alert DialogのOKボタンが押された時（現在はヒーローセレクト画面のみ対応）
    public void PushedAlertOKButton()
    {
        
        if(selectHeroView.activeSelf == true && mainView.activeSelf == false && resultView.activeSelf == false) {
            
            HideAlertDialog();
        }
    }

    public void HideMainView()
    {
        // 非表示でもFocusがあるため
        Vector3 pos = mainView.transform.localPosition;
        pos.z = -1;

        mainView.SetActive(false);
    }

    public void ShowMainView()
    {
        // バグでZ軸が0以下になるため
        Vector3 pos = mainView.transform.localPosition;
        pos.z = 1;

        mainView.SetActive(true);
    }

    public void HideSelectHeroView(){

        // 非表示でもFocusがあるため
        Vector3 pos = selectHeroView.transform.localPosition;
        pos.z = -1;

        gameManager.soundController.StopBGM(); 
        gameManager.uiController.GameStartButtonActivate(false);

        selectHeroView.SetActive(false);
    }

    public void ShowSelectHeroView(){

        gameManager.soundController.PlayBGM(BGMSoundData.BGM.SELECTHERO);
        gameManager.uiController.GameStartButtonActivate(true);

        // バグでZ軸が0以下になるため
        Vector3 pos = selectHeroView.transform.localPosition;
        pos.z = 1;

        selectHeroView.SetActive(true);
    }

    public void HideResultView()
    {
        // 非表示でもFocusがあるため
        Vector3 pos = resultView.transform.localPosition;
        pos.z = -1;

        resultView.SetActive(false);
    }

    public void ShowResultView()
    {
        // バグでZ軸が0以下になるため
        Vector3 pos = resultView.transform.localPosition;
        pos.z = 1;
        
        if (GameManager.instance.player1Hero.model.GetHP() <= 0 || GameManager.instance.player2Hero.model.GetHP() <= 0)
        {
            resultView.SetActive(true);
            
            if (GameManager.instance.player1Hero.model.GetHP() <= 0)
            {
                resultText.text = "LOSE";
            }
            else
            {
                resultText.text = "WIN";
            }
            // リスタート・リセレクトボタンを表示し、ターンエンドボタンを非表示にする
            RestartButtonActivate(true);
            ReselectButtonActivate(true);
            TurnendButtonActivate(false);
        }
    }

    public void HideAlertDialog()
    {
        // 非表示でもFocusがあるため
        Vector3 pos = alertDialog.transform.localPosition;
        pos.z = -1;

        alertDialog.SetActive(false);
        alertOKButton.GetComponent<Button>().interactable = false;
    }

    public void ShowAlertDialog(string alertString)
    {
        // バグでZ軸が0以下になるため
        Vector3 pos = alertDialog.transform.localPosition;
        pos.z = 1;
        alertText.text = alertString;

        alertDialog.SetActive(true);
        alertOKButton.GetComponent<Button>().interactable = true;
    }
}
