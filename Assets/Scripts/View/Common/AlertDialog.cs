using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlertDialog : MonoBehaviour
{
    [SerializeField] GameObject alertDialog;
    [SerializeField] TextMeshProUGUI alertText;

    private void Awake()
    {
        alertDialog.SetActive(false);
    }

    public void ShowAlertDialog(string message)
    {
        alertText.text = message;
        alertDialog.SetActive(true);
    }

    public void HideAlertDialog()
    {
        alertDialog.SetActive(false);
    }
}
