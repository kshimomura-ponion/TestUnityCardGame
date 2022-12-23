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
using TestUnityCardGame.Domain.Hero;
using TestUnityCardGame.Domain.Card;
using TestUnityCardGame.Presenter.Battle;


namespace TestUnityCardGame.View.Battle
{
    public class BattleView: MonoBehaviour
    {
        public Button turnEndButton;

        // Hero Prefab
        [SerializeField] HeroController heroPrefab;

        // Card Prefab
        [SerializeField] CardController cardPrefab;

        // ターン情報ビュー
        [SerializeField] TurnInfoView turnInfoViewPrefab;

        // ターン数（メイン画面）
        [SerializeField] TextMeshProUGUI turnNumText;

        void Awake(){}

        void Start()
        {}

        public void TurnendButtonActivate(bool activeState)
        {
            turnEndButton.interactable = activeState;
        }

        public void SetTurnNumText(string turnNumString)
        {
            turnNumText.text = turnNumString;
        }

        public TurnInfoView GetTurnInfoViewPrefab()
        {
            return turnInfoViewPrefab;
        }

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