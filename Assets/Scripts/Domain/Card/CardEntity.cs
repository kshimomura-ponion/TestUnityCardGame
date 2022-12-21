using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEntity
{
    public string name;
    public int hp;
    public int at;
    public int manaCost;
    public Sprite icon;
    public Skill skill;
    public Sprite skillIcon;

    public CardEntity(MonsterCardEntity monsterCardEntity)
    {
        name = monsterCardEntity.name;
        hp = monsterCardEntity.hp;
        at = monsterCardEntity.at;
        manaCost = monsterCardEntity.manaCost;
        icon = monsterCardEntity.icon;
        skill = new Skill(monsterCardEntity.ability);
        skillIcon = monsterCardEntity.abilityIcon;
    }

    public CardEntity(SpellCardEntity spellCardEntity)
    {
        name = spellCardEntity.name;
        hp = 1; // スペルカードにHPはいらないが、エラーとなるので1を設定する（使用後に即廃棄する）
        at = spellCardEntity.at;
        manaCost = spellCardEntity.manaCost;
        icon = spellCardEntity.icon;
        skill = new Skill(spellCardEntity.spell); 
        skillIcon = spellCardEntity.spellIcon;
    }
}
