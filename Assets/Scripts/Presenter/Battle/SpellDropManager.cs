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
        CardController targetCard = GetComponent<CardController>(); // nullの可能性もある
        HeroController targetHero = GetComponent<HeroController>(); // nullの可能性もある

        if (spellCard == null) {
            return;
        }
        CardController[] targetCards = BattleViewController.Instance.GetOpponentFieldCards(spellCard.GetOwner());

        if (spellCard.CanUseSpell(targetCards)) {
            // Dropで攻撃するのはプレイヤーのみ
            if(spellCard.GetOwner() == Player.Player1){
                if(targetCard != null) {
                    spellCard.UseSpellTo(targetCard, BattleViewController.Instance.player1Hero);
                } else if(targetHero != null){
                    spellCard.UseSpellTo(targetHero, BattleViewController.Instance.player1Hero);
                }
            } else {
                if(targetCard != null) {
                    spellCard.UseSpellTo(targetCard, BattleViewController.Instance.player2Hero);
                } else if(targetHero != null){
                    spellCard.UseSpellTo(targetHero, BattleViewController.Instance.player2Hero);
                }
            }
        }
    }
}
}