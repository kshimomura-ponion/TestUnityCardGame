using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestUnityCardGame.Presenter.Battle
{
public class SpellDropManager : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        CardController target = GetComponent<CardController>(); // nullの可能性もある

        if (spellCard == null) {
            return;
        }
        if (BattleViewController.Instance.CanUseSpell(target)) {
            // Dropで攻撃するのはプレイヤーのみであるため
            if(spellCard.GetOwner() == Player.Player1){
                spellCard.UseSpellTo(target, BattleViewController.Instance.player1Hero);
            } else {
                spellCard.UseSpellTo(target, BattleViewController.Instance.player2Hero);
            }
        }
    }
}
}