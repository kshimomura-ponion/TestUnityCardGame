
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MiniUnidux.Util;
using TestUnityCardGame.View.Battle;

namespace TestUnityCardGame.Presenter.Battle
{
    public class TurnInfoViewController : SingletonMonoBehaviour<TurnInfoViewController>
    {
        TurnInfoView turnInfoView;
        private void Awake()
        {
            turnInfoView = GetComponent<TurnInfoView>();
        }

        // この方法だと、必ず2回目は右から来てしまうため
        public void OnEnable()
        {
            turnInfoView.HideTurnInfo();
            if(TurnController.Instance.isPlayer1Turn == true) 
            {
                turnInfoView.SetTurnInfoText("Player 1's Turn");
                turnInfoView.turnInfo.transform.DOLocalMove(new Vector3(-2000f,0f,0f), 0f);
            } else {
                turnInfoView.SetTurnInfoText("Player 2's Turn");
                turnInfoView.turnInfo.transform.DOLocalMove(new Vector3(2000f,0f,0f), 0f);
            }
            turnInfoView.turnInfo.transform.DORestart();
            turnInfoView.ShowTurnInfo();
            turnInfoView.turnInfo.transform.DOLocalMove(new Vector3(0f,0f,0f), 1.5f).OnComplete(TurnInfoXAxisTransForm);
        }

        public void OnDisable()
        {
            turnInfoView.turnInfo.transform.DORewind();
        }

        void TurnInfoXAxisTransForm(){
            float turnInfoPanelXDestination = 0.0f;
            if(TurnController.Instance.isPlayer1Turn == true) 
            {
                turnInfoPanelXDestination = 2000.0f;
            } else {
                turnInfoPanelXDestination = -2000.0f;
            }
            turnInfoView.turnInfo.transform.DOLocalMove(new Vector3(turnInfoPanelXDestination,0f,0f), 1.5f).OnComplete(HideTurnInfoView);
        }

        public void ShowTurnInfoView(int turnNum)
        {
            if(turnInfoView != null)
            {
                turnInfoView.GetComponent<GameObject>().SetActive(true);
                turnInfoView.Show(turnNum);
            }
        }

        void HideTurnInfoView()
        {
            turnInfoView.GetComponent<GameObject>().SetActive(false);
        }
    }
}
