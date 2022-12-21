using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestUnityCardGame
{
public class AttackedCard : MonoBehaviour, IDropHandler
{
    // 攻撃された側で攻撃処理を行う
    public void OnDrop(PointerEventData eventData)
    {
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        CardController defender = GetComponent<CardController>();

        if (attacker == null || defender == null)
        {
            return;
        }

        if (attacker.GetOwner() == defender.GetOwner())
        {
            return;
        }

        //　シールドカードがあればシールドカード以外は攻撃できない
        /* CardController[] enemyFieldCards = GameManager.instance.GetOpponentFieldCards(attacker.GetOwner());
        if (Array.Exists(enemyFieldCards, card => card.model.GetAbility() == ABILITY.SHIELD)
            && defender.model.GetAbility() != ABILITY.SHIELD)
        {
            return;
        }*/

        if (attacker.model.CanAttack())
        {
            // attackerとdefenderを戦わせる
            // GameManager.instance.turnController.CardsBattle(attacker, defender);
        }
    }
}
}