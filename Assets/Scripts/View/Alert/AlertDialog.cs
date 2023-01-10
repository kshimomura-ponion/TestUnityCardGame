using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Common;
using TestUnityCardGame.Presenter.Common;

namespace TestUnityCardGame.View.Alert{
    public class AlertDialog : MonoBehaviour
    {
        private SoundManager soundManager;

        [SerializeField] GameObject alertDialog;
        [SerializeField] TextMeshProUGUI alertText;

        private void Awake()
        {
            alertDialog.SetActive(false);

            // CommonからSoundManagerを取得する
            soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");
        }

        public void ShowAlertDialog(string message)
        {
            if (soundManager != null) soundManager.PlaySE(SE.DialogOpen); // ダイアログが出たよという意味で
            alertText.text = message;
            alertDialog.SetActive(true);
        }

        public void HideAlertDialog()
        {
            alertDialog.SetActive(false);
        }
    }
}
