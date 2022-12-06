using System.Collections.Generic;
using UnityEngine;

public class CardModel
{
    public string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public bool isAlive;

    public CardModel(int id)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card"+ id.ToString());
        name = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        isAlive = true;
    }

    void Damage(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    public int Attack(CardController card)
    {
        card.model.Damage(at);

        return at;
    }
}
