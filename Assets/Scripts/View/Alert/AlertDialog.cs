using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TestUnityCardGame.Domain.Sound;

namespace TestUnityCardGame.View.Alert{
    public class AlertDialog : MonoBehaviour
    {
        public AudioManager audioManager;

        [SerializeField] GameObject alertDialog;
        [SerializeField] TextMeshProUGUI alertText;

        private void Awake()
        {
            alertDialog.SetActive(false);
        }

        public void ShowAlertDialog(string message)
        {
            audioManager.PlaySE(SE.DialogOpen); // ダイアログが出たよという意味で
            alertText.text = message;
            alertDialog.SetActive(true);
        }

        public void HideAlertDialog()
        {
            alertDialog.SetActive(false);
        }
    }
}
