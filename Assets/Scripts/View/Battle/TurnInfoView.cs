
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace TestUnityCardGame
{
    public class TurnInfoView : MonoBehaviour
    {
        [SerializeField] GameObject turnInfoView;
        [SerializeField] GameObject turnInfo;
        [SerializeField] TextMeshProUGUI turnInfoText;
        [SerializeField] TextMeshProUGUI turnNumInfoText;

        private void Awake()
        {
            turnInfoView.SetActive(false);
        }

        public void ShowTurnInfoView(int turnNum)
        {
            turnNumInfoText.text = "Turn " + turnNum.ToString();
            turnInfoView.SetActive(true);
        }

        public void HideTurnInfoView()
        {
            turnInfoView.SetActive(false);
        }

        // この方法だと、必ず2回目は右から来てしまうため
        public void OnEnable()
        {
            turnInfo.SetActive(false);
            if(TurnController.Instance.isPlayer1Turn == true) 
            {
                turnInfoText.text = "Player 1's Turn";
                turnInfo.transform.DOLocalMove(new Vector3(-2000f,0f,0f), 0f);
            } else {
                turnInfoText.text = "Player 2's Turn";
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
            if(TurnController.Instance.isPlayer1Turn == true) 
            {
                turnInfoPanelXDestination = 2000.0f;
            } else {
                turnInfoPanelXDestination = -2000.0f;
            }
            turnInfo.transform.DOLocalMove(new Vector3(turnInfoPanelXDestination,0f,0f), 1.5f).OnComplete(HideTurnInfoView);
        }
    }
}
