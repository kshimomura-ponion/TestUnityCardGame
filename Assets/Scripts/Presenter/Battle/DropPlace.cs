using System;
using System.Diagnostics.Tracing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TestUnityCardGame.Domain.Hero;
using TestUnityCardGame.Domain.Card;

namespace TestUnityCardGame.Presenter.Battle
{
    public class DropPlace : MonoBehaviour, IDropHandler
    {
        // 手札なのかFieldなのか
        public PlaceType placeType;

        public void OnDrop(PointerEventData eventData)
        {
            if(placeType == PlaceType.Hand) {
                return;
            }
            CardController card = eventData.pointerDrag.GetComponent<CardController>();
            if(card != null) {
                if(!card.IsDraggable()){
                    return;
                }

                // スペルカードは場にセットできない
                if (card.model.IsSpell()) {
                    return;
                }

                // すでに場に出ているカードは動かせない
                if(card.model.IsFieldCard()) {
                    return;
                }

                // 敵のFieldに配置しないようにする
                if(card.GetOwner() == Player.Player1 && this.transform != BattleViewController.Instance.GetPlayer1FieldTransform()){
                    return;
                } else if(card.GetOwner() == Player.Player2 && this.transform != BattleViewController.Instance.GetPlayer2FieldTransform()){
                    return;
                }

                // ドラッグ中のカードの親コンポーネントを自分に変える
                card.movement.SetDefaultParent(this.transform);

                // 自分のカードがフィールドに出たことを明示する
                card.model.OnField();

                // 自分のカードを表示状態にする
                card.view.SetActiveFrontPanel(true);

                // Mana Costを減らす
                switch(card.GetOwner()){
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