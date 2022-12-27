using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Card;

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

            HeroController ownerHero = null;
            if (spellCard.GetOwner() == Player.Player1) {
                ownerHero = BattleViewController.Instance.player1Hero;
            } else if (spellCard.GetOwner() == Player.Player2) {
                ownerHero = BattleViewController.Instance.player2Hero;
            }

            if (ownerHero != null) {
                if (spellCard.model.GetManaCost() > ownerHero.model.GetManaCost().Value || ownerHero.model.GetManaCost().Value < 0) {
                    return;
                }

                if (spellCard.model.GetSpell() == Spell.AttackEnemyCard || spellCard.model.GetSpell() == Spell.HealFriendCard) {
                    if (targetCard != null && spellCard.model.GetManaCost() <= ownerHero.model.GetManaCost().Value && ownerHero.model.GetManaCost().Value > 0) {
                        // Hand上のカードには使用できない
                        if(targetCard.IsFieldCard()){
                            StartCoroutine(spellCard.UseSpellToCard(targetCard, ownerHero, null));
                        }
                    }
                } else if(spellCard.model.GetSpell() == Spell.AttackEnemyHero || spellCard.model.GetSpell() == Spell.HealFriendHero) {
                    if (targetHero != null && spellCard.model.GetManaCost() <= ownerHero.model.GetManaCost().Value && ownerHero.model.GetManaCost().Value > 0) {
                        StartCoroutine(spellCard.UseSpellToHero(targetHero, ownerHero, null));
                    }
                }
            }
        }
    }
}