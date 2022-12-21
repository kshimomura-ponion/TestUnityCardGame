using System;
using System.Diagnostics.Tracing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestUnityCardGame
{
public class DropPlace : MonoBehaviour, IDropHandler
{
    // 手札なのかFieldなのか
    public TYPE type;

    public void OnDrop(PointerEventData eventData)
    {
        if(type == TYPE.HAND) {
            return;
        }
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if(card != null) {
            if(!card.movement.IsDraggable()){
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

            // ドラッグ中のカードの親コンポーネントを自分に変える
            card.movement.SetDefaultParent(this.transform);

            // 敵のFieldに配置しないようにする
            /*if(card.GetOwner() == PLAYER.PLAYER1 && this.transform != gameManager.player1FieldTransform){
                return;
            } else if(card.GetOwner() == PLAYER.PLAYER2 && this.transform != gameManager.player2FieldTransform){
                return;
            }*/

            // 自分のカードがフィールドに出たことを明示する
            card.model.OnField();

            // 自分のカードを表示状態にする
            card.view.SetActiveFrontPanel(true);

            // Mana Costを減らす
            switch(card.GetOwner()){
                case PLAYER.PLAYER1:
                    // gameManager.player1Hero.ReduceManaCost(card.model.GetManaCost());
                    break;
                case PLAYER.PLAYER2:
                   // gameManager.player2Hero.ReduceManaCost(card.model.GetManaCost());
                    break;
            }
            card.model.SetIsFieldCard(true);
        }
    }
}
}