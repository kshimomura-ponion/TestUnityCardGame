using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using TestUnityCardGame.Presenter.Card;

namespace TestUnityCardGame.Domain.Card
{
    public class CardModel
    {
        private string name;
        private int hp;
        private int at;
        private int manaCost;
        private Sprite icon;
        private Skill skill;
        private Sprite skillIcon;

        private bool isAlive;

        public CardModel(CardEntity cardEntity)
        {
            name = cardEntity.name;
            at = cardEntity.at;
            manaCost = cardEntity.manaCost;
            icon = cardEntity.icon;
            hp = cardEntity.hp;
            skill = cardEntity.skill;
            skillIcon = cardEntity.skillIcon;
            isAlive = true;
        }

        public void Damage(int dmg)
        {
            hp -= dmg;
            if (hp <= 0) {
                hp = 0;
                isAlive = false;
            }
        }

        // 敵を攻撃する
        public int Attack(CardController card)
        {
            Damage(at);
            return at;
        }

        // 自分を回復する
        void RecoveryHP(int point)
        {
            hp += point;
        }

        // cardを回復させる
        public int Heal(CardController card)
        {
            RecoveryHP(at);
            return at;
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

        public Ability GetAbility()
        {
            return skill.GetAbility();
        }

        public Spell GetSpell()
        {
            return skill.GetSpell();
        }

        public Sprite GetSkillIcon()
        {
            return skillIcon;
        }

        public bool IsSpell()
        {
            if (skill.GetSpell() == Spell.None) {
                return false;
            } else {
                return true;
            }
        }

        public void SetIsAlive(bool isAliveOrDead)
        {
            isAlive = isAliveOrDead;
        }
        public bool IsAlive()
        {
            return isAlive;
        }
    }
}