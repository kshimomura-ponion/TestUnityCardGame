using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCard : MonoBehaviour, IDropHandler
{
    // 攻撃された側で攻撃処理を行う
    public void OnDrop(PointerEventData eventData)
    {
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        CardController defender = GetComponent<CardController>();
        GameManager.instance.CardsBattle(attacker, defender);
    }
}
