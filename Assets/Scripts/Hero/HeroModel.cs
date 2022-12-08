using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroModel
{
    private string name;
    private int hp;
    private int manaCost;
    private Sprite icon;
    private bool isAlive;

    // Cardの種類を把握するために、Tupleでカード情報を持っておくようにする
    private List<(int, CARDTYPE)> cardDeck = new List<(int, CARDTYPE)>();

    public HeroModel(int id, List<int> deck, PLAYER player)
    {
        HeroEntity heroEntity = Resources.Load<HeroEntity>("HeroEntities/Hero"+ id.ToString());
        name = heroEntity.name;
        manaCost = 5;  // Initial Value of Mana Cost.
        hp = heroEntity.hp;
        if(player == PLAYER.PLAYER1){
            icon = heroEntity.leftIcon;
        } else {
            icon = heroEntity.rightIcon;
        }
        isAlive = true;

        // FIXME:カードの奇数をモンスターカード、偶数をスペルカードにする（もっと良い方法があったら変えてください）
        List<(int, CARDTYPE)> tmpCardDeck = new List<(int, CARDTYPE)>();
        int monsterCardID = 1;
        int spellCardID = 1;
        for(int i = 0; i < deck.Count; i++){
            if(i % 2 != 0)
            {
                tmpCardDeck.Add((monsterCardID, CARDTYPE.SPELL));
                monsterCardID++;
            } else {
                tmpCardDeck.Add((spellCardID, CARDTYPE.MONSTER));
                spellCardID++;
            }
        }

        // 公平性を期すため、構築後はデッキの内容をシャッフルする（System.Linq）
        var random = new System.Random();
        cardDeck = tmpCardDeck.OrderBy(x => random.Next()).ToList();
    }

    // デッキのidx番目のカードをデッキから取り除く
    public void RemoveCard(int idx)
    {
        cardDeck.RemoveAt(idx);
    }

    public void Damage(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    public void Heal(int heal)
    {
        hp += heal;
    }

    public void AddManaCost(int cost)
    {
        manaCost += cost;
    }

    public void ReduceManaCost(int cost)
    {
        manaCost -= cost;
    }

    public string GetName()
    {
        return name;
    }

    public int GetHP()
    {
        return hp;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public void SetIsAlive(bool isAliveOrDead)
    {
        isAlive = isAliveOrDead;
    }
    public bool IsAlive()
    {
        return isAlive;
    }

    public int GetManaCost()
    {
        return manaCost;
    }

    public List<(int, CARDTYPE)> GetCardDeck()
    {
        return cardDeck;
    }
}