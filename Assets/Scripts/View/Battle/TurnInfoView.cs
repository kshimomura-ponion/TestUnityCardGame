
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using TestUnityCardGame.Presenter.Battle;

namespace TestUnityCardGame.View.Battle
{
    public class TurnInfoView : MonoBehaviour
    {
        public GameObject turnInfoView;
        public GameObject turnInfo;
        [SerializeField] TextMeshProUGUI turnInfoText;
        [SerializeField] TextMeshProUGUI turnNumInfoText;

        private void Awake()
        {
            turnInfoView.SetActive(false);
            HideTurnInfo();
        }

        public void Show(int turnNum)
        {
            turnInfoView.SetActive(true);
            turnNumInfoText.text = "Turn " + turnNum.ToString();
        }

        public void Hide()
        {
            turnInfoView.SetActive(false);
        }

        public void ShowTurnInfo()
        {
            turnInfo.SetActive(true);
        }

        public void HideTurnInfo()
        {
            turnInfo.SetActive(false);
        }

        public void SetTurnInfoText(string infoString)
        {
            turnInfoText.text = infoString;
        }

        
        void TurnInfoXAxisTransForm() {
            float turnInfoPanelXDestination = 0.0f;
            if (TurnController.Instance.isPlayer1Turn) 
            {
                turnInfoPanelXDestination = 2000.0f;
            } else {
                turnInfoPanelXDestination = -2000.0f;
            }
            turnInfo.transform.DOLocalMove(new Vector3(turnInfoPanelXDestination,0f,0f), 1.5f).OnComplete(HideTurnInfoView);
        }

        public void StartAnimation()
        {
            HideTurnInfo();
            if (TurnController.Instance.isPlayer1Turn) 
            {
                SetTurnInfoText("Player 1's Turn");
                turnInfo.transform.DOLocalMove(new Vector3(-2000f,0f,0f), 0f);
            } else {
                SetTurnInfoText("Player 2's Turn");
                turnInfo.transform.DOLocalMove(new Vector3(2000f,0f,0f), 0f);
            }
            turnInfo.transform.DORestart();
            ShowTurnInfo();
            turnInfo.transform.DOLocalMove(new Vector3(0f,0f,0f), 1.5f).OnComplete(TurnInfoXAxisTransForm);
        }
   
        public void ShowTurnInfoView(int turnNum)
        {
            if (turnInfoView != null)
            {
                Show(turnNum);
            }
        }

        void HideTurnInfoView()
        {
            Hide();
        }
    }
}
