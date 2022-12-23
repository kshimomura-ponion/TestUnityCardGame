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

                // スペルカードかつ全体攻撃（回復）ならばフィールドに出た瞬間に使用する
                if (card.model.IsSpell()) {
                    UnityEngine.Debug.Log(card.model.GetSpell().ToString());
                    var ownerHero = BattleViewController.Instance.player1Hero;
                    if (card.GetOwner() == Player.Player2) {
                        ownerHero = BattleViewController.Instance.player2Hero;
                    }

                    if (card.model.GetSpell() == Spell.AttackEnemyCards) {
                        CardController[] enemyCards = BattleViewController.Instance.GetOpponentFieldCards(card.GetOwner());
                        StartCoroutine(card.UseSpellToCards(enemyCards, ownerHero));

                    } else if (card.model.GetSpell() == Spell.HealFriendCards) {
                        CardController[] friendCards = BattleViewController.Instance.GetFriendFieldCards(card.GetOwner());
                        StartCoroutine(card.UseSpellToCards(friendCards, ownerHero));
                    }
                    return;
                }

                // すでに場に出ているカードは動かせない
                if (card.model.IsFieldCard()) {
                    return;
                }

                // 敵のFieldに配置しないようにする
                if (card.GetOwner() == Player.Player1 && this.transform != BattleViewController.Instance.GetPlayer1FieldTransform()) {
                    return;
                } else if (card.GetOwner() == Player.Player2 && this.transform != BattleViewController.Instance.GetPlayer2FieldTransform()) {
                    return;
                }

                // ドラッグ中のカードの親コンポーネントを自分に変える
                card.movement.SetDefaultParent(this.transform);

                // 自分のカードがフィールドに出たことを明示する
                card.model.OnField();

                // 自分のカードを表示状態にする
                card.view.SetActiveFrontPanel(true);

                // Mana Costを減らす
                switch(card.GetOwner()) {
                    case Player.Player1:
                        BattleViewController.Instance.player1Hero.ReduceManaCost(card.model.GetManaCost());
                        break;
                    case Player.Player2:
                        BattleViewController.Instance.player2Hero.ReduceManaCost(card.model.GetManaCost());
                        break;
                }
                card.model.OnField();
            }
        }
    }
}