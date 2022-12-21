using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestUnityCardGame
{
    public class EntitiesManager : ScriptableObject
    {

        [SerializeField] List<HeroEntity> heroEntityList;
        [SerializeField] List<MonsterCardEntity> monsterCardEntityList;
        [SerializeField] List<SpellCardEntity> spellCardEntityList;

        public HeroEntity GetHeroEntity(int id)
        {
            return heroEntityList[id - 1];
        }

        public MonsterCardEntity GetMonsterCardEntity(int id)
        {
            return monsterCardEntityList[id - 1];
        }

        public SpellCardEntity GetSpellCardEntity(int id)
        {
            return spellCardEntityList[id - 1];
        }

        public CardEntity GetCardEntity(int id, CARDTYPE cardType)
        {
            if (cardType == CARDTYPE.MONSTER)
            {
                return new CardEntity(GetMonsterCardEntity(id));

            }
            else
            {
                return new CardEntity(GetSpellCardEntity(id));
            }
        }
    }
}