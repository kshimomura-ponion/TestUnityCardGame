using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Card;
using TestUnityCardGame.Presenter.Battle;

namespace TestUnityCardGame.Presenter.Card
{
    public class AttackedCard : MonoBehaviour, IDropHandler
    {
        // 攻撃された側で攻撃処理を行う
        public void OnDrop(PointerEventData eventData)
        {
            CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
            CardController defender = GetComponent<CardController>();

            if (attacker == null || defender == null) {
                return;
            }

            if (attacker.GetOwner() == defender.GetOwner()) {
                return;
            }

            //　シールドカードがあればシールドカード以外は攻撃できない
            if (BattleViewController.Instance.ExistsSheldCard(attacker.GetOwner()) && defender.model.GetAbility() != Ability.Shield) {
                return;
            }

            if (attacker.model.CanAttack()) {
                // attackerとdefenderを戦わせる
                BattleViewController.Instance.CardsBattle(attacker, defender);
            }
        }
    }
}