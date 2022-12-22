using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using MiniUnidux.SceneTransition;
using TestUnityCardGame.Domain.Service;

namespace TestUnityCardGame.View.Result
{
    public class ResultView : MonoBehaviour
    {
        // リスタートボタン
        public Button restartButton;

        // リセレクトボタン
        public Button reselectButton;

        // 結果表示テキスト
        [SerializeField] TextMeshProUGUI resultText;

        public void SetResultText(string resultString)
        {   
            resultText.text = resultString;
        }
    }
}