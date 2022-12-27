using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace TestUnityCardGame.Domain.Hero{
    public class HeroModel
    {
        private int heroID;
        private string name;
        private ReactiveProperty<int> hp;
        private ReactiveProperty<int> manaCost;
        private Sprite icon;
        private HEROTYPE heroType;
        private string info;
        private bool isAlive;

        private int initialManaCost = 5;    // initial manacost with hero.

        private HeroEntity heroEntity;

        // Cardの種類を把握するために、Tupleでカード情報を持っておくようにする
        private List<(int, CardType)> cardDeck = new List<(int, CardType)>();

        public HeroModel(HeroEntity heroEntity, List<int> deck, Player player)
        {
            // 監視用のオブジェクトのインスタンス化
            hp = new ReactiveProperty<int>(heroEntity.hp);
            manaCost = new ReactiveProperty<int>(initialManaCost);

            heroID = heroEntity.id;
            name = heroEntity.name;
            if (player == Player.Player1) {
                icon = heroEntity.leftIcon;
            } else {
                icon = heroEntity.rightIcon;
            }
            heroType = heroEntity.heroType;
            info = heroEntity.info;
            isAlive = true;

            if (deck.Count != 0) {
                // ヒーローのタイプによって比率を変える
                List<(int, CardType)> tmpCardDeck = new List<(int, CardType)>();
                int monsterCardIdx = 0;
                int spellCardIdx = 0;
                int spellNum = 0;
                int monsterNum = 0;
                switch (heroType) {
                    case HEROTYPE.MAGICUSER:
                        spellNum = (int)Math.Floor(deck.Count * 0.8f);
                        monsterNum = deck.Count - spellNum;
                        for(int i = 0; i < spellNum; i++) {
                            tmpCardDeck.Add((deck[spellCardIdx], CardType.Spell));
                            spellCardIdx++;
                        }
                        for(int i = 0; i < monsterNum; i++) {
                            tmpCardDeck.Add((deck[monsterCardIdx], CardType.Monster));
                            monsterCardIdx++;
                        }
                        break;
                    case HEROTYPE.MAGICFIGHTER:
                        for(int i = 0; i < deck.Count; i++) {
                            if (i % 2 == 0)
                            {
                                tmpCardDeck.Add((deck[spellCardIdx], CardType.Spell));
                                spellCardIdx++;
                            } else {
                                tmpCardDeck.Add((deck[monsterCardIdx], CardType.Monster));
                                monsterCardIdx++;
                            }
                        }
                        break;
                    case HEROTYPE.WARRIOR:
                        spellNum = (int)Math.Floor(deck.Count * 0.2f);
                        monsterNum = deck.Count - spellNum;
                        for(int i = 0; i < spellNum; i++) {
                            tmpCardDeck.Add((deck[spellCardIdx], CardType.Spell));
                            spellCardIdx++;
                        }
                        for(int i = 0; i < monsterNum; i++) {
                            tmpCardDeck.Add((deck[monsterCardIdx], CardType.Monster));
                            monsterCardIdx++;
                        }
                    break;
                }

                // 公平性を期すため、構築後はデッキの内容をシャッフルする（System.Linq）
                var random = new System.Random();
                cardDeck = tmpCardDeck.OrderBy(x => random.Next()).ToList();
            }
        }

        // デッキのidx番目のカードをデッキから取り除く
        public void RemoveCard(int idx)
        {
            cardDeck.RemoveAt(idx);
        }

        public void Damage(int dmg)
        {
            hp.Value -= dmg;
            if (hp.Value <= 0)
            {
                hp.Value = 0;
                isAlive = false;
            }
        }

        public void Heal(int heal)
        {
            hp.Value += heal;
        }

        public void AddManaCost(int cost)
        {
            manaCost.Value += cost;
        }

        public void ReduceManaCost(int cost)
        {
            manaCost.Value -= cost;
        }

        public int GetHeroID()
        {
            return heroID;
        }

        public string GetName()
        {
            return name;
        }

        public ReactiveProperty<int> GetHP()
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

        public ReactiveProperty<int> GetManaCost()
        {
            return manaCost;
        }

        public List<(int, CardType)> GetCardDeck()
        {
            return cardDeck;
        }

        public HEROTYPE GetHeroType()
        {
            return heroType;
        }

        public string GetInfo()
        {
            return info;
        }
    }
}