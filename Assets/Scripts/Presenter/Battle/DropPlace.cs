using System;
using System.Diagnostics.Tracing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Card;

namespace TestUnityCardGame.Presenter.Battle
{
    public class DropPlace : MonoBehaviour, IDropHandler
    {
        // 手札なのかFieldなのか
        public PlaceType placeType;

        public void OnDrop(PointerEventData eventData)
        {
            if (placeType == PlaceType.Hand) {
                return;
            }
            CardController card = eventData.pointerDrag.GetComponent<CardController>();
            if (card != null) {
                if (!card.IsDraggable()) {
                    return;
                }

                // すでに場に出ているカードは動かせない
                if (card.IsFieldCard()) {
                    return;
                }

                // 敵のFieldに配置しないようにする
                if (card.GetOwner() == Player.Player1 && this.transform != BattleViewController.Instance.GetPlayer1FieldTransform()) {
                    return;
                } else if (card.GetOwner() == Player.Player2 && this.transform != BattleViewController.Instance.GetPlayer2FieldTransform()) {
                    return;
                }

                HeroController ownerHero = null;
                if (card.GetOwner() == Player.Player1) {
                    ownerHero = BattleViewController.Instance.player1Hero;
                } else if (card.GetOwner() == Player.Player2) {
                    ownerHero = BattleViewController.Instance.player2Hero;
                }

                // スペルカードかつ全体攻撃（回復）ならばMana Costを減らした上でフィールドに出た瞬間に使用する
                if (card.model.IsSpell()) {
                    if(ownerHero != null) {
                        if (card.model.GetSpell() == Spell.AttackEnemyCards) {
                            CardController[] enemyCards = BattleViewController.Instance.GetOpponentFieldCards(card.GetOwner());
                            if (enemyCards.Length > 0) {
                                StartCoroutine(card.UseSpellToCards(enemyCards, ownerHero, null));
                            } else {
                                return;
                            }

                        } else if (card.model.GetSpell() == Spell.HealFriendCards) {
                            CardController[] friendCards = BattleViewController.Instance.GetFriendFieldCards(card.GetOwner());
                            if (friendCards.Length > 0) {
                                StartCoroutine(card.UseSpellToCards(friendCards, ownerHero, null));
                            } else {
                                return;
                            }
                        }
                    }
                    return;
                // モンスターカードならMana Costを減らすだけ
                } else {
                    // ドラッグ中のカードの親コンポーネントを自分に変える
                    card.movement.SetDefaultParent(this.transform);

                    // 自分のカードがフィールドに出たことを明示する
                    card.OnField();

                    // 自分のカードを表示状態にする
                    card.view.SetActiveFrontPanel(true);

                    // Mana Costを減らす
                    if(ownerHero != null) {
                        ownerHero.ReduceManaCost(card.model.GetManaCost());
                    }
                }
            }
        }
    }
}