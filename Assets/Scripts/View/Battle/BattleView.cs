using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TMPro;

namespace TestUnityCardGame.View.Battle
{
    public class BattleView: MonoBehaviour
    {
        public Transform canvasTransform;  // canvas transform

        public Button turnEndButton;

        // Hero Prefab
        [SerializeField] HeroController heroPrefab;

        // Card Prefab
        [SerializeField] CardController cardPrefab;

        // ターン情報ビュー
        //[SerializeField] TurnInfoViewController turnInfoViewPrefab;

        // ターン数（メイン画面）
        [SerializeField] TextMeshProUGUI turnNumText;

        void Awake()
        {
            // ターン情報ビューを生成する
            /*if(turnInfoView == null)
            {
                turnInfoView = Instantiate(turnInfoViewPrefab, canvasTransform, false);
            }*/
        }

        void Start()
        {

        }

        public void TurnendButtonActivate(bool activeState)
        {
            turnEndButton.interactable = activeState;
        }

        public void SetTurnNumText(string turnNumString)
        {
            turnNumText.text = turnNumString;
        }

       /* public TurnInfoViewController GetTurnInfoViewPrefab()
        {
            return turnInfoViewPrefab;
        }*/

        public HeroController GetHeroPrefab()
        {
            return heroPrefab;
        }

        public CardController GetCardPrefab()
        {
            return cardPrefab;
        }
     }
}