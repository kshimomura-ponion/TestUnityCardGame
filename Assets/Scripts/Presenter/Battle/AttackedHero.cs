using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestUnityCardGame
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
        // CardController[] enemyFieldCards = GameManager.instance.GetOpponentFieldCards(attacker.GetOwner());
        /*if (Array.Exists(enemyFieldCards, card => card.model.GetAbility() == ABILITY.SHIELD))
        {
            return;
        }*/

        if (attacker.model.CanAttack())
        {
            // attackerがHeroに攻撃する
            if(attacker.GetOwner() == PLAYER.PLAYER1){
                // gameManager.player2Hero.Attacked(attacker);
            } else {
                // gameManager.player1Hero.Attacked(attacker);
            }
        }
    }
}
}