using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TurnInfoAnimation : MonoBehaviour
{
    [SerializeField] Transform turnInfoTransform;
    [SerializeField] TextMeshProUGUI turnInfoText;
    [System.NonSerialized] public GameObject gameManager;
    float xDestination = 0f;

    [SerializeField] GameObject turnInfoPanel;

    // Start is called before the first frame update
    void Start()
    { 
        gameManager = GameObject.Find("GameManager");
        turnInfoTransform.DOLocalMove(new Vector3(-1800.0f,0f,0f), 0f);
    }

    public IEnumerator ShowPanel(bool isPlayerTurn)
    {

        EnablePanel();
        if(isPlayerTurn == true) {
            turnInfoText.text = "Your Turn";
                    UnityEngine.Debug.Log(turnInfoText.text);
        } else {
            turnInfoText.text = "Enemy Turn";
        }
        xDestination = 0.0f;
        XAxisTransForm();
        xDestination = 2000.0f;
        Invoke("XAxisTransForm", 2.0f);
        Invoke("DisablePanel", 2.0f);
        SetFirstPosition();
        CancelInvoke();
        yield return new WaitForSeconds(5.0f);
    }

    public IEnumerator HidePanel()
    {
        CancelInvoke();
        DisablePanel();
        SetFirstPosition();
        yield return new WaitForSeconds(1.0f);
    }

    void SetFirstPosition()
    {
        // 元の場所に戻す
        turnInfoTransform.DOLocalMove(new Vector3(-1800.0f,0f,0f), 0f);
    }

    void XAxisTransForm(){
        turnInfoTransform.DOLocalMove(new Vector3(xDestination,0f,0f), 1.5f);
    }

    void EnablePanel(){
        turnInfoPanel.SetActive(true);
    }

    void DisablePanel(){
        turnInfoPanel.SetActive(false);
    }
}
