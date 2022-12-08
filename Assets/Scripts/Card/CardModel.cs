using System.Collections.Generic;
using UnityEngine;

public class CardModel
{
    private string name;
    private int hp;
    private int at;
    private int manaCost;
    private Sprite icon;
    private ABILITY ability;
    private SPELL spell;

    private bool isAlive;
    private bool canAttack;
    private bool isFieldCard;

    public CardModel(int id, CARDTYPE cardType)
    {
        switch (cardType) {
            case CARDTYPE.MONSTER:
                MonsterCardEntity monsterCardEntity = Resources.Load<MonsterCardEntity>("MonsterCardEntities/MonsterCard"+ id.ToString());
                name = monsterCardEntity.name;
                hp = monsterCardEntity.hp;
                at = monsterCardEntity.at;
                manaCost = monsterCardEntity.manaCost;
                icon = monsterCardEntity.icon;
                ability = monsterCardEntity.ability;
                spell = SPELL.NONE;
                break;
            case CARDTYPE.SPELL:
                SpellCardEntity spellCardEntity = Resources.Load<SpellCardEntity>("SpellCardEntities/SpellCard"+ id.ToString());
                name = spellCardEntity.name;
                hp = 1; // スペルカードにHPはいらないが、エラーとなるので1を設定する（使用後に即廃棄する）
                at = spellCardEntity.at;
                manaCost = spellCardEntity.manaCost;
                icon = spellCardEntity.icon;
                ability = ABILITY.NONE;
                spell = spellCardEntity.spell; 
                break;
        }
        isAlive = true;
        canAttack = false;
        isFieldCard = false;
    }

    public void OnField()
    {
        isFieldCard = true;
        // 速攻カードの場合
        if (ability == ABILITY.INIT_ATTACKABLE) {
            SetCanAttack(true);
        }
    }

    public void Damage(int dmg)
    {
        hp -= dmg;
        if(hp <= 0) {
            hp = 0;
            isAlive = false;
        }
    }

    // 敵を攻撃する
    public int Attack(CardController card)
    {
        card.model.Damage(at);

        return at;
    }

    // 自分を回復する
    void RecoveryHP(int point)
    {
        hp += point;
    }

    // cardを回復させる
    public void Heal(CardController card)
    {
        card.model.RecoveryHP(at);
    }

    public string GetName()
    {
        return name;
    }

    public int GetHP()
    {
        return hp;
    }

    public int GetAT()
    {
        return at;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetManaCost()
    {
        return manaCost;
    }

    public ABILITY GetAbility()
    {
        return ability;
    }

    public SPELL GetSpell()
    {
        return spell;
    }

    public bool IsSpell()
    {
        if(spell == SPELL.NONE) {
            return false;
        } else {
            return true;
        }
    }

    public void SetIsFieldCard(bool isField)
    {
        isFieldCard = isField;
    }

    public bool IsFieldCard()
    {
        return isFieldCard;
    }

    public void SetIsAlive(bool isAliveOrDead)
    {
        isAlive = isAliveOrDead;
    }
    public bool IsAlive()
    {
        return isAlive;
    }

    public void SetCanAttack(bool isAttackable)
    {
        canAttack = isAttackable;
    }
    public bool CanAttack()
    {
        return canAttack;
    }
}
