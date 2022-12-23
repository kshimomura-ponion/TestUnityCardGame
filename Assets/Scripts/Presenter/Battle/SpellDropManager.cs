using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TestUnityCardGame.Domain.Hero;
using TestUnityCardGame.Domain.Card;

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
            //UnityEngine.Debug.Log(string.Join(",", targetCards.Select(n => n.ToString())));

            if (spellCard.CanUseSpell(targetCards)) {
                // Dropで攻撃するのはプレイヤーのみ
                if(spellCard.GetOwner() == Player.Player1){
                    
                    if(targetCard != null) {
                        spellCard.UseSpellTo(targetCard, BattleViewController.Instance.player1Hero);
                    }
                    if(targetHero != null){
                        spellCard.UseSpellTo(targetHero, BattleViewController.Instance.player1Hero);
                    }
                } else {
                    if(targetCard != null) {
                        spellCard.UseSpellTo(targetCard, BattleViewController.Instance.player2Hero);
                    } 
                    if(targetHero != null){
                        spellCard.UseSpellTo(targetHero, BattleViewController.Instance.player2Hero);
                    }
                }
            }
        }
    }
}