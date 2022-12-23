using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestUnityCardGame.Presenter.Battle
{
    public class AttackedHero : MonoBehaviour, IDropHandler
    {

        public void OnDrop(PointerEventData eventData)
        {
            // attackerカードを選択
            CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
            if (attacker == null)
            {
                return;
            }

            // 敵フィールドにシールドカードがあれば攻撃できない
            CardController[] enemyFieldCards = BattleViewController.Instance.GetOpponentFieldCards(attacker.GetOwner());
            if (Array.Exists(enemyFieldCards, card => card.model.GetAbility() == ABILITY.SHIELD))
            {
                return;
            }

            if (attacker.model.CanAttack())
            {
                // attackerがHeroに攻撃する
                if(attacker.GetOwner() == Player.Player1){
                    UnityEngine.Debug.Log("Attacked Hero");
                    BattleViewController.Instance.player2Hero.Attacked(attacker);
                } else {
                    BattleViewController.Instance.player1Hero.Attacked(attacker);
                }
            }
        }
    }
}