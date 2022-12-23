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

            var ownerHero = BattleViewController.Instance.player1Hero;
            if (spellCard.GetOwner() == Player.Player2) {
                ownerHero = BattleViewController.Instance.player2Hero;
            }

            CardController[] enemyCards = BattleViewController.Instance.GetOpponentFieldCards(spellCard.GetOwner());

            if (spellCard.CanUseSpellToCard(enemyCards)) {
                if (targetCard != null) {
                    StartCoroutine(spellCard.UseSpellToCard(targetCard, ownerHero));
                }
            }
            if (spellCard.CanUseSpellToHero(targetHero)) {
                if (targetHero != null) {
                    StartCoroutine(spellCard.UseSpellToHero(targetHero, ownerHero));
                }
            }

           CardController[] friendCards = BattleViewController.Instance.GetFriendFieldCards(spellCard.GetOwner());

            if (spellCard.CanUseSpellToCard(friendCards)) {
                if (targetCard != null) {
                    StartCoroutine(spellCard.UseSpellToCard(targetCard, ownerHero));
                }
            }
            if (spellCard.CanUseSpellToHero(targetHero)) {
                if (targetHero != null) {
                    StartCoroutine(spellCard.UseSpellToHero(targetHero, ownerHero));
                }
            }
        }
    }
}