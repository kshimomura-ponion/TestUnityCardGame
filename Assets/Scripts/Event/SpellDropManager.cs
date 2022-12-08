using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellDropManager : MonoBehaviour, IDropHandler
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        CardController target = GetComponent<CardController>(); // nullの可能性もある

        if (spellCard == null) {
            return;
        }
        if (spellCard.CanUseSpell()) {
            // Dropで攻撃するのはプレイヤーのみであるため
            if(spellCard.GetOwner() == PLAYER.PLAYER1){
                spellCard.UseSpellTo(target, gameManager.player1Hero);
            } else {
                spellCard.UseSpellTo(target, gameManager.player2Hero);
            }
        }
    }
}