
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TestUnityCardGame.Presenter.Battle;

namespace TestUnityCardGame.View.Battle
{
    public class TurnInfoView : MonoBehaviour
    {
        public GameObject turnInfo;
        [SerializeField] TextMeshProUGUI turnInfoText;
        [SerializeField] TextMeshProUGUI turnNumInfoText;

        private void Awake()
        {
            HideTurnInfo();
        }

        public void Show(int turnNum)
        {
            turnNumInfoText.text = "Turn " + turnNum.ToString();
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

        
    }
}
