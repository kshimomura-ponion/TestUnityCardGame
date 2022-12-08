
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TurnInfoAnimation : MonoBehaviour
{
    [SerializeField] GameObject turnInfo;
    [SerializeField] TextMeshProUGUI turnInfoText;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
    }

    // この方法だと、必ず2回目は右から来てしまうため
    public void OnEnable()
    {
        turnInfo.SetActive(false);
        if(gameManager.turnController.isPlayer1Turn == true) 
        {
            turnInfoText.text = "Your Turn";
            turnInfo.transform.DOLocalMove(new Vector3(-2000f,0f,0f), 0f);
        } else {
            turnInfoText.text = "Enemy Turn";
            turnInfo.transform.DOLocalMove(new Vector3(2000f,0f,0f), 0f);
        }
        turnInfo.transform.DORestart();
        turnInfo.SetActive(true);
        turnInfo.transform.DOLocalMove(new Vector3(0f,0f,0f), 1.5f).OnComplete(TurnInfoXAxisTransForm);
    }

    public void OnDisable()
    {
        turnInfo.transform.DORewind();
    }

    void TurnInfoXAxisTransForm(){
        float turnInfoPanelXDestination = 0.0f;
        if(gameManager.turnController.isPlayer1Turn == true) 
        {
            turnInfoPanelXDestination = 2000.0f;
        } else {
            turnInfoPanelXDestination = -2000.0f;
        }
        turnInfo.transform.DOLocalMove(new Vector3(turnInfoPanelXDestination,0f,0f), 1.5f);
    }
}
