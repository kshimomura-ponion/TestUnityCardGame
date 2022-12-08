using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
     // ターンエンドボタン
    [SerializeField] GameObject turnendButton;

    // リスタートボタン
    [SerializeField] GameObject restartButton;

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

    public void restartButtonActivate(bool isActive)
    {
        restartButton.SetActive(isActive);
        restartButton.GetComponent<Button>().interactable = isActive;
    }
    
    public void turnendButtonActivate(bool isActive)
    {
        turnendButton.SetActive(isActive);
        turnendButton.GetComponent<Button>().interactable = isActive;
    }

    public void HideMainView()
    {
        mainView.SetActive(false);
    }

    public void ShowMainView()
    {
        mainView.SetActive(true);
    }

    public void HideSelectHeroView(){
        selectHeroView.SetActive(false);
    }

    public void ShowSelectHeroView(){
        selectHeroView.SetActive(true);
    }

    public void HideResultView()
    {
        resultView.SetActive(false);
    }

    public void ShowResultView()
    {
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
            // リスタートボタンを表示し、ターンエンドボタンを非表示にする
            restartButtonActivate(true);
            turnendButtonActivate(false);
        }
    }

    public void HideAlertDialog()
    {
        alertDialog.SetActive(false);
    }

    public void ShowAlertDialog(string alertString)
    {
        alertText.text = alertString;

        alertDialog.SetActive(true);
    }
}
